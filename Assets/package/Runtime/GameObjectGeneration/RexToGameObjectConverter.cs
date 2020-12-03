/*
 * Copyright (c) 2019 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Networking;
using System.IO;
using System.Timers;

namespace RoboticEyes.Rex.RexFileReader.Internal
{
    internal class RexToGameObjectConverter
    {
        private int MaxConversionTimePerFrameMilliseconds
        {
            get
            {
                return Application.targetFrameRate > 0 ? (850 / Application.targetFrameRate) : 25; //take max 85% of frametime
            }
        }

        private enum YieldStatus
        {
            None,
            WaitOneFrame,
            Break
        }

        List<RexDataMesh> rexMeshes = new List<RexDataMesh>();
        List<RexDataImage> rexImages = new List<RexDataImage>();
        List<RexDataMaterialStandard> rexMaterials = new List<RexDataMaterialStandard>();
        List<RexDataLineSet> rexLineSets = new List<RexDataLineSet>();
        List<RexDataText> rexTexts = new List<RexDataText>();
        List<RexDataPointList> rexPointLists = new List<RexDataPointList>();

        Dictionary<UInt64, Texture2D> decompressedImages = new Dictionary<ulong, Texture2D>();
        Dictionary<UInt64, Material> extractedMaterials = new Dictionary<ulong, Material>();

        private bool yieldRequired = false;
        private Timer timer;

        private RexConverter rexConverter;
        private Coroutine conversionRoutine;

        private LoadedObjects loadedObjects;

        private bool cancellationPending;

        private bool markMeshesNoLongerReadable = true;

        private RexToGameObjectConverter (bool markMeshesNoLongerReadable)
        {
            rexConverter = RexConverter.Instance;
            loadedObjects = new LoadedObjects();
            this.markMeshesNoLongerReadable = markMeshesNoLongerReadable;
        }

        public static RexToGameObjectConverter GenerateGameObjectsFromRex (byte[] rexData, RexConverter.ConversionResultDelegate onConversionResult, bool markMeshesNoLongerReadable = true)
        {
            var converter = new RexToGameObjectConverter (markMeshesNoLongerReadable);
            converter.StartConversion (rexData, onConversionResult);
            return converter;
        }

        private void StartConversion (byte[] rexData, RexConverter.ConversionResultDelegate onConversionResult)
        {
            conversionRoutine = rexConverter.StartCoroutine (ConversionRoutine (rexData, onConversionResult));
        }

        internal void CancelConversion()
        {
            cancellationPending = true;
        }

        private IEnumerator ConversionRoutine (byte[] rexData, RexConverter.ConversionResultDelegate onConversionResult)
        {
            ThreadedRexLoader tLoader = new ThreadedRexLoader (rexData);
            tLoader.StartJob();

            while (!tLoader.IsDone)
            {
                // Only check every 50ms, no point in checking every frame
                yield return new WaitForSeconds (0.05f);
            }

            if (cancellationPending)
            {
                yield break;
            }

            if (tLoader.IsError)
            {
                Debug.LogError ("Failed reading Rex File: " + tLoader.ErrorMessage);
            }

            var rexObject = tLoader.rexObject;

            if (rexObject == null || rexObject.dataBlocks.Count == 0)
            {
                onConversionResult (false, null);
                yield break;
            }

            List<RexDataMesh> rexMeshes = new List<RexDataMesh>();
            List<RexDataImage> rexImages = new List<RexDataImage>();
            List<RexDataMaterialStandard> rexMaterials = new List<RexDataMaterialStandard>();
            List<RexDataLineSet> rexLineSets = new List<RexDataLineSet>();
            List<RexDataText> rexTexts = new List<RexDataText>();
            List<RexDataPointList> rexPointLists = new List<RexDataPointList>();

            Dictionary<UInt64, Texture2D> decompressedImages = new Dictionary<ulong, Texture2D>();
            Dictionary<UInt64, Material> extractedMaterials = new Dictionary<ulong, Material>();

            foreach (RexDataBlock dataBlock in rexObject.dataBlocks)
            {
                switch (dataBlock.type)
                {
                    case RexDataBlockType.Mesh:
                        rexMeshes.Add ((RexDataMesh) dataBlock);
                        break;

                    case RexDataBlockType.Image:
                        rexImages.Add ((RexDataImage) dataBlock);
                        break;

                    case RexDataBlockType.MeshMaterialStandard:
                        rexMaterials.Add ((RexDataMaterialStandard) dataBlock);
                        break;

                    case RexDataBlockType.LineSet:
                        rexLineSets.Add ((RexDataLineSet) dataBlock);
                        break;

                    case RexDataBlockType.Text:
                        rexTexts.Add ((RexDataText) dataBlock);
                        break;

                    case RexDataBlockType.PointList:
                        rexPointLists.Add ((RexDataPointList) dataBlock);
                        break;

                    default:
                        break;
                }
            }

            // create a timer that calls OnTimedYield every `MaxConversionTimePerFrame` milliseconds to prevent blocking of Render Thread
            timer = new Timer();
            timer.Elapsed += OnTimedYield;
            timer.Interval = MaxConversionTimePerFrameMilliseconds;
            timer.Start();

            if (!cancellationPending && rexTexts.Count > 0)
            {
                yield return rexConverter.StartCoroutine (GenerateText (rexTexts));
            }

            if (!cancellationPending && rexLineSets.Count > 0)
            {
                yield return rexConverter.StartCoroutine (GenerateLineSets (rexLineSets));
            }

            if (!cancellationPending && rexPointLists.Count > 0)
            {
                yield return rexConverter.StartCoroutine (GeneratePointLists (rexPointLists));
            }

            if (!cancellationPending && rexImages.Count > 0)
            {
                yield return rexConverter.StartCoroutine (DecompressImages (rexImages, decompressedImages));
            }

            if (!cancellationPending && rexMaterials.Count > 0)
            {
                yield return rexConverter.StartCoroutine (ExtractMaterials (rexMaterials, decompressedImages, extractedMaterials));
            }

            if (!cancellationPending && rexMeshes.Count > 0)
            {
                yield return rexConverter.StartCoroutine (GetGameObjectFromRexData (rexMeshes, extractedMaterials));
            }

            if (cancellationPending)
            {
                loadedObjects.DestroyLoadedObjects();
                rexConverter.SignalConverterFinished (this);
                cancellationPending = false;
                yield break;
            }

            if (loadedObjects.AllGameObjects.Count == 0)
            {
                onConversionResult (false, null);
                rexConverter.SignalConverterFinished (this);
                yield break;
            }

            timer.Stop();

            loadedObjects.CalculateGeometryBounds();
            onConversionResult (true, loadedObjects);
            rexConverter.SignalConverterFinished (this);
        }

        private void OnTimedYield (object source, object e)
        {
            timer.Stop();
            yieldRequired = true;
        }

        private YieldStatus GetCurrentYieldStatus()
        {
            if (cancellationPending)
            {
                return YieldStatus.Break;
            }
            else if (yieldRequired)
            {
                yieldRequired = false;
                // Don't forget to start the timer again after yielding!
                return YieldStatus.WaitOneFrame;
            }
            else
            {
                return YieldStatus.None;
            }
        }

        private IEnumerator DecompressImages (List<RexDataImage> input, Dictionary<UInt64, Texture2D> decompressedImages)
        {
            foreach (var image in input)
            {
                YieldStatus yieldStatus = GetCurrentYieldStatus();
                if (yieldStatus == YieldStatus.WaitOneFrame)
                {
                    yield return null;
                    timer.Start();
                }
                else if (yieldStatus == YieldStatus.Break)
                {
                    yield break;
                }



                if (image.compression == ImageCompression.JPEG || image.compression == ImageCompression.PNG)
                {
                    string tempImage = Path.Combine (Application.temporaryCachePath, "tempImage" + Guid.NewGuid().ToString());
                    File.WriteAllBytes (tempImage, image.data);
                    using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture ("file://" + tempImage, true))
                    {
                        yield return uwr.SendWebRequest();

                        if (uwr.isNetworkError || uwr.isHttpError)
                        {
                            Debug.LogError (uwr.error);
                            Debug.LogError (uwr.downloadHandler?.text);
                        }
                        else
                        {
                            decompressedImages[image.dataId] = DownloadHandlerTexture.GetContent (uwr);
                        }
                        File.Delete (tempImage);
                    }
                }
                else
                {
                    Debug.LogError ("Unsupported compression type! Image ID: " + image.dataId);
                }
            }
        }

        private IEnumerator ExtractMaterials (List<RexDataMaterialStandard> input, Dictionary<UInt64, Texture2D> textures,
                                              Dictionary<UInt64, Material> target)
        {
            foreach (var rexMaterial in input)
            {
                YieldStatus yieldStatus = GetCurrentYieldStatus();
                if (yieldStatus == YieldStatus.WaitOneFrame)
                {
                    yield return null;
                    timer.Start();
                }
                else if (yieldStatus == YieldStatus.Break)
                {
                    yield break;
                }

                Material material = rexConverter.CreateNewMaterial (rexMaterial.alpha < 1f);

                material.color = new Color (rexMaterial.kdRed, rexMaterial.kdGreen, rexMaterial.kdBlue, rexMaterial.alpha);

                if (rexMaterial.kdTextureId != Int64.MaxValue)
                {
                    if (textures.ContainsKey (rexMaterial.kdTextureId))
                    {
                        material.mainTexture = textures[rexMaterial.kdTextureId];
                    }
                    else
                    {
                        Debug.LogError ("Texture with ID " + rexMaterial.kdTextureId + " not found");
                    }
                }

                if (rexMaterial.ns > 0)
                {
                    if (rexMaterial.ksTextureId != Int64.MaxValue)
                    {
                        if (textures.ContainsKey (rexMaterial.ksTextureId))
                        {
                            material.SetTexture ("_SpecTex", textures[rexMaterial.ksTextureId]);
                        }
                        else
                        {
                            Debug.LogError ("Texture with ID " + rexMaterial.ksTextureId + " not found");
                        }
                    }

                    material.SetColor ("_SpecColor", new Color (rexMaterial.ksRed, rexMaterial.ksGreen, rexMaterial.ksBlue));
                    material.SetFloat ("_Ns", rexMaterial.ns);
                }

                if (rexMaterial.kaTextureId != Int64.MaxValue)
                {
                    if (textures.ContainsKey (rexMaterial.kaTextureId))
                    {
                        material.SetTexture ("_AmbTex", textures[rexMaterial.kaTextureId]);
                    }
                    else
                    {
                        Debug.LogError ("Texture with ID " + rexMaterial.ksTextureId + " not found");
                    }
                }

                material.SetColor ("_AmbColor", new Color (rexMaterial.kaRed, rexMaterial.kaGreen, rexMaterial.kaBlue, 0.5f));

                target[rexMaterial.dataId] = material;
            }
        }

        private IEnumerator GenerateText (List<RexDataText> input)
        {
            foreach (var inputText in input)
            {
                YieldStatus yieldStatus = GetCurrentYieldStatus();
                if (yieldStatus == YieldStatus.WaitOneFrame)
                {
                    yield return null;
                    timer.Start();
                }
                else if (yieldStatus == YieldStatus.Break)
                {
                    yield break;
                }

                GameObject rexTextGameObject = rexConverter.CreateNewText();
                RexTextObject rexTextObject = rexTextGameObject.GetComponentInChildren<RexTextObject>();

                if (rexTextObject == null)
                {
                    Debug.LogError ("Text Prefab needs to have a RexTextObject Component!!!");
                    UnityEngine.Object.Destroy (rexTextGameObject);
                    continue;
                }

                bool success = rexTextObject.SetText (inputText.text, inputText.color);

                if (success)
                {
                    loadedObjects.AddTextObject (rexTextObject);
                }
                else
                {
                    UnityEngine.Object.Destroy (rexTextGameObject);
                }
            }
        }

        private IEnumerator GenerateLineSets (List<RexDataLineSet> input)
        {
            foreach (var inputLineSet in input)
            {
                YieldStatus yieldStatus = GetCurrentYieldStatus();
                if (yieldStatus == YieldStatus.WaitOneFrame)
                {
                    yield return null;
                    timer.Start();
                }
                else if (yieldStatus == YieldStatus.Break)
                {
                    yield break;
                }

                GameObject rexLineSetGameObject = rexConverter.CreateNewLineSet();
                RexLineSetObject rexLineSetObject = rexLineSetGameObject.GetComponentInChildren<RexLineSetObject>();

                if (rexLineSetObject == null)
                {
                    Debug.LogError ("LineSet Prefab needs to have a RexLineSetObject Component!!!");
                    UnityEngine.Object.Destroy (rexLineSetGameObject);
                    continue;
                }

                bool success = rexLineSetObject.SetPositions (inputLineSet.vertices, inputLineSet.color);

                if (success)
                {
                    loadedObjects.AddLineSetObject (rexLineSetObject);
                }
                else
                {
                    UnityEngine.Object.Destroy (rexLineSetGameObject);
                }
            }
        }

        private IEnumerator GeneratePointLists (List<RexDataPointList> input)
        {
            foreach (var inputPointList in input)
            {
                YieldStatus yieldStatus = GetCurrentYieldStatus();
                if (yieldStatus == YieldStatus.WaitOneFrame)
                {
                    yield return null;
                    timer.Start();
                }
                else if (yieldStatus == YieldStatus.Break)
                {
                    yield break;
                }

                GameObject rexPointListGameObject = rexConverter.CreateNewPointSet();
                RexPointListObject rexPointListObject = rexPointListGameObject.GetComponentInChildren<RexPointListObject>();

                if (rexPointListObject == null)
                {
                    Debug.LogError ("PointList Prefab needs to have a RexPointListObject Component!!!");
                    UnityEngine.Object.Destroy (rexPointListGameObject);
                    continue;
                }

                bool success = rexPointListObject.SetPoints (inputPointList.vertices, inputPointList.colors);

                if (success)
                {
                    loadedObjects.AddPointSetObject (rexPointListObject);
                }
                else
                {
                    UnityEngine.Object.Destroy (rexPointListGameObject);
                }
            }
        }

        private IEnumerator GetGameObjectFromRexData (List<RexDataMesh> meshes, Dictionary<UInt64, Material> materials)
        {
            foreach (var rexMesh in meshes)
            {
                YieldStatus yieldStatus = GetCurrentYieldStatus();
                if (yieldStatus == YieldStatus.WaitOneFrame)
                {
                    yield return null;
                    timer.Start();
                }
                else if (yieldStatus == YieldStatus.Break)
                {
                    yield break;
                }

                if (rexMesh.textureCoordinates.Count > 0 && rexMesh.vertexCoordinates.Count != rexMesh.textureCoordinates.Count)
                {
                    Debug.LogError ("Number of UVs must match number of vertices! Mesh ID " + rexMesh.dataId);
                    continue;
                }

                bool hasVertexColors = rexMesh.vertexColors.Count != 0;

                if (hasVertexColors && rexMesh.vertexColors.Count != rexMesh.vertexCoordinates.Count)
                {
                    Debug.LogError ("Number of vertex colors must match number of vertices! Mesh ID " + rexMesh.dataId);
                    continue;
                }

                GameObject rexMeshGameObject = rexConverter.CreateNewMesh();
                RexMeshObject rexMeshObject = rexMeshGameObject.GetComponentInChildren<RexMeshObject>();

                if (rexMeshObject == null)
                {
                    Debug.LogError ("Mesh Prefab needs to have a RexMeshObject Component!!!");
                    UnityEngine.Object.Destroy (rexMeshGameObject);
                    continue;
                }

                rexMeshGameObject.SetActive (false);

                Mesh mesh = new Mesh
                {
                    name = "RexMesh",
                    subMeshCount = 1,
                };

                if (rexMesh.vertexCoordinates.Count >= 65535 || rexMesh.vertexTriangles.Length >= 65535)
                {
                    // Need to explicitly tell Unity to use 32Bit indexes for verts, edges and faces.
                    // Note: 32 bit indexed meshes cannot be batched
                    mesh.indexFormat = IndexFormat.UInt32;
                }

                try
                {
                    mesh.SetVertices (rexMesh.vertexCoordinates);

                    if (hasVertexColors)
                    {
                        mesh.SetColors (rexMesh.vertexColors);
                    }

                    mesh.SetUVs (0, rexMesh.textureCoordinates);
                    mesh.SetTriangles (Utils.FlipTriangles (rexMesh.vertexTriangles), 0);

                    if (rexMesh.normalVectors.Count > 0)
                    {
                        mesh.SetNormals (rexMesh.normalVectors);
                    }
                    else
                    {
                        mesh.RecalculateNormals();
                    }

                    mesh.RecalculateBounds();
                }
                catch (Exception)
                {
                    //corrupted data, prevent loading of this LOD
                    Debug.Log ("Error generating Mesh with ID " + rexMesh.dataId);
                    UnityEngine.Object.Destroy (rexMeshGameObject);
                    continue;
                }

                mesh.UploadMeshData (markMeshesNoLongerReadable);

                Material material = null;
                if (rexMesh.materialId != Int64.MaxValue && materials.ContainsKey (rexMesh.materialId))
                {
                    material = materials[rexMesh.materialId];
                }

                rexMeshObject.SetMeshData (mesh, material);

                loadedObjects.AddMeshObject (rexMeshObject);
            }
        }
    }
}

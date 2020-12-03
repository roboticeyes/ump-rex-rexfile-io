/*
 * Copyright (c) 2019 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */

using RoboticEyes.Rex.RexFileReader.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RoboticEyes.Rex.RexFileReader
{
    public class RexConverter : MonoBehaviour
    {
        [Header ("Prefabs")]
        [SerializeField]
        public GameObject textPrefab;

        [SerializeField]
        public GameObject lineSetPrefab;

        [SerializeField]
        public GameObject pointSetPrefab;

        [SerializeField]
        public GameObject meshPrefab;

        [Header ("Materials")]
        [SerializeField]
        public Material meshMaterialSolid;

        [SerializeField]
        public Material meshMaterialTransparent;

        [Header ("Conversion Options")]
        [Tooltip ("Needs to be set to false if you want MeshColliders on rex meshes")]
        [SerializeField]
        public bool markMeshesNoLongerReadable = true;

        public static RexConverter Instance
        {
            get;
            private set;
        }

        private List<RexToGameObjectConverter> activeConverters = new List<RexToGameObjectConverter>();

        public delegate void ConversionResultDelegate (bool success, LoadedObjects loadedObjects);

        private void Awake()
        {
            Instance = this;
        }

        public byte[] GenerateRexFile (params MeshFilter[] meshFilters)
        {
            List<KeyValuePair<Mesh, Material>> meshes = new List<KeyValuePair<Mesh, Material>> ();

            foreach (var mf in meshFilters)
            {
                meshes.Add (new KeyValuePair<Mesh, Material> (mf.sharedMesh, null));
            }

            return GenerateRexFile (meshes);
        }



        public byte[] GenerateRexFile (List<KeyValuePair<Mesh, Material>> meshMaterialPairs)
        {
            List<RexDataBlock> rexDataBlocks = new List<RexDataBlock> ();

            Dictionary<Material, RexDataMaterialStandard> materialDataBlocks = new Dictionary<Material, RexDataMaterialStandard> ();

            bool hasDefaultMaterial = false;
            RexDataMaterialStandard defaultMaterialBlock = new RexDataMaterialStandard (new Color (1f, 1f, 1f), 1f);

            foreach (var mmPair in meshMaterialPairs)
            {
                Material material = mmPair.Value;
                Mesh mesh = mmPair.Key;
                ulong materialId = long.MaxValue;

                if (material == null)
                {
                    hasDefaultMaterial = true;
                    materialId = defaultMaterialBlock.dataId;
                }
                else if (!materialDataBlocks.ContainsKey (material))
                {
                    RexDataMaterialStandard materialBlock = new RexDataMaterialStandard (material);
                    materialId = materialBlock.dataId;
                    rexDataBlocks.Add (materialBlock);
                }

                RexDataMesh rexMeshData = new RexDataMesh (mesh.name, 0, 0,
                        new List<Vector3> (mesh.vertices),
                        new List<Vector3> (mesh.normals),
                        new List<Vector2> (mesh.uv),
                        new List<Color> (mesh.colors),
                        mesh.triangles,
                        materialId);

                rexDataBlocks.Add (rexMeshData);
            }

            if (hasDefaultMaterial)
            {
                rexDataBlocks.Add (defaultMaterialBlock);
            }

            RexFileData rexFileData = new RexFileData (rexDataBlocks);

            return rexFileData.GetBytes ();
        }

        public void ConvertFromRex (byte[] rexData, ConversionResultDelegate onConversionResult)
        {
            RexToGameObjectConverter converter = RexToGameObjectConverter.GenerateGameObjectsFromRex (rexData, onConversionResult, markMeshesNoLongerReadable);
            activeConverters.Add (converter);
        }

        public void CancelAllConversions()
        {
            foreach (var converter in activeConverters)
            {
                converter.CancelConversion();
            }
        }

        public GameObject CreateNewMesh()
        {
            return Instantiate (meshPrefab);
        }

        public GameObject CreateNewLineSet()
        {
            return Instantiate (lineSetPrefab);
        }

        public GameObject CreateNewPointSet()
        {
            return Instantiate (pointSetPrefab);
        }

        public GameObject CreateNewText()
        {
            return Instantiate (textPrefab);
        }

        public Material CreateNewMaterial (bool transparent)
        {
            if (transparent)
            {
                return Instantiate (meshMaterialTransparent);
            }
            else
            {
                return Instantiate (meshMaterialSolid);
            }
        }

        internal void SignalConverterFinished (RexToGameObjectConverter converter)
        {
            activeConverters.Remove (converter);
        }
    }
}

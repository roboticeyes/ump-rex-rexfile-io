/*
 * Copyright (c) 2019 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */

using RoboticEyes.Rex.RexFileReader;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LoadRexFileExample : MonoBehaviour
{
    [Space]
    [Header ("File must be inside the `Assets/StreamingAssets`.")]
    [SerializeField]
    [Tooltip ("File must be inside the `StreamingAssets` Folder. Filename must include `.rex` ending.")]
    private List<string> rexFileNames;
    [SerializeField]
    private MeshFilter[] meshesToWrite;

    List<byte[]> data;
    private LoadedObjects loaded;
    private List<GameObject> parentObjects;

    bool load = true;
    int loadedTimes = 0;
    float startTime = 0f;

    private void Start()
    {
        //Required for Materials that use _RexFadeAlpha as a global transparency value, e.g. SolidWithFadeTransparency.mat, otherwise they will be invisible.
        Shader.SetGlobalFloat ("_RexFadeAlpha", 1);

        data = new List<byte[]> ();
        parentObjects = new List<GameObject> ();

        foreach (var rexFileName in rexFileNames)
        {
            if(string.IsNullOrEmpty (rexFileName))
            {
                continue;
            }

            var dataPath = Path.Combine (Application.streamingAssetsPath, rexFileName);

            if (Application.platform == RuntimePlatform.Android)
            {
                var uwr = UnityWebRequest.Get (dataPath);
                uwr.SendWebRequest ();
                while (!uwr.isDone) { }
                data.Add (uwr.downloadHandler.data);
            } 
            else
            {
                data.Add (File.ReadAllBytes (dataPath));
            }
        }

#if !UNITY_EDITOR
        return;
#endif
        if (meshesToWrite != null && meshesToWrite.Length > 0)
        {
            File.WriteAllBytes ("out.rex", RexConverter.Instance.GenerateRexFile (meshesToWrite));
            Debug.Log ("Writing done.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown (KeyCode.Space))
        {
            loadedTimes = 0;
        }

        if (data != null && load && loadedTimes < 1)
        {
            load = false;
            if (loaded != null)
            {
                loaded.DestroyLoadedObjects ();
                loaded = null;

                foreach (var parentObject in parentObjects)
                {
                    Destroy (parentObject);
                }

                parentObjects.Clear ();
            }

            startTime = Time.realtimeSinceStartup;
            foreach (var item in data)
            {
                RexConverter.Instance.ConvertFromRex (item, (success, loadedObjects) =>
                {
                    if (!success)
                    {
                        Debug.Log ("ConvertFromRex failed.");
                        return;
                    }

                    parentObjects.Add (new GameObject ("RexContainer_" + parentObjects.Count));

                    loaded = loadedObjects;
                    loadedTimes++;

                    var parentObjectsIdx = parentObjects.Count - 1;

                    foreach (var meshObject in loadedObjects.Meshes)
                    {
                        meshObject.gameObject.SetActive (true);
                        meshObject.gameObject.transform.SetParent (parentObjects[parentObjectsIdx].transform);
                    }

                    foreach (var pointListObject in loadedObjects.PointSets)
                    {
                        pointListObject.gameObject.transform.SetParent (parentObjects[parentObjectsIdx].transform);
                    }

                    Debug.Log ("ConvertFromRex success after " + (Time.realtimeSinceStartup - startTime).ToString ("F4") + " s");
                    load = true;
                });
            }
        }
    }
}
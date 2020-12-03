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

public class LoadRexFileExample : MonoBehaviour
{
    [Space]
    [Header ("File must be inside the `Assets/StreamingAssets`.")]
    [SerializeField]
    [Tooltip ("File must be inside the `StreamingAssets` Folder. Filename must include `.rex` ending.")]
    private string rexFileName;

    [SerializeField]
    private MeshFilter[] meshesToWrite;

    float startTime = 0f;
    byte[] data;

    private LoadedObjects loaded;

    bool load = true;
    int loadedTimes = 0;

    private void Start()
    {
        //Required for Materials that use _RexFadeAlpha as a global transparency value, e.g. SolidWithFadeTransparency.mat, otherwise they will be invisible.
        Shader.SetGlobalFloat ("_RexFadeAlpha", 1);

        if (!string.IsNullOrEmpty (rexFileName))
        {
            data = File.ReadAllBytes (Application.streamingAssetsPath + "/" + rexFileName);
        }

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
                loaded.DestroyLoadedObjects();
                loaded = null;
            }

            startTime = Time.realtimeSinceStartup;
            RexConverter.Instance.ConvertFromRex (data, (success, loadedObjects) =>
            {
                if (!success)
                {
                    Debug.Log ("ConvertFromRex failed.");
                    return;
                }

                List<MeshFilter> meshFilters = new List<MeshFilter> ();

                foreach (var item in loadedObjects.Meshes)
                {
                    item.gameObject.SetActive (true);

                    meshFilters.Add (item.GetComponentInChildren<MeshFilter>());
                }

            });
        }
    }
}

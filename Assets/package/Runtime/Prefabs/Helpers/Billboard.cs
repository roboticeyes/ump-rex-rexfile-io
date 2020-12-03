/*
 * Copyright (c) 2019 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */


using UnityEngine;

namespace RoboticEyes.Rex.RexFileReader.Examples
{
    public class Billboard : MonoBehaviour
    {
        public Camera billboardCamera;

        private void Start()
        {
            if (billboardCamera == null)
            {
                billboardCamera = Camera.main;
            }
        }

        // Update is called once per frame
        void Update()
        {
            transform.rotation = Quaternion.LookRotation (transform.position - billboardCamera.transform.position);
        }
    }
}
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
    [RequireComponent (typeof (MeshPrefab))]
    public class AlignedWireframeDataGenerator : WireframeDataGenerator
    {
        [SerializeField]
        private float thresholdMin = 0.0005f;
        [SerializeField]
        private float thresholdMax = 0.9995f;
        [SerializeField]
        [Tooltip ("Wireframe will only be displayed if an edge is roughly (depending on threshold) aligned or perpendicular to this direction.")]
        private Vector3 alignment = Vector3.up;

        public Vector3 Alignment
        {
            set
            {
                alignment = Vector3.Normalize (value);
            }
            get
            {
                return alignment;
            }
        }

        private void Awake()
        {
            alignment = Vector3.Normalize (alignment);
        }

        protected override void SetWireframeData (int index, int[] triangles, Vector3[] vertices, Vector3[] data)
        {
            Vector3 v1 = vertices[triangles[index]];
            Vector3 v2 = vertices[triangles[index + 1]];
            Vector3 v3 = vertices[triangles[index + 2]];

            var b1 = Vector3.one;
            var b2 = Vector3.one;
            var b3 = Vector3.one;

            Vector3 e12 = Vector3.Normalize (v2 - v1);
            float e12dot = Mathf.Abs (Vector3.Dot (e12, alignment));
            if (e12dot < thresholdMin || e12dot > thresholdMax)
            {
                b1.x = 0;
                b2.x = 0;
            }

            Vector3 e23 = Vector3.Normalize (v3 - v2);
            float e23dot = Mathf.Abs (Vector3.Dot (e23, alignment));
            if (e23dot < thresholdMin || e23dot > thresholdMax)
            {
                b2.y = 0;
                b3.y = 0;
            }

            Vector3 e31 = Vector3.Normalize (v1 - v3);
            float e31dot = Mathf.Abs (Vector3.Dot (e31, alignment));
            if (e31dot < thresholdMin || e31dot > thresholdMax)
            {
                b1.z = 0;
                b3.z = 0;
            }

            data[triangles[index + 0]] = b1;
            data[triangles[index + 1]] = b2;
            data[triangles[index + 2]] = b3;
        }
    }
}

/*
 * Copyright (c) 2019 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */

using System.Collections.Generic;
using UnityEngine;

namespace RoboticEyes.Rex.RexFileReader.Examples
{
    [RequireComponent (typeof (MeshPrefab))]
    public class WireframeDataGenerator : MonoBehaviour
    {
        private void Start()
        {
            Generate();
        }

        public void Generate()
        {
            var meshPrefab = GetComponent<MeshPrefab>();
            var filter = meshPrefab.MeshFilter;

            var mesh = filter.sharedMesh;

            if (mesh == null)
            {
                return;
            }

            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
            Vector3[] uv1;

            if (vertices.Length != triangles.Length)
            {
                //Making Vertices unique
                int vertexCount = triangles.Length;

                var newVerts = new Vector3[vertexCount];
                var normals = mesh.normals;
                var newNormals = new Vector3[vertexCount];
                var uv0 = mesh.uv;
                bool hasUVs = uv0.Length > 0;
                Vector2[] newUv0 = { };
                if (hasUVs)
                {
                    newUv0 = new Vector2[vertexCount];
                }

                uv1 = new Vector3[vertexCount];

                for (int i = 0; i < triangles.Length; i++)
                {
                    newVerts[i] = vertices[triangles[i]];
                    newNormals[i] = normals[triangles[i]];

                    if (hasUVs)
                    {
                        newUv0[i] = uv0[triangles[i]];
                    }

                    triangles[i] = i;

                    if (i % 3 == 2)
                    {
                        //Only calc data after the whole triangle has been made unique
                        SetWireframeData (i - 2, triangles, newVerts, uv1);
                    }
                }
                mesh.vertices = newVerts;
                mesh.triangles = triangles;
                mesh.normals = newNormals;
                mesh.uv = newUv0;
            }
            else
            {
                uv1 = new Vector3[vertices.Length];

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    SetWireframeData (i, triangles, vertices, uv1);
                }
            }


            mesh.SetUVs (1, new List<Vector3> (uv1));
            filter.sharedMesh = mesh;
        }

        /// <summary>
        /// Calculates Wireframe data for triangle (index, index + 1, index + 2) and writes it into data
        /// </summary>
        /// <param name="firstTriangleIndex">First index of the triangle inside the triangles array</param>
        /// <param name="triangles">Array of triangle indices</param>
        /// <param name="vertices">Array of vertices</param>
        /// <param name="data">The array where the computed data is written into, data[i] is linked to vertices[i]</param>
        protected virtual void SetWireframeData (int firstTriangleIndex, int[] triangles, Vector3[] vertices, Vector3[] data)
        {
            data[triangles[firstTriangleIndex]] = new Vector3 (1, 0, 0);
            data[triangles[firstTriangleIndex + 1]] = new Vector3 (0, 1, 0);
            data[triangles[firstTriangleIndex + 2]] = new Vector3 (0, 0, 1);
        }
    }
}

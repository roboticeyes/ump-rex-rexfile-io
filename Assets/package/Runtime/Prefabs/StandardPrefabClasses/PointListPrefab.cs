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
using UnityEngine.Rendering;

namespace RoboticEyes.Rex.RexFileReader.Examples
{
    public class PointListPrefab : RexPointListObject
    {
        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private MeshRenderer meshRenderer;

        [SerializeField]
        float pointSize = 0.01f;

        public override bool SetPoints (List<Vector3> pointPositions, List<Color> pointColors)
        {
            int vertexCount = pointPositions.Count;
            bool hasColors = pointColors.Count == pointPositions.Count;

            Vector3[] vertices = new Vector3[vertexCount * 6];
            Color[] colors = new Color[vertexCount * 6];
            int[] indices = new int[vertexCount * 6];



            for (int i = 0; i < vertexCount; i++)
            {
                var vertex = pointPositions[i];

                Vector3 p1 = new Vector3 (vertex.x + pointSize, vertex.y, vertex.z + pointSize);
                Vector3 p2 = new Vector3 (vertex.x + pointSize, vertex.y, vertex.z - pointSize);
                Vector3 p3 = new Vector3 (vertex.x - pointSize, vertex.y, vertex.z + pointSize);
                Vector3 p4 = new Vector3 (vertex.x - pointSize, vertex.y, vertex.z - pointSize);

                int offset = i * 6;

                vertices[offset] = p1;
                vertices[offset + 1] = p2;
                vertices[offset + 2] = p3;
                vertices[offset + 3] = p2;
                vertices[offset + 4] = p3;
                vertices[offset + 5] = p4;

                var color = Color.white;
                if (hasColors)
                {
                    color = pointColors[i];
                }

                colors[offset] = color;
                colors[offset + 1] = color;
                colors[offset + 2] = color;
                colors[offset + 3] = color;
                colors[offset + 4] = color;
                colors[offset + 5] = color;

                for (int j = 0; j < 6; j++)
                {
                    indices[offset + j] = offset + j;
                    colors[offset + j] = color;
                }
            }

            Mesh pointMesh = new Mesh
            {
                indexFormat = IndexFormat.UInt32,
                vertices = vertices,
                colors = colors
            };

            pointMesh.SetIndices (indices, MeshTopology.Triangles, 0);
            pointMesh.UploadMeshData (true);

            meshFilter.sharedMesh = pointMesh;

            Bounds = new Bounds
            {
                center = meshFilter.sharedMesh.bounds.center,
                extents = meshFilter.sharedMesh.bounds.extents
            };

            return true;
        }

        public override void SetRendererEnabled (bool enabled)
        {
            meshRenderer.enabled = enabled;
        }

        public override void SetLayer (int layer)
        {
            meshRenderer.gameObject.layer = layer;
        }
    }
}

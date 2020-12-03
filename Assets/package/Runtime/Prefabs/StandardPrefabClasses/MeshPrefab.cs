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
    public class MeshPrefab : RexMeshObject
    {
        [SerializeField]
        private MeshFilter meshFilter;
        public MeshFilter MeshFilter => meshFilter;

        [SerializeField]
        private MeshRenderer meshRenderer;

        private void OnDestroy()
        {
            Destroy (meshRenderer.sharedMaterial?.mainTexture);
            Destroy (meshRenderer.sharedMaterial);
            Destroy (meshFilter.sharedMesh);
        }

        public override bool SetMeshData (Mesh mesh, Material material)
        {
            meshRenderer.sharedMaterial = material;
            meshFilter.sharedMesh = mesh;

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
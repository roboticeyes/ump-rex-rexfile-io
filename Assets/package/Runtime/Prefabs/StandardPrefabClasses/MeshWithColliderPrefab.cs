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
    public class MeshWithColliderPrefab : MeshPrefab
    {
        [SerializeField]
        private MeshCollider meshCollider;

        public override bool SetMeshData (Mesh mesh, Material material)
        {
            meshCollider.sharedMesh = mesh;
            return base.SetMeshData (mesh, material);
        }
    }
}
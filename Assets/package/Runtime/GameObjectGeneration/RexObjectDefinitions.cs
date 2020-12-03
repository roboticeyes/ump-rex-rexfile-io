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

namespace RoboticEyes.Rex.RexFileReader
{
    public abstract class RexFileObject : MonoBehaviour
    {
        public Bounds Bounds
        {
            get;
            protected set;
        } = new Bounds();

        public abstract void SetRendererEnabled (bool enabled);

        public void SetLayer (string layerName)
        {
            int layer = LayerMask.NameToLayer (layerName);

            if (layer == -1)
            {
                Debug.LogError ("Layer \"" + layerName + "\" is not defined.");
                return;
            }

            SetLayer (layer);
        }

        public abstract void SetLayer (int layer);
    }

    public abstract class RexTextObject : RexFileObject
    {
        public abstract bool SetText (string text, Color textColor);
    }

    public abstract class RexLineSetObject : RexFileObject
    {
        public abstract bool SetPositions (List<Vector3> pointPositions, Color lineColor);
    }

    public abstract class RexPointListObject : RexFileObject
    {
        public abstract bool SetPoints (List<Vector3> pointPositions, List<Color> pointColors);
    }

    public abstract class RexMeshObject : RexFileObject
    {
        public abstract bool SetMeshData (Mesh mesh, Material material);
    }

    public abstract class RexUnityPackageObject : RexFileObject
    {
        public abstract bool SetPackageData (byte[] unityPackageData);
    }
}

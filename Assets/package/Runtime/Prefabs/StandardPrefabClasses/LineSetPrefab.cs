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
    public class LineSetPrefab : RexLineSetObject
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        private Material initialMaterial;

        public override bool SetPositions (List<Vector3> pointPositions, Color lineColor)
        {
            lineRenderer.positionCount = pointPositions.Count;
            lineRenderer.SetPositions (pointPositions.ToArray());

            initialMaterial = lineRenderer.material;
            initialMaterial.color = lineColor;
            lineRenderer.sharedMaterial = initialMaterial;

            Bounds = new Bounds
            {
                center = lineRenderer.bounds.center,
                extents = lineRenderer.bounds.extents
            };

            return true;
        }

        private void OnDestroy()
        {
            Destroy (initialMaterial);
        }

        public override void SetRendererEnabled (bool enabled)
        {
            lineRenderer.enabled = enabled;
        }

        public override void SetLayer (int layer)
        {
            lineRenderer.gameObject.layer = layer;
        }
    }
}

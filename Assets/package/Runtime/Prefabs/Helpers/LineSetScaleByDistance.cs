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
    [RequireComponent (typeof (LineRenderer))]
    public class LineSetScaleByDistance : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        private AnimationCurve widthCurve;

        private Vector3[] linePositions;

        private float currentScale = 0f;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            linePositions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions (linePositions);
            widthCurve = new AnimationCurve();
            widthCurve.AddKey (0f, 1f);
            widthCurve.AddKey (1f, 1f);
            lineRenderer.widthCurve = widthCurve;

        }

        void Update ()
        {
            if (currentScale == transform.lossyScale.x)
            {
                return;
            }

            currentScale = transform.lossyScale.x;

            var key = new Keyframe (0f, Mathf.Max (0.05f, Mathf.Min (1.0f, currentScale)));
            widthCurve.MoveKey (0, key);
            key = new Keyframe (1f, Mathf.Max (0.05f, Mathf.Min (1.0f, currentScale)));
            widthCurve.MoveKey (1, key);
            lineRenderer.widthCurve = widthCurve;
        }
    }
}
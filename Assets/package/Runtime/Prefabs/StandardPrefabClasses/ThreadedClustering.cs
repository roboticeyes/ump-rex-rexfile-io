/*
 * Copyright (c) 2019 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace RoboticEyes.Rex.RexFileReader.Examples
{
    public class ThreadedClustering : ThreadedWorker
    {
        private readonly IEnumerable<ColoredPoint> pointPositions;
        private readonly int kVal;
        private readonly Vector3[] centroids;
        
        public List<ColoredPoint>[] clusteredPoints;

        public ThreadedClustering (IEnumerable<ColoredPoint> pointPositions, Vector3[] centroids, int kVal)
        {
            this.pointPositions = pointPositions;
            this.kVal = kVal;
            this.centroids = centroids;
        }

        public override void WorkerFunction()
        {
            try {
                clusteredPoints = new List<ColoredPoint>[kVal];
                var concurrentClusteredPoints = InitializeConcurrentClusterPointsList (kVal);

                Parallel.ForEach (pointPositions, pointPosition => {
                    concurrentClusteredPoints[GetIndexOfClosestCentroid (pointPosition.point, centroids)].Add (pointPosition);
                });
                
                for (int i = 0; i < kVal; i++)
                {
                    clusteredPoints[i] = concurrentClusteredPoints[i].ToList ();
                }
            }
            catch (Exception e)
            {
                IsError = true;
                ErrorMessage = e.Message;
            }
        }

        private static ConcurrentBag<ColoredPoint>[] InitializeConcurrentClusterPointsList (int kVal)
        {
            var concurrentClusteredPoints = new ConcurrentBag<ColoredPoint>[kVal];

            for (int i = 0; i < kVal; i++)
            {
                concurrentClusteredPoints[i] = new ConcurrentBag<ColoredPoint> ();
            }

            return concurrentClusteredPoints;
        }

        private static int GetIndexOfClosestCentroid (Vector3 pointPosition, Vector3[] centroids)
        {
            var minDist = float.PositiveInfinity;
            var minIdx = 0;

            for (int i = 0; i < centroids.Length; i++)
            {
                var dist = (centroids[i] - pointPosition).sqrMagnitude;

                if (dist < minDist)
                {
                    minDist = dist;
                    minIdx = i;
                }
            }

            return minIdx;
        }

        public struct ColoredPoint
        {
            public Vector3 point;
            public Color32 color;
        }
    }
}
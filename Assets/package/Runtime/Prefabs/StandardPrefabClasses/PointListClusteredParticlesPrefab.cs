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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoboticEyes.Rex.RexFileReader.Examples
{
    public class PointListClusteredParticlesPrefab : RexPointListObject
    {
        [SerializeField] private int clusterCount = 64;
        [SerializeField] private GameObject pointListParticlesPrefab;

        public override bool SetPoints (List<Vector3> pointPositions, List<Color> pointColors)
        {
            var lasPoints = CombinePointInfo(pointPositions, pointColors);
            var clusteredPointPositions = ClusterPoints(lasPoints, clusterCount);

            foreach (var clusteredPointPosition in clusteredPointPositions)
            {
                var child = Instantiate(pointListParticlesPrefab, transform);

                child.GetComponent<PointListParticlesPrefab>().SetPoints(
                    clusteredPointPosition.Select(ls => ls.point).ToList(), 
                    clusteredPointPosition.Select(ls => ls.color).ToList());
            }
            
            return true;
        }

        private static List<LasPoint> CombinePointInfo(List<Vector3> pointPositions, List<Color> pointColors)
        {
            var lasPoints = pointColors.Select((color, i) => new LasPoint {point = pointPositions[i], color = color}).ToList();
            return lasPoints;
        }

        private static List<List<LasPoint>> ClusterPoints(List<LasPoint> pointPositions, int kVal)
        {
            var clusteredPoints = InitializeClusterPointsList(kVal);
            var centroids = GetRandomStartingCentroids(pointPositions, kVal);

            foreach (var pointPosition in pointPositions)
            {
                clusteredPoints[GetIndexOfClosestCentroid(pointPosition.point, centroids)].Add(pointPosition);
            }

            return clusteredPoints;
        }

        private static List<List<LasPoint>> InitializeClusterPointsList(int kVal)
        {
            var clusteredPoints = new List<List<LasPoint>>();
            
            for (int i = 0; i < kVal; i++)
            {
                clusteredPoints.Add(new List<LasPoint>());
            }

            return clusteredPoints;
        }

        private static int GetIndexOfClosestCentroid(Vector3 pointPosition, List<Vector3> centroids)
        {
            var minDist = float.PositiveInfinity;
            var minIdx = 0;
            
            for (int i = 0; i < centroids.Count; i++)
            {
                var dist = Vector3.Distance(pointPosition, centroids[i]);

                if (dist < minDist)
                {
                    minDist = dist;
                    minIdx = i;
                }
            }

            return minIdx;
        }

        private static List<Vector3> GetRandomStartingCentroids(List<LasPoint> pointPositions, int kVal)
        {
            var min = new Vector3(
                pointPositions.Min(v => v.point.x),
                pointPositions.Min(v => v.point.y),
                pointPositions.Min(v => v.point.z));

            var max = new Vector3(
                pointPositions.Max(v => v.point.x),
                pointPositions.Max(v => v.point.y),
                pointPositions.Max(v => v.point.z));

            var centroids = new List<Vector3>();
            
            for (var i = 0; i < kVal; i++)
            {
                centroids.Add(GetRandomVectorWithinBounds(min, max));
            }
            
            return centroids;
        }

        private static Vector3 GetRandomVectorWithinBounds(Vector3 min, Vector3 max)
        {
            return new Vector3(
                    Random.Range(min.x, max.x),
                    Random.Range(min.y, max.y),
                    Random.Range(min.z, max.z));
        }

        public override void SetRendererEnabled (bool enabled)
        {
        }

        public override void SetLayer (int layer)
        {
        }

        struct LasPoint
        {
            public Vector3 point;
            public Color color;
        }
    }
}

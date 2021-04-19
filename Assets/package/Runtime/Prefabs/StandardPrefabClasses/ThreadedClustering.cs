using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RoboticEyes.Rex.RexFileReader;
using UnityEngine;
using Random = System.Random;

namespace RoboticEyes.Rex.RexFileReader.Examples
{
	public class ThreadedClustering : ThreadedWorker
	{
		readonly List<ColoredPoint> pointPositions;
		readonly int kVal;
		private readonly List<Vector3> centroids;

		public List<List<ColoredPoint>> clusteredPoints;


		public ThreadedClustering (List<ColoredPoint> pointPositions, List<Vector3> centroids, int kVal)
		{
			this.pointPositions = pointPositions;
			this.kVal = kVal;
			this.centroids = centroids;
		}

		public override void WorkerFunction()
		{
			try
			{
				var tempClusteredPoints = InitializeClusterPointsList (kVal);

				foreach (var pointPosition in pointPositions)
				{
					tempClusteredPoints[GetIndexOfClosestCentroid (pointPosition.point, centroids)].Add (pointPosition);
				}

				clusteredPoints = tempClusteredPoints;
			}
			catch (Exception e)
			{
				IsError = true;
				ErrorMessage = e.Message;
			}
		}

		private static List<List<ColoredPoint>> InitializeClusterPointsList (int kVal)
		{
			var clusteredPoints = new List<List<ColoredPoint>> ();

			for (int i = 0; i < kVal; i++)
			{
				clusteredPoints.Add (new List<ColoredPoint> ());
			}

			return clusteredPoints;
		}

		private static int GetIndexOfClosestCentroid (Vector3 pointPosition, List<Vector3> centroids)
		{
			var minDist = float.PositiveInfinity;
			var minIdx = 0;

			for (int i = 0; i < centroids.Count; i++)
			{
				var dist = Vector3.Distance (pointPosition, centroids[i]);

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
			public Color color;
		}
	}
}
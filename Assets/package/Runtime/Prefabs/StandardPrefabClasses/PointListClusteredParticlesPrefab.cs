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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using ColoredPoint = RoboticEyes.Rex.RexFileReader.Examples.ThreadedClustering.ColoredPoint;

namespace RoboticEyes.Rex.RexFileReader.Examples
{
	public class PointListClusteredParticlesPrefab : RexPointListObject
	{
		[SerializeField] private int clusterCount = 16;
		[SerializeField] private GameObject pointListParticlesPrefab;

		public override bool SetPoints (List<Vector3> pointPositions, List<Color> pointColors)
		{
			var startingCentroids = GetRandomStartingCentroids (pointPositions, clusterCount);
			var coloredPoints = CombinePointInfo (pointPositions, pointColors);

			StartClusteringCoroutine (coloredPoints, startingCentroids, (success, clusteredPointPositions) =>
				{
					if (!success) return;
					StartCoroutine (ParticleSystemSpawnerCoroutine (clusteredPointPositions));
				}
			);

			return true;
		}

		private IEnumerator ParticleSystemSpawnerCoroutine (List<List<ColoredPoint>> clusteredPointPositions)
		{
			foreach (var clusteredPointPosition in clusteredPointPositions)
			{
				var child = Instantiate (pointListParticlesPrefab, transform).GetComponent<PointListParticlesPrefab> ();

				child.SetPoints (
					clusteredPointPosition.Select (ls => ls.point).ToList (),
					clusteredPointPosition.Select (ls => ls.color).ToList ());

				yield return null;
			}
		}

		private void StartClusteringCoroutine (List<ColoredPoint> coloredPoints, List<Vector3> startingCentroids, ClusterResultDelegate resultDelegate)
		{
			StartCoroutine (ClusterPointsCoroutine (coloredPoints, startingCentroids, clusterCount, resultDelegate));
		}

		private delegate void ClusterResultDelegate (bool success, List<List<ColoredPoint>> loadedObjects);

		private IEnumerator ClusterPointsCoroutine (List<ColoredPoint> pointPositions, List<Vector3> startingCentroids, int kVal, ClusterResultDelegate result)
		{
			var threadedClustering = new ThreadedClustering (pointPositions, startingCentroids, kVal);
			threadedClustering.StartJob ();

			var waiter = new WaitForSeconds (0.05f);

			while (!threadedClustering.IsDone)
			{
				yield return waiter;
			}

			result (threadedClustering.clusteredPoints != null, threadedClustering.clusteredPoints);
		}


		private static List<ColoredPoint> CombinePointInfo (List<Vector3> pointPositions, List<Color> pointColors)
		{
			var lasPoints = pointColors.Select ((color, i) => new ColoredPoint {point = pointPositions[i], color = color}).ToList ();
			return lasPoints;
		}


		private static List<Vector3> GetRandomStartingCentroids (List<Vector3> pointPositions, int kVal)
		{
			var min = new Vector3 (
				pointPositions.Min (v => v.x),
				pointPositions.Min (v => v.y),
				pointPositions.Min (v => v.z));

			var max = new Vector3 (
				pointPositions.Max (v => v.x),
				pointPositions.Max (v => v.y),
				pointPositions.Max (v => v.z));

			var centroids = new List<Vector3> ();

			for (var i = 0; i < kVal; i++)
			{
				centroids.Add (GetRandomVectorWithinBounds (min, max));
			}

			return centroids;
		}

		private static Vector3 GetRandomVectorWithinBounds (Vector3 min, Vector3 max)
		{
			return new Vector3 (
				Random.Range (min.x, max.x),
				Random.Range (min.y, max.y),
				Random.Range (min.z, max.z));
		}

		public override void SetRendererEnabled (bool enabled)
		{
		}

		public override void SetLayer (int layer)
		{
		}
	}
}
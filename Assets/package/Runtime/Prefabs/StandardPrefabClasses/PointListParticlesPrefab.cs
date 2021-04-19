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
	public class PointListParticlesPrefab : RexPointListObject
	{
		[SerializeField] private bool pauseAfterInstantiation = true;
		[SerializeField] private float particleSize = 0.1f;
		[SerializeField] private ParticleSystem particlesSystem;
		private ParticleSystem.Particle[] particles;

		public override bool SetPoints (List<Vector3> pointPositions, List<Color> pointColors)
		{
			particles = new ParticleSystem.Particle[pointPositions.Count];

			for (int i = 0; i < pointPositions.Count; i++)
			{
				var particle = new ParticleSystem.Particle
				{
					position = pointPositions[i],
					remainingLifetime = 99999,
					startColor = pointColors[i],
					startSize = particleSize
				};
				particle.SetMeshIndex (0);

				particles[i] = particle;
			}

			particlesSystem.SetParticles (particles);

			if (pauseAfterInstantiation)
			{
				particlesSystem.Pause ();
			}

			return true;
		}

		public override void SetRendererEnabled (bool enabled)
		{
		}

		public override void SetLayer (int layer)
		{
		}
	}
}
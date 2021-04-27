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
using System.Threading.Tasks;
using UnityEngine;

namespace RoboticEyes.Rex.RexFileReader.Examples
{
    public class PointListParticlesPrefab : RexPointListObject
    {
        [SerializeField] 
        private bool pauseAfterInstantiation = true;
        [SerializeField] 
        private float particleSize = 0.1f;
        [SerializeField] 
        private ParticleSystem particlesSystem;
        
        private ParticleSystem.Particle[] particles;
        private float minDeltaForDensityUpdate = 0.05f;
        private float currentDensityFactor = 0;
        private Renderer objRenderer;
        private Renderer particleRenderer;

        //only useful if this prefab is used without clustering prefab, which it shouldn't be
        public override bool SetPoints (List<Vector3> pointPositions, List<Color> pointColors)
        {
            particles = new ParticleSystem.Particle[pointPositions.Count];
            var startSize = Vector3.one * particleSize;

            for (int i = 0; i < pointPositions.Count; i++)
            {
                particles[i].position = pointPositions[i];
                particles[i].startColor = pointColors[i];
                particles[i].startSize3D = startSize;
            }

            particlesSystem.SetParticles (particles);
            
            if (pauseAfterInstantiation)
            {
                particlesSystem.Pause ();
            }
            
            return true;
        }
        
        public void SetPoints (List<ThreadedClustering.ColoredPoint> coloredPoints)
        {
            particles = new ParticleSystem.Particle[coloredPoints.Count];
            var startSize = Vector3.one * particleSize;

            Parallel.For (0, coloredPoints.Count, i =>
            {
                var coloredPoint = coloredPoints[i];
                particles[i].position = coloredPoint.point;
                particles[i].startColor = coloredPoint.color;
                particles[i].startSize3D = startSize;
            });

            if (pauseAfterInstantiation)
            {
                particlesSystem.Pause ();
            }
            
            objRenderer = GetComponent<Renderer> ();
            particleRenderer = particlesSystem.GetComponent<Renderer> ();
        }

        public override void SetRendererEnabled (bool enabled)
        {
            particleRenderer.enabled = enabled;
        }

        public override void SetLayer (int layer)
        {
            gameObject.layer = layer;
        }

        public int GetParticleCount()
        {
            return particles.Length;
        }

        public bool IsVisibleByCamera()
        {
            return objRenderer.isVisible;
        }

        public void SetMinDeltaForDensityUpdate (float minDelta)
        {
            minDeltaForDensityUpdate = minDelta;
        }
        
        public void SetDensity (float densityFactor)
        {
            //prevent irrelevant updates
            if (Mathf.Abs (densityFactor - currentDensityFactor) < minDeltaForDensityUpdate)
            {
                return;
            }

            //skip reduction if there isn't any
            if (Mathf.Approximately (densityFactor, 1f))
            {
                particlesSystem.SetParticles (particles);
            }
            else
            {
                float reducedCount = Mathf.CeilToInt (particles.Length * densityFactor);
                var particleSubset = new ParticleSystem.Particle[(int)reducedCount];
                var dynamicParticleSize = Vector3.one * Mathf.Max ((1 - densityFactor) * 0.1f, particleSize);
                
                Parallel.For (0, (int)reducedCount, i =>
                {
                    particleSubset[i] = particles[(int)(i / reducedCount * particles.Length)];
                    particleSubset[i].startSize3D = dynamicParticleSize;
                });
                
                particlesSystem.SetParticles (particleSubset);
            }
            
            currentDensityFactor = densityFactor;
        }
    }
}
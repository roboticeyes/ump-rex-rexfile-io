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
    public class LoadedObjects
    {
        private List<RexMeshObject> meshes = new List<RexMeshObject>();
        public IReadOnlyCollection<RexMeshObject> Meshes
        {
            get
            {
                return meshes.AsReadOnly();
            }
        }

        private List<RexLineSetObject> lineSets = new List<RexLineSetObject>();
        public IReadOnlyCollection<RexLineSetObject> LineSets
        {
            get
            {
                return lineSets.AsReadOnly();
            }
        }

        private List<RexPointListObject> pointSets = new List<RexPointListObject>();
        public IReadOnlyCollection<RexPointListObject> PointSets
        {
            get
            {
                return pointSets.AsReadOnly();
            }
        }

        private List<RexTextObject> texts = new List<RexTextObject>();
        public IReadOnlyCollection<RexTextObject> Texts
        {
            get
            {
                return texts.AsReadOnly();
            }
        }

        private List<RexFileObject> allGameObjects = new List<RexFileObject>();
        public IReadOnlyCollection<RexFileObject> AllGameObjects
        {
            get
            {
                return allGameObjects.AsReadOnly();
            }
        }

        public Bounds Bounds
        {
            get;
            private set;
        }


        public void AddMeshObject (RexMeshObject mesh)
        {
            meshes.Add (mesh);
            allGameObjects.Add (mesh);
        }

        public void AddLineSetObject (RexLineSetObject lineSet)
        {
            lineSets.Add (lineSet);
            allGameObjects.Add (lineSet);
        }

        public void AddPointSetObject (RexPointListObject pointSet)
        {
            pointSets.Add (pointSet);
            allGameObjects.Add (pointSet);
        }

        public void AddTextObject (RexTextObject text)
        {
            texts.Add (text);
            allGameObjects.Add (text);
        }

        public void DestroyLoadedObjects()
        {
            foreach (var item in AllGameObjects)
            {
                Object.Destroy (item.gameObject);
            }

            meshes.Clear();
            lineSets.Clear();
            pointSets.Clear();
            texts.Clear();
            allGameObjects.Clear();
        }

        public void CalculateGeometryBounds()
        {
            Bounds geometryBounds = new Bounds();
            bool boundsInitialized = false;

            foreach (var rexObject in AllGameObjects)
            {
                if (!boundsInitialized)
                {
                    geometryBounds = rexObject.Bounds;
                    boundsInitialized = true;
                }
                else
                {
                    geometryBounds.Encapsulate (rexObject.Bounds);
                }
            }

            Bounds = geometryBounds;
        }
    }
}
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
using System.Text;
using UnityEngine;

namespace RoboticEyes.Rex.RexFileReader
{
    public class RexDataPointList : RexDataBlock
    {
        private const byte SizePosition = 4;
        private const byte SizeColors = 4;

        public List<Vector3> vertices;
        public List<Color> colors;

        public RexDataPointList (byte[] buffer, int offset) : base (buffer, ref offset)
        {
            uint nrOfVertices = BitConverter.ToUInt32 (buffer, offset);
            offset += SizePosition;

            uint nrOfColors = BitConverter.ToUInt32 (buffer, offset);
            offset += SizeColors;

            vertices = Utils.Vector3ListFromArray (buffer, offset, (int) nrOfVertices * 3 * sizeof (float));
            offset += (int) nrOfVertices * 3 * sizeof (float);

            colors = Utils.ColorListFromArray (buffer, offset, (int) nrOfColors * 3 * sizeof (float));
            offset += (int) nrOfColors * 3 * sizeof (float);

        }

        protected override byte[] GetBlockBytes ()
        {
            throw new NotImplementedException ();
        }
    }
}

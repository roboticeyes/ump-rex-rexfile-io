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
using UnityEngine;

namespace RoboticEyes.Rex.RexFileReader
{
    /**
     * Data block which allows to specify a connected line of given vertices
     */
    public class RexDataLineSet : RexDataBlock
    {
        private const byte SizeNrOfVertices = 4;
        private const byte SizeColor = 4;

        public Color color;
        public List<Vector3> vertices;

        public RexDataLineSet (byte[] buffer, int offset) : base (buffer, ref offset)
        {
            float red = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            float green = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            float blue = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            float alpha = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            color = new Color (red, green, blue, alpha);

            UInt32 nrOfVertices = BitConverter.ToUInt32 (buffer, offset);
            offset += SizeNrOfVertices;

            vertices = Utils.Vector3ListFromArray (buffer, offset, (int) nrOfVertices * 3 * sizeof (float));
        }

        protected override byte[] GetBlockBytes ()
        {
            throw new NotImplementedException ();
        }
    }
}

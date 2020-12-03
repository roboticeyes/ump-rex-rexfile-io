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
    public class RexDataText : RexDataBlock
    {
        private const byte SizeColor = 4;
        private const byte SizePosition = 4;
        private const byte SizeTextSize = 4;
        private const byte SizeSz = 2;

        public string text;
        public float textSize;
        public Vector3 position;
        public Color color;


        public RexDataText (byte[] buffer, int offset) : base (buffer, ref offset)
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

            float posX = BitConverter.ToSingle (buffer, offset);
            offset += SizePosition;

            float posY = BitConverter.ToSingle (buffer, offset);
            offset += SizePosition;

            float posZ = BitConverter.ToSingle (buffer, offset);
            offset += SizePosition;

            position = new Vector3 (posX, posY, posZ);

            textSize = BitConverter.ToSingle (buffer, offset);
            offset += SizeTextSize;

            UInt16 sz = BitConverter.ToUInt16 (buffer, offset);
            offset += SizeSz;

            text = Encoding.ASCII.GetString (buffer, offset, sz);
            offset += sz;

        }

        protected override byte[] GetBlockBytes ()
        {
            throw new NotImplementedException ();
        }
    }
}

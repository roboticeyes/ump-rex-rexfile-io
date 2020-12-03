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
    * Data block which allows to specify a 3D mesh
    */
    public class RexDataMaterialStandard : RexDataBlock
    {
        private const byte SizeColor = 4;
        private const byte SizeTextureID = 8;
        private const byte SizeNs = 4;
        private const byte SizeAlpha = 4;

        public float kaRed;
        public float kaGreen;
        public float kaBlue;
        public UInt64 kaTextureId = long.MaxValue;

        public float kdRed;
        public float kdGreen;
        public float kdBlue;
        public UInt64 kdTextureId = long.MaxValue;

        public float ksRed;
        public float ksGreen;
        public float ksBlue;
        public UInt64 ksTextureId = long.MaxValue;

        public float ns;
        public float alpha;

        public RexDataMaterialStandard (Material material) : this (material.color, material.color.a)
        {

        }

        public RexDataMaterialStandard (Color kd, float alpha)
        {
            type = RexDataBlockType.MeshMaterialStandard;
            kdRed = kd.r;
            kdBlue = kd.b;
            kdGreen = kd.g;
            this.alpha = alpha;
        }

        public RexDataMaterialStandard (byte[] buffer, int offset) : base (buffer, ref offset)
        {
            kaRed = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            kaGreen = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            kaBlue = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            kaTextureId = BitConverter.ToUInt64 (buffer, offset);
            offset += SizeTextureID;


            kdRed = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            kdGreen = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            kdBlue = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            kdTextureId = BitConverter.ToUInt64 (buffer, offset);
            offset += SizeTextureID;


            ksRed = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            ksGreen = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            ksBlue = BitConverter.ToSingle (buffer, offset);
            offset += SizeColor;

            ksTextureId = BitConverter.ToUInt64 (buffer, offset);
            offset += SizeTextureID;

            ns = BitConverter.ToSingle (buffer, offset);
            offset += SizeNs;

            alpha = BitConverter.ToSingle (buffer, offset);
            offset += SizeAlpha;
        }

        protected override byte[] GetBlockBytes ()
        {
            List<byte> data = new List<byte>();

            data.AddRange (BitConverter.GetBytes (kaRed) );
            data.AddRange (BitConverter.GetBytes (kaGreen) );
            data.AddRange (BitConverter.GetBytes (kaBlue) );
            data.AddRange (BitConverter.GetBytes (kaTextureId) );

            data.AddRange (BitConverter.GetBytes (kdRed) );
            data.AddRange (BitConverter.GetBytes (kdGreen) );
            data.AddRange (BitConverter.GetBytes (kdBlue) );
            data.AddRange (BitConverter.GetBytes (kdTextureId) );

            data.AddRange (BitConverter.GetBytes (ksRed) );
            data.AddRange (BitConverter.GetBytes (ksGreen) );
            data.AddRange (BitConverter.GetBytes (ksBlue) );
            data.AddRange (BitConverter.GetBytes (ksTextureId) );

            data.AddRange (BitConverter.GetBytes (ns) );
            data.AddRange (BitConverter.GetBytes (alpha) );

            return data.ToArray();
        }
    }
}

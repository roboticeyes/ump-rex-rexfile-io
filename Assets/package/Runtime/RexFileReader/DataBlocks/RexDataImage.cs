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

namespace RoboticEyes.Rex.RexFileReader
{
    /**
     * Data block which allows to store an arbitrary image which can also be used
     * by the DataMesh storing the texture element.
     */
    public class RexDataImage : RexDataBlock
    {
        private const byte SizeCompression = 4;

        public ImageCompression compression;
        public byte[] data;

        public RexDataImage (byte[] buffer, int offset) : base (buffer, ref offset)
        {
            compression = (ImageCompression) BitConverter.ToInt32 (buffer, offset);
            offset += SizeCompression;

            data = new byte[blockSize - SizeCompression];
            Buffer.BlockCopy (buffer, offset, data, 0, (int) blockSize - SizeCompression);
        }

        protected override byte[] GetBlockBytes ()
        {
            throw new NotImplementedException ();
        }
    }
}

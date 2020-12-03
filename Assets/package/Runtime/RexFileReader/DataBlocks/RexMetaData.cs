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
using System.Text;

namespace RoboticEyes.Rex.RexFileReader
{
    /**
     * This is the general header of the NDF file
     */
    public class RexMetaData
    {
        private const byte SizeMagic = 4;
        private const byte SizeVersion = 2;

        public int blockSize = 10;
        public string magic;
        public UInt16 version;
        public UInt32 crc32;

        public RexMetaData ()
        {
            magic = "REX1";
            version = 1;
        }

        public byte[] GetBytes ()
        {
            List<byte> result = new List<byte> ();
            result.AddRange (Encoding.ASCII.GetBytes (magic));
            result.AddRange (BitConverter.GetBytes (version));
            result.AddRange (BitConverter.GetBytes (crc32));
            return result.ToArray ();
        }

        public RexMetaData (byte[] buffer, int offset)
        {
            magic = Encoding.ASCII.GetString (buffer, offset, SizeMagic);
            offset += SizeMagic;

            version = buffer[offset];
            offset += SizeVersion;

            crc32 = BitConverter.ToUInt32 (buffer, offset);
        }
    };
}

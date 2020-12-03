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

namespace RoboticEyes.Rex.RexFileReader
{
    /**
     * Please do not change the order of this enum, since the integer values
     * are stored in the binary file
     */
    public enum RexDataBlockType
    {
        LineSet = 0,
        Text = 1,
        PointList = 2,
        Mesh = 3,
        Image = 4,
        MeshMaterialStandard = 5,
    }

    /** This is a list of the possible image compressions in the REX file
     */
    public enum ImageCompression
    {
        RAW24 = 0,      //!< RGB 1 byte per channel (not supported yet)
        JPEG = 1,       //!< encoded JPG data stream
        PNG = 2         //!< encoded PNG data stream
    };

    /**
     * Abstract class which specifies any kind of data block.
     * This needs to be derived by a special type
     */
    public abstract class RexDataBlock
    {
        private const byte SizeType = 2;
        private const byte SizeVersion = 2;
        private const byte SizeBlockSize = 4;
        private const byte SizeDataId = 8;

        public static int DataBlockHeaderSize = 16;
        public UInt32 blockSize;

        public UInt64 dataId;
        public UInt16 version;
        public RexDataBlockType type;

        private static ulong nextBlockId = 1;

        public RexDataBlock ()
        {
            if (dataId == 0)
            {
                dataId = nextBlockId;
                nextBlockId++;
            }
        }

        public byte[] ToRexBytes ()
        {
            byte[] blockBytes = GetBlockBytes ();
            byte[] result = new byte[DataBlockHeaderSize + blockBytes.Length];

            List<byte> headerBytes = new List<byte> ();
            headerBytes.AddRange (BitConverter.GetBytes ( (ushort) type) );
            headerBytes.AddRange (BitConverter.GetBytes ( (ushort) 1) ); //version
            headerBytes.AddRange (BitConverter.GetBytes (blockBytes.Length) );
            headerBytes.AddRange (BitConverter.GetBytes (dataId) );

            Buffer.BlockCopy (headerBytes.ToArray(), 0, result, 0, DataBlockHeaderSize);
            Buffer.BlockCopy (blockBytes, 0, result, DataBlockHeaderSize, blockBytes.Length);

            return result;
        }

        protected abstract byte[] GetBlockBytes ();

        public RexDataBlock (byte[] buffer, ref int offset)
        {
            type = (RexDataBlockType) BitConverter.ToUInt16 (buffer, offset);
            offset += SizeType;

            version = BitConverter.ToUInt16 (buffer, offset);
            offset += SizeVersion;

            blockSize = BitConverter.ToUInt32 (buffer, offset);
            offset += SizeBlockSize;

            dataId = BitConverter.ToUInt64 (buffer, offset);
            offset += SizeDataId;
        }
    }
}

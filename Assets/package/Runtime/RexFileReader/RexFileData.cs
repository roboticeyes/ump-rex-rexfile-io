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
    * Main data structure which contains all the information of a NDF file
    */
    public class RexFileData
    {
        private const int RexHeaderSize = 64;
        private const byte SizeNrOfDataBlocks = 2;
        private const byte SizeStartData = 2;
        private const byte SizeSizeDataBlocks = 8;

        public RexMetaData meta;
        public RexCoordinateSystem coordinateSystem;
        public List<RexDataBlock> dataBlocks;

        public RexFileData (List<RexDataBlock> dataBlocks)
        {
            meta = new RexMetaData ();
            coordinateSystem = new RexCoordinateSystem ();

            this.dataBlocks = dataBlocks;
        }

        public byte[] GetBytes ()
        {
            List<byte> result = new List<byte> ();

            List<byte> dataBlockBytes = new List<byte> ();

            ulong dataSize = 0;

            foreach (var block in dataBlocks)
            {
                byte[] currentBlockBytes = block.ToRexBytes ();
                dataSize += (ulong)currentBlockBytes.Length;
                dataBlockBytes.AddRange (currentBlockBytes);
            }

            byte[] coordinateSystemBytes = coordinateSystem.GetBytes ();

            result.AddRange (meta.GetBytes ());
            result.AddRange (BitConverter.GetBytes ((ushort) dataBlocks.Count));
            result.AddRange (BitConverter.GetBytes ((ushort) (RexHeaderSize + coordinateSystemBytes.Length)));
            result.AddRange (BitConverter.GetBytes (dataSize));

            while (result.Count < RexHeaderSize)
            {
                result.Add (0);
            }

            result.AddRange (coordinateSystemBytes);
            result.AddRange (dataBlockBytes);

            return result.ToArray ();
        }

        public RexFileData (byte[] buffer)
        {
            int offset = 0;
            if (buffer.Length < RexHeaderSize)
            {
                throw new FormatException ("REX file corrupt.");
            }

            dataBlocks = new List<RexDataBlock>();

            // read the first part of the File header block
            // magic, version, crc32
            meta = new RexMetaData (buffer, offset);
            offset += meta.blockSize;

            if (meta.magic != "REX1")
            {
                throw new FormatException ("REX file corrupt.");
            }

            // File header block second part
            UInt16 nrOfDataBlocks = BitConverter.ToUInt16 (buffer, offset);
            offset += SizeNrOfDataBlocks;

            UInt16 startData = BitConverter.ToUInt16 (buffer, offset);
            offset += SizeStartData;

            BitConverter.ToUInt64 (buffer, offset); // sizeDataBlocks, return value unused here
            offset += SizeSizeDataBlocks;

            offset = RexHeaderSize;
            coordinateSystem = new RexCoordinateSystem (buffer, offset);

            offset = (int) startData;

            for (UInt16 i = 0; i < nrOfDataBlocks; i++)
            {
                try
                {
                    RexDataBlock block;

                    switch ( (RexDataBlockType) buffer[offset])
                    {
                        case RexDataBlockType.LineSet:
                            block = new RexDataLineSet (buffer, offset);
                            break;
                        case RexDataBlockType.Text:
                            block = new RexDataText (buffer, offset);
                            break;

                        case RexDataBlockType.PointList:
                            block = new RexDataPointList (buffer, offset);
                            break;

                        case RexDataBlockType.Mesh:
                            block = new RexDataMesh (buffer, offset);
                            break;

                        case RexDataBlockType.Image:
                            block = new RexDataImage (buffer, offset);
                            break;

                        case RexDataBlockType.MeshMaterialStandard:
                            block = new RexDataMaterialStandard (buffer, offset);
                            break;

                        default:
                            throw new FormatException ("Unknown REX data block.");
                    }

                    dataBlocks.Add (block);
                    offset += (int) block.blockSize + RexDataBlock.DataBlockHeaderSize;
                }
                catch (Exception e)
                {

                    throw new FormatException ("Unable to read REX file:\n" + e.Message
                                               + "\n DataBlock " + i + "/" + nrOfDataBlocks
                                               + "\n at Byte number " + offset + "/" + buffer.Length);
                }
            }
        }
    };
}

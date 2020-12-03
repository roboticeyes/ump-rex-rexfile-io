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
using UnityEngine;

namespace RoboticEyes.Rex.RexFileReader
{
    public class RexCoordinateSystem
    {
        private const byte SizeSrid = 4;
        private const byte SizeCoordinate = 4;
        private const byte SizeSz = 2;

        public UInt32 srid;                         //!< spatial reference system identifier (unique value)
        public string authName;                     //!< defines the name for the used system (e.g. EPSG)
        public Vector3 globalOffset;      //!< this the global data offset in meter unit

        public RexCoordinateSystem ()
        {
            srid = 123456;
            authName = "EPSG";
            globalOffset = Vector3.zero;
        }

        public byte[] GetBytes ()
        {
            List<byte> result = new List<byte> ();
            result.AddRange (BitConverter.GetBytes (srid));
            result.AddRange (BitConverter.GetBytes ((ushort) authName.Length));
            result.AddRange (Encoding.ASCII.GetBytes (authName));
            result.AddRange (BitConverter.GetBytes (globalOffset.x));
            result.AddRange (BitConverter.GetBytes (globalOffset.y));
            result.AddRange (BitConverter.GetBytes (globalOffset.z));
            return result.ToArray ();
        }

        public RexCoordinateSystem (byte[] buffer, int offset)
        {
            srid = BitConverter.ToUInt32 (buffer, offset);
            offset += SizeSrid;

            UInt16 sz = BitConverter.ToUInt16 (buffer, offset);
            offset += SizeSz;

            authName = Encoding.ASCII.GetString (buffer, offset, sz);
            offset += sz;

            float offX = BitConverter.ToSingle (buffer, offset);
            offset += SizeCoordinate;

            float offY = BitConverter.ToSingle (buffer, offset);
            offset += SizeCoordinate;

            float offZ = BitConverter.ToSingle (buffer, offset);
            offset += SizeCoordinate;

            globalOffset = new Vector3 (offX, offY, offZ);
        }
    };
}

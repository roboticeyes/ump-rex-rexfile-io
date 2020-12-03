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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoboticEyes.Rex.RexFileReader
{
    public class Utils
    {
        // OpenREX defines positive Z pointing towards the camera, while unity has it pointing away
        private const float scaleZ = -1f;

        public static List<Vector2> Vector2ListFromArray (byte[] sourceArray, int offset, int length)
        {
            if (length % 2 != 0)
            {
                throw new InvalidOperationException ("Source array has invalid length.");
            }

            int numberOfVectors = length / 2 / sizeof (float);

            // We use a temporary Array, as this is way faster than a few thousand List.Add() calls
            Vector2[] resultArray = new Vector2[numberOfVectors];

            int index = offset;

            for (int i = 0; i < numberOfVectors; i++)
            {
                float x = BitConverter.ToSingle (sourceArray, index);
                index += sizeof (float);

                float y = BitConverter.ToSingle (sourceArray, index);
                index += sizeof (float);

                resultArray[i] = new Vector2 (x, y);
            }

            return new List<Vector2> (resultArray);
        }

        // after the Z axis has been inverted, the triangles are facing the wrong way, so we have to flip them
        public static int[] FlipTriangles (int[] triangles)
        {
            int tmp;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                tmp = triangles[i];
                triangles[i] = triangles[i + 1];
                triangles[i + 1] = tmp;
            }
            return triangles;
        }

        public static byte[] BytesFromVector3List (List<Vector3> vectors)
        {
            int numberOfBytes = vectors.Count * 3 * sizeof (float);
            byte[] result = new byte[numberOfBytes];

            int offset = 0;
            for (int i = 0; i < vectors.Count; i++)
            {
                Vector3 currentVector = vectors[i];
                Buffer.BlockCopy (BitConverter.GetBytes (currentVector.x), 0, result, offset, sizeof (float));
                offset += sizeof (float);
                Buffer.BlockCopy (BitConverter.GetBytes (currentVector.y), 0, result, offset, sizeof (float));
                offset += sizeof (float);
                Buffer.BlockCopy (BitConverter.GetBytes (scaleZ * currentVector.z), 0, result, offset, sizeof (float));
                offset += sizeof (float);
            }

            return result;
        }

        public static byte[] BytesFromVector2List (List<Vector2> vectors)
        {
            int numberOfBytes = vectors.Count * 2 * sizeof (float);
            byte[] result = new byte[numberOfBytes];

            int offset = 0;
            for (int i = 0; i < vectors.Count; i++)
            {
                Vector2 currentVector = vectors[i];
                Buffer.BlockCopy (BitConverter.GetBytes (currentVector.x), 0, result, offset, sizeof (float));
                offset += sizeof (float);
                Buffer.BlockCopy (BitConverter.GetBytes (currentVector.y), 0, result, offset, sizeof (float));
                offset += sizeof (float);
            }

            return result;
        }

        public static byte[] BytesFromColorList (List<Color> colors)
        {
            int numberOfBytes = colors.Count * 3 * sizeof (float);
            byte[] result = new byte[numberOfBytes];

            int offset = 0;
            for (int i = 0; i < colors.Count; i++)
            {
                Color currentColor = colors[i];
                Buffer.BlockCopy (BitConverter.GetBytes (currentColor.r), 0, result, offset, sizeof (float));
                offset += sizeof (float);
                Buffer.BlockCopy (BitConverter.GetBytes (currentColor.g), 0, result, offset, sizeof (float));
                offset += sizeof (float);
                Buffer.BlockCopy (BitConverter.GetBytes (currentColor.b), 0, result, offset, sizeof (float));
                offset += sizeof (float);
            }

            return result;
        }

        public static List<Vector3>
        Vector3ListFromArray (byte[] sourceArray, int offset, int length)
        {
            if (length % 3 != 0)
            {
                throw new InvalidOperationException ("Source array has invalid length.");
            }

            int numberOfVectors = length / 3 / sizeof (float);

            // We use a temporary Array, as this is way faster than a few thousand List.Add() calls
            Vector3[] resultArray = new Vector3[numberOfVectors];

            int index = offset;

            for (int i = 0; i < numberOfVectors; i++)
            {
                float x = BitConverter.ToSingle (sourceArray, index);
                index += sizeof (float);

                float y = BitConverter.ToSingle (sourceArray, index);
                index += sizeof (float);

                float z = BitConverter.ToSingle (sourceArray, index);
                index += sizeof (float);

                resultArray[i] = new Vector3 (x, y, z * scaleZ);
            }

            return new List<Vector3> (resultArray);
        }

        public static List<Color> ColorListFromArray (byte[] sourceArray, int offset, int length)
        {
            if (length % 3 != 0)
            {
                throw new InvalidOperationException ("Source array has invalid length.");
            }

            int numberOfVectors = length / 3 / sizeof (float);

            // We use a temporary Array, as this is way faster than a few thousand List.Add() calls
            Color[] resultArray = new Color[numberOfVectors];

            int index = offset;

            for (int i = 0; i < numberOfVectors; i++)
            {
                float r = BitConverter.ToSingle (sourceArray, index);
                index += sizeof (float);

                float g = BitConverter.ToSingle (sourceArray, index);
                index += sizeof (float);

                float b = BitConverter.ToSingle (sourceArray, index);
                index += sizeof (float);

                resultArray[i] = new Color (r, g, b);
            }

            return new List<Color> (resultArray);
        }
    }
}

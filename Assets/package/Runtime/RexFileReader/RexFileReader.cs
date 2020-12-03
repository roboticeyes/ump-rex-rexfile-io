/*
 * Copyright (c) 2019 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */

using System.IO;

namespace RoboticEyes.Rex.RexFileReader
{
    public class RexFileReader
    {
        public static RexFileData Read (string filename)
        {
            byte[] buffer = File.ReadAllBytes (filename);
            return Read (buffer);
        }

        public static RexFileData Read (byte[] buffer)
        {
            return new RexFileData (buffer);
        }
    }
}

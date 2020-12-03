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
    public class ThreadedRexLoader : ThreadedWorker
    {
        public RexFileData rexObject;
        private byte[] data;

        public ThreadedRexLoader (byte[] rexStreamData)
        {
            data = rexStreamData;
        }

        public override void WorkerFunction()
        {
            try
            {
                rexObject = RexFileReader.Read (data);
            }
            catch (Exception e)
            {
                IsError = true;
                ErrorMessage = e.Message;
            }
        }
    }
}

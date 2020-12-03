/*
 * Copyright (c) 2019 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */

using System.Collections;
using System.Threading;

#if UNITY_WSA_10_0 && !UNITY_EDITOR
    using System.Threading.Tasks;
#endif

namespace RoboticEyes.Rex.RexFileReader
{
    public abstract class ThreadedWorker
    {
        private volatile bool isDone = false;
        private volatile bool isError = false;
        private volatile string errorMessage = "";

        public virtual void StartJob()
        {
#if UNITY_WSA_10_0 && !UNITY_EDITOR
            Task.Run (() => Run());
#else
            ThreadPool.QueueUserWorkItem (new WaitCallback (Run));
#endif

        }

        public abstract void WorkerFunction();

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            protected set
            {
                errorMessage = value;
            }
        }

        public bool IsError
        {
            get
            {
                return isError;
            }

            protected set
            {
                isError = value;
            }
        }

        public bool IsDone
        {
            get
            {
                return isDone;
            }

            protected set
            {
                isDone = value;
            }
        }

#if UNITY_WSA_10_0 && !UNITY_EDITOR
        private void Run ()
#else
        private void Run (object stateInfo)
#endif
        {
            WorkerFunction();
            isDone = true;
        }
    }
}

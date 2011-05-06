using System;

namespace Builderdash
{
    public class JobStatusEventArgs : EventArgs
    {
        public JobStatusEventArgs(float progress, string message)
        {
            Progress = progress;
            Message = message;
        }

        public string Message { get; private set; }
        public float Progress { get; private set; }
    }
}
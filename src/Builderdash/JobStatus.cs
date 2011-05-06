using System;

namespace Builderdash
{
    public class JobStatus
    {
        private readonly object _guard = new object();

        private EventHandler<JobStatusEventArgs> _messageSent;

        public int NumberOfSubscribers = 0;

        public event EventHandler<JobStatusEventArgs> MessageSent
        {
            add
            {
                lock (_guard)
                {
                    NumberOfSubscribers++;
                    _messageSent += value;
                }
            }

            remove
            {
                lock (_guard)
                {
                    NumberOfSubscribers--;
                    _messageSent -= value;
                }
            }
        }

        private void InvokeMessageSent(JobStatusEventArgs e)
        {
            var handler = _messageSent;
            if (handler != null) handler(this, e);
        }

        public void SetProgess(float progress, string message)
        {
            InvokeMessageSent(new JobStatusEventArgs(progress, message));
        }
    }
}
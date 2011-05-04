using System;

namespace WcfShared
{
    public class Job
    {
        private object _guard = new object();

        public Guid Id { get; set; }

        private bool _complete;
        public bool Complete
        {
            get
            {
                bool result;
                lock (_guard)
                    result = _complete;
                return result;
            }
            set
            {
                lock (_guard)
                    _complete = value;
            }
        }

        private bool _started;
        public bool Started
        {
            get
            {
                bool result;
                lock (_guard)
                    result = _started;
                return result;
            }
            set
            {
                lock (_guard)
                    _started = value;
            }
        }

        private object _state;
        public object State
        {
            get { return _state; }
            set { _state = value; }
        }

        public object Result { get; set; }

        public JobStatus Status { get; set; }
    }
}
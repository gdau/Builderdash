using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WcfShared
{
    public class Jobber
    {
        private readonly Dictionary<Guid, Job> _jobs = new Dictionary<Guid, Job>();

        public Job Queue(Action<JobStatus> action)
        {
            var job = new Job { Id = Guid.NewGuid(), Started = false, Status = new JobStatus() };

            var guard = new object();

            ThreadPool.QueueUserWorkItem(o =>
                                             {
                                                 lock (guard)
                                                     job.Started = true;

                                                 action(job.Status);

                                                 lock (guard)
                                                 {
                                                     job.Status.SetProgess(1, "Done");
                                                     job.Complete = true;
                                                 }

                                             });

            _jobs[job.Id] = job;

            return job;
        }

        public int JobCount()
        {
            return _jobs.Count;
        }

        public int RunnigCount()
        {
            return _jobs.Values.Where(j => j.Started && !j.Complete).Count();
        }

        public int UncompleteCount()
        {
            return _jobs.Values.Where(j => !j.Complete).Count();
        }

        public int CompleteCount()
        {
            return _jobs.Values.Where(j => j.Complete).Count();
        }

        public Job GetJob(Guid jobId)
        {
            return _jobs[jobId];
        }
    }
}
using System;
using System.Diagnostics;
using Synoptic;

namespace Builderdash.Client
{
    public class CommandSet
    {
        [Command]
        public void CreateJob()
        {
            var proxy = new MyClient().GetProxy();
            var result = proxy.RunJob();
            
            Trace.WriteLine("Job result: " + result);
        }        
        
        [Command]
        public void GetJobStatus(Guid jobId)
        {
            var proxy = new MyClient().GetProxy();
            var result = proxy.GetJob(jobId).Result;
            
            Trace.WriteLine("Job result: " + result);
        }
    }
}

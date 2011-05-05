using System;
using System.Diagnostics;
using Builderdash.Configuration;
using Synoptic;

namespace Builderdash.Client
{
    public class CommandSet
    {
        [Command]
        public void CreateJob()
        {
            var proxy = GetProxy();
            var result = proxy.RunJob();
            
            Trace.WriteLine("Job result: " + result);
        }        
        
        [Command]
        public void GetJobStatus(Guid jobId)
        {
            var proxy = GetProxy();
            var result = proxy.GetJob(jobId).Result;
            
            Trace.WriteLine("Job result: " + result);
        }

        private IJobService GetProxy()
        {
            return new ClientProxyFactory(ClientConfiguration.Configuration).GetProxy();
        }
    }
}

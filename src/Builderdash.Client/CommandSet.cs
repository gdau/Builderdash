using System;
using System.Diagnostics;
using System.Linq;
using Builderdash.Configuration;
using Synoptic;

namespace Builderdash.Client
{
    public class CommandSet
    {
        private readonly static TraceSource Trace = new TraceSource("Builderdash");

        [Command]
        public void CreateJob()
        {
            var proxy = GetProxy();
            var result = proxy.RunJob();
            
            Trace.Information("Job result: " + result);
        }        
        
        [Command]
        public void GetJobStatus(Guid jobId)
        {
            var proxy = GetProxy();
            var result = proxy.GetJob(jobId).Result;

            Trace.Information("Job result: " + result);
        }

        private IJobService GetProxy()
        {
            return new JobServiceProxy(ClientConfiguration.Configuration.DefaultServer).GetService();
        }
    }
}

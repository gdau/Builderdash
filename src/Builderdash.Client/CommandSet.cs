using System;
using System.Diagnostics;
using Builderdash.Configuration;
using Synoptic;

namespace Builderdash.Client
{
    public class CommandSet
    {
        private readonly static TraceSource Trace = new TraceSource("Builderdash");

        [Command]
        public void CreateJob(string masterName)
        {

            var proxy = GetProxy(masterName);
            var result = proxy.RunJob();
            
            Trace.Information("Job result: " + result);
        }        
        
        [Command]
        public void GetJobStatus(Guid jobId, string masterName)
        {
            var proxy = GetProxy(masterName);
            var result = proxy.GetJob(jobId).Result;

            Trace.Information("Job result: " + result);
        }

        private IJobService GetProxy(string masterName)
        {
            var server = ClientConfiguration.Configuration.GetMasterServer(masterName);
            if(server == null)
                throw new ArgumentException("Master server invalid.", "masterName");
            
            return new JobServiceProxy(server).GetService();
        }
    }
}

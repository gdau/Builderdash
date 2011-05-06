using System;
using System.Diagnostics;
using System.ServiceModel;
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
            using(var proxy = GetProxy(masterName))
            {
                var result = proxy.Channel.RunJob();
                Trace.Information("Job result: " + result);
            }
        }        
        
        [Command]
        public void GetJobStatus(Guid jobId, string masterName)
        {
            var proxy = GetProxy(masterName);
//            var result = proxy.GetJob(jobId).Result;

//            Trace.Information("Job result: " + result);
        }

        private ServiceClientWrapper<IJobService> GetProxy(string masterName)
        {
            var server = ClientConfiguration.Configuration.GetMasterServer(masterName);
            if(server == null)
                throw new ArgumentException("Master server invalid.", "masterName");
            
            return new JobServiceProxy(server).GetService();
        }
    }
}

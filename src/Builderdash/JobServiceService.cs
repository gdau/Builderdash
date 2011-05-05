using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using WcfShared;

namespace Builderdash
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class JobServiceService : IJobService, IAuthenticationRequest
    {
        public string RequestCertificate(string commonName)
        {
            return File.ReadAllText(@"c:\\castore\cn2.pem");
        }

        public Jobber Jobber = new Jobber();

        public string Echo(string input)
        {
            return "echo: " + input;
        }

        public Guid RunJob()
        {
            Job job = Jobber.Queue(s =>
                                         {
                                             for (float i = 0; i < 10; i++)
                                             {
                                                 Console.WriteLine("Sending: " + i);
                                                 Console.WriteLine("Number of subs: " + s.NumberOfSubscribers);
                                                 
                                                 s.SetProgess(i / 10, "progress:" + i);
                                                 Thread.Sleep(1000);
                                             }
                                         });
            return job.Id;
        }

        public Job GetJob(Guid jobId)
        {
            return Jobber.GetJob(jobId);
        }

        private class JobEventSubscription
        {
            public EventHandler<JobStatusEventArgs> Event { get; set; }
            public object Callback { get; set; }
            public Guid SubId { get; set; }
        }

        private readonly Dictionary<Guid, List<JobEventSubscription>> _callbacks = new Dictionary<Guid, List<JobEventSubscription>>();

        public void SubscribeToStatus(Guid jobId)
        {
            Job job = Jobber.GetJob(jobId);

            IContextChannel contextChannel = OperationContext.Current.Channel;
            string sessionId = contextChannel.SessionId;
            var subscriptionId = Guid.NewGuid();

            IDataOutputCallback callback = OperationContext.Current.GetCallbackChannel<IDataOutputCallback>();

            EventHandler<JobStatusEventArgs> statusOnMessageSent = (s, e) =>
                                                                       {
                                                                           try
                                                                           {
                                                                               callback.SendDataPacket(e.Message);
                                                                           }
                                                                           catch (Exception e2)
                                                                           {
                                                                               Console.WriteLine("Client disconnected.." + e2.Message);
                                                                               UnsubscribeToStatus(jobId, subscriptionId);
                                                                           }
                                                                       };

            job.Status.MessageSent += statusOnMessageSent;

            if (!_callbacks.ContainsKey(jobId))
                _callbacks[jobId] = new List<JobEventSubscription>();

            _callbacks[jobId].Add(new JobEventSubscription { Event = statusOnMessageSent, Callback = callback, SubId = subscriptionId});

        }

        private void UnsubscribeToStatus(Guid jobId, Guid subId)
        {
            Job job = Jobber.GetJob(jobId);

            job.Status.MessageSent -= _callbacks[jobId].FirstOrDefault(c => c.SubId == subId).Event;
        }

        public void UnsubscribeToStatus(Guid jobId)
        {
            IDataOutputCallback callback = OperationContext.Current.GetCallbackChannel<IDataOutputCallback>();
            Job job = Jobber.GetJob(jobId);

            job.Status.MessageSent -= _callbacks[jobId].FirstOrDefault(c => c.Callback == callback).Event;
        }
    }
}
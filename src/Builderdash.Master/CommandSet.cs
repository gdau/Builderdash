﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Builderdash.Configuration;
using Builderdash.Master.Configuration;
using Synoptic;
using Synoptic.Service;
using WcfShared;

namespace Builderdash.Master
{
    public class CommandSet
    {
        private readonly ComboDaemon _comboDaemon;

        public CommandSet()
        {


//            var skinnyUdpServer = new UdpDaemon(msg =>
//            {
//                Trace.WriteLine("Got udp message..");
//
//                string SourceDir =
//                    @"d:\projects\scratch\src\autoup\AutoUpdate\bin\Debug";
//                string DestinationDir = @"c:\au";
//
//                new ServiceUpdater(SourceDir, DestinationDir, "au")
//                    .InitializeUpdate();
//            },
//                                                cfg =>
//                                                {
//                                                    cfg.EndPoint = new IPEndPoint(IPAddress.Any, 6666);
//                                                });


            _comboDaemon = new ComboDaemon(new MasterServer(
                MasterConfiguration.Configuration.Mode, 
                MasterConfiguration.Configuration.Server.Address, 
                MasterConfiguration.Configuration.Server.Port));
        }

        [Command]
        public void Service(bool start, bool install, bool uninstall)
        {
            var windowsService = new WindowsService(_comboDaemon, 
                new WindowsServiceConfiguration("au")
                    {
                        CommandLineArguments = "service --start"
                    });

            if (install) windowsService.Install();
            if (uninstall) windowsService.Uninstall();
            if (start) ServiceBase.Run(windowsService);
        }

        [Command]
        public void RunAsConsole()
        {
            var resetEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                resetEvent.Set();
            };
            _comboDaemon.Start();
            resetEvent.WaitOne();
            _comboDaemon.Stop();
        }

        [Command]
        public void Update()
        {

            string SourceDir = @"d:\projects\scratch\src\autoup\AutoUpdate\bin\Debug";
            string DestinationDir = @"c:\au";

            new ServiceUpdater(SourceDir, DestinationDir, "au")
                .Update();
        }

        [Command]
        public void GetVersion()
        {
            //            ThreadPool.SetMinThreads(100, 100);

            Jobber jobber = new Jobber();

            //            for (int i = 0; i < 1000; i++)
            Job job = jobber.Queue(s =>
            {
                for (float j = 0; j < 10; j++)
                {
                    s.SetProgess(j / 10, "s" + j.ToString());

                    Thread.Sleep(100);
                }
            });

            job.Status.MessageSent += (s, e) =>
            {
                Console.WriteLine("Status: {0} {1}", e.Progress, e.Message);
            };

            while (jobber.UncompleteCount() != 0)
            {
                //                Console.WriteLine("count: {0}/{1} - {2}", jobber.RunnigCount(), jobber.JobCount(), jobber.CompleteCount());
                Thread.Sleep(100);
            }
        }
    }
}
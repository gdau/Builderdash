using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Builderdash.Configuration;
using Builderdash.X509;
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


            _comboDaemon = new ComboDaemon(new MasterServer(MasterConfiguration.Configuration));
        }

        [Command]
        public void Service(bool start, bool install, bool uninstall)
        {
            var windowsService = new WindowsService("bdmaster", _comboDaemon,
                                                    c =>
                                                        {
                                                            c.Description = "Builderdash Master Server";
                                                            c.DisplayName = "Builderdash Master";
                                                            c.CommandLineArguments = "service --start";
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

        [Command]
        public void GenerateCa([CommandParameter(DefaultValue = "root")]string commonName)
        {
            string ca = new CertificateAuthority().GenerateCa("CN=" + commonName);
            Console.WriteLine(ca);
        }

        [Command]
        public void GenerateCert(
            [CommandParameter(DefaultValue = "client")]string commonName,
            [CommandParameter(DefaultValue = "root.crt")]string caPemFile)
        {
            string cert = new CertificateAuthority().GenerateCert("CN=" + commonName, File.ReadAllText(caPemFile));
            Console.WriteLine(cert);
        }
    }
}

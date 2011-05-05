using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using Builderdash.Configuration;
using Synoptic;

namespace Builderdash.Master
{
    public class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.Listeners.Add(new TextWriterTraceListener(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "au.log")));
            Trace.AutoFlush = true;

            try
            {
                Console.WriteLine(MasterConfiguration.Configuration.Mode);
                new CommandRunner().Run(args);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }
    }
}

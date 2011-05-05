using System;
using System.Diagnostics;
using Builderdash.Configuration;
using Synoptic;

namespace Builderdash.Master
{
    public class Program
    {
        private readonly static TraceSource Trace = new TraceSource("Builderdash");

        static void Main(string[] args)
        {
            try
            {
                Console.Title = "bd.master.server";
                new CommandRunner().Run(args);
            }
            catch (Exception e)
            {
                Trace.Error(e.ToString());
            }
        }
    }
}

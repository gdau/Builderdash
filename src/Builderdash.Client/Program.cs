using System;
using System.Diagnostics;
using Synoptic;

namespace Builderdash.Client
{
    class Program
    {
        private readonly static TraceSource Trace = new TraceSource("Builderdash");
        
        static void Main(string[] args)
        {
            try
            {
                new CommandRunner().Run(args);
            }
            catch (Exception e)
            {
                Trace.Error(e.ToString());
            }
        }
    }
}

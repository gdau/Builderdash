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
                Console.WriteLine(@"
 _           _ _     _              _           _
| |         (_) |   | |            | |         | |
| |__  _   _ _| | __| | ___ _ __ __| | __ _ ___| |__
| '_ \| | | | | |/ _` |/ _ \ '__/ _` |/ _` / __| '_ \
| |_) | |_| | | | (_| |  __/ | | (_| | (_| \__ \ | | |
|_.__/ \__,_|_|_|\__,_|\___|_|  \__,_|\__,_|___/_| |_|");

                new CommandRunner().Run(args);
            }
            catch (Exception e)
            {
                Trace.Error(e.ToString());
            }
        }
    }
}

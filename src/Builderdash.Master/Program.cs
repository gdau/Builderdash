using System;
using System.Diagnostics;
using Builderdash.Configuration;
using Synoptic;

namespace Builderdash.Master
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "bd.master.server";
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

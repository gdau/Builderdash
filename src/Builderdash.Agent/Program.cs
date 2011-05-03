using Synoptic;

namespace Builderdash.Agent
{
    public class Program
    {
        static int Main(string[] args)
        {
            new CommandRunner().Run(args);

            return 0;
        }
    }
}

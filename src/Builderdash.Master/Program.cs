using Synoptic;

namespace Builderdash.Master
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

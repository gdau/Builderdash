using System.Configuration;
using Builderdash.Configuration;

namespace Builderdash.Master.Configuration
{
    public class MasterConfiguration : ConfigurationSection
    {
        private static readonly MasterConfiguration ConfigSection
            = ConfigurationManager.GetSection("master") as MasterConfiguration;

        [ConfigurationProperty("listen")]
        public ServerConfigurationElement Server
        {
            get
            {
                return (ServerConfigurationElement)this["listen"];
            }
            set
            {
                this["listen"] = value;
            }
        }

        public static MasterConfiguration Configuration
        {
            get
            {
                return ConfigSection;
            }
        }
    }
}
using System;
using System.Configuration;
using System.Linq;

namespace Builderdash.Configuration
{
    public class MasterConfiguration : ConfigurationSection
    {
        private static readonly MasterConfiguration ConfigSection = 
            ConfigurationManager.GetSection("master") as MasterConfiguration;
        
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            return true;
        }

        [ConfigurationProperty("", IsDefaultCollection = true, IsKey = false)]
        public MasterServerConfigurationCollection MasterServers
        {
            get { return (MasterServerConfigurationCollection)base[""]; }
            set { base[""] = value; }
        }

        public static MasterConfiguration Configuration
        {
            get
            {
                return ConfigSection;
            }
        }

        public ServerConfiguration GetMasterServer(string name)
        {
            return MasterServers.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
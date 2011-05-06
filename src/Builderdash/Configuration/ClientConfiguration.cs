using System;
using System.Configuration;
using System.Linq;

namespace Builderdash.Configuration
{
    public class ClientConfiguration : ConfigurationSection
    {
        private static readonly ClientConfiguration ConfigSection
            = ConfigurationManager.GetSection("client") as ClientConfiguration;

        [ConfigurationProperty("", IsDefaultCollection = true, IsKey = false)]
        public ClientServerConfigurationCollection MasterServers
        {
            get { return (ClientServerConfigurationCollection)base[""]; }
            set { base[""] = value; }
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            return true;
        }

        public static ClientConfiguration Configuration
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
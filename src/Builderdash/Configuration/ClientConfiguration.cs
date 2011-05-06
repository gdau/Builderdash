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
        public ServerConfigurationCollection MasterServers
        {
            get { return (ServerConfigurationCollection)base[""]; }
            set { base[""] = value; }  
        }
        
        [ConfigurationProperty("masterName")]
        public string MasterName
        {
            get
            {
                return (string)this["masterName"];
            }
            set
            {
                this["masterName"] = value;
            }
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

        public ServerConfiguration DefaultServer
        {
            get
            {
                foreach(ServerConfiguration server in MasterServers)
                {
                    if (server.Name.Equals(MasterName))
                        return server;
                }

                return MasterServers.First();
            }
        }
    }
}
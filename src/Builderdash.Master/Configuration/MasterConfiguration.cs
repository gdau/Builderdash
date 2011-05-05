using System;
using System.Configuration;
using Builderdash.Configuration;

namespace Builderdash.Master.Configuration
{
    public enum ServerMode
    {
        Secure,
        Open
    }

    public class MasterConfiguration : ConfigurationSection
    {
        private static readonly MasterConfiguration ConfigSection
            = ConfigurationManager.GetSection("master") as MasterConfiguration;

        public MasterConfiguration()
        {
            Mode = ServerMode.Secure;
        }

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

        public ServerMode Mode
        {
            get; set;
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            if(name.Equals("mode"))
            {
                Mode = (ServerMode)Enum.Parse(typeof(ServerMode), value, true);
                return true;
            }
            
            return false;
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
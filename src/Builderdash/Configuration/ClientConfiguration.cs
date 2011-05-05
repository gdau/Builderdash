using System;
using System.Configuration;

namespace Builderdash.Configuration
{
    public class ClientConfiguration : ConfigurationSection
    {
        private static readonly ClientConfiguration ConfigSection
            = ConfigurationManager.GetSection("master") as ClientConfiguration;

        public ClientConfiguration()
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

        [ConfigurationProperty("certificatePemFile")]
        public string CertificatePemFile
        {
            get
            {
                return (string)this["certificatePemFile"];
            }
            set
            {
                this["certificatePemFile"] = value;
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

        public static ClientConfiguration Configuration
        {
            get
            {
                return ConfigSection;
            }
        }
    }
}
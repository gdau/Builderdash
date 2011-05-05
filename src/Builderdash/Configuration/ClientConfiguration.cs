using System;
using System.Configuration;

namespace Builderdash.Configuration
{
    public class ClientConfiguration : ConfigurationSection
    {
        private static readonly ClientConfiguration ConfigSection
            = ConfigurationManager.GetSection("client") as ClientConfiguration;

        public ClientConfiguration()
        {
            Mode = ServerMode.Secure;
        }

        [ConfigurationProperty("master")]
        public ServerConfigurationElement MasterServer
        {
            get
            {
                return (ServerConfigurationElement)this["master"];
            }
            set
            {
                this["master"] = value;
            }
        }

        [ConfigurationProperty("certificatePemFile", DefaultValue = "cert.crt")]
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
            
            return true;
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
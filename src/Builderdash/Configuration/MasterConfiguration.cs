using System;
using System.Configuration;

namespace Builderdash.Configuration
{
    public class MasterConfiguration : ConfigurationSection
    {
        private static readonly MasterConfiguration ConfigSection = 
            ConfigurationManager.GetSection("master") as MasterConfiguration;
        
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

        public static MasterConfiguration Configuration
        {
            get
            {
                return ConfigSection;
            }
        }
    }
}
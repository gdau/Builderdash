using System;
using System.Configuration;

namespace Builderdash.Configuration
{
    public class MasterConfiguration : ConfigurationSection
    {
        private static readonly MasterConfiguration ConfigSection = 
            ConfigurationManager.GetSection("master") as MasterConfiguration;
        
        [ConfigurationProperty("listen")]
        public ServerConfiguration Server
        {
            get
            {
                return (ServerConfiguration)this["listen"];
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

        public static MasterConfiguration Configuration
        {
            get
            {
                return ConfigSection;
            }
        }
    }
}
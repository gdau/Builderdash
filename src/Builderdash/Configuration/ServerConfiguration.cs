using System;
using System.Configuration;

namespace Builderdash.Configuration
{
    public class ServerConfiguration : ConfigurationElement
    {
        public ServerConfiguration()
        {
            Mode = ServerMode.Secure;
        }
        
        [ConfigurationProperty("address", IsRequired = true)]
        public string Address
        {
            get
            {
                return Convert.ToString(this["address"]);
            }
            set
            {
                this["address"] = value;
            }
        }        

        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }
            set
            {
                this["port"] = value;
            }
        }

        [ConfigurationProperty("commonName")]
        public string CommonName
        {
            get
            {
                return (string)this["commonName"];
            }
            set
            {
                this["commonName"] = value;
            }
        }

        [ConfigurationProperty("name")]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        public ServerMode Mode
        {
            get;
            set;
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

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            if (name.Equals("mode"))
            {
                Mode = (ServerMode)Enum.Parse(typeof(ServerMode), value, true);
                return true;
            }

            return true;
        }
    }
}
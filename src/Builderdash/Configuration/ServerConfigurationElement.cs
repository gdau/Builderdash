using System;
using System.Configuration;

namespace Builderdash.Configuration
{
    public class ServerConfigurationElement : ConfigurationElement
    {
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
    }
}
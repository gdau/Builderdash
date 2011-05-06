using System;
using System.Collections.Generic;
using System.Configuration;

namespace Builderdash.Configuration
{
    [ConfigurationCollection(typeof(ServerConfiguration), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class ServerConfigurationCollection : ConfigurationElementCollection, IEnumerable<ServerConfiguration>
    {
        private const string PropertyName = "master";

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMapAlternate; }
        }  

        protected override string ElementName
        {
            get
            {
                return PropertyName;
            }
        }

        public new IEnumerator<ServerConfiguration> GetEnumerator()
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                yield return BaseGet(i) as ServerConfiguration;
            }
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServerConfiguration)element).Name;
        }  

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(PropertyName);
        }  

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerConfiguration();
        }
    }
}
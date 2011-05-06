namespace Builderdash.Configuration
{
    public class ClientServerConfigurationCollection : ServerConfigurationCollectionBase
    {
        protected override string PropertyName
        {
            get { return "master"; }
        }
    }
}
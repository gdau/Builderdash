namespace Builderdash.Configuration
{
    public class MasterServerConfigurationCollection : ServerConfigurationCollectionBase
    {
        protected override string PropertyName
        {
            get { return "listen"; }
        }
    }
}
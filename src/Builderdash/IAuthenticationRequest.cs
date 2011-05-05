using System.ServiceModel;

namespace Builderdash
{
    [ServiceContract]
    public interface IAuthenticationRequest
    {
        [OperationContract]
        string RequestCertificate(string commonName);
    }
}
using System;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Builderdash.Configuration;
using Synoptic;

namespace Builderdash
{
    public class JobServiceProxy
    {
        private readonly static TraceSource Trace = new TraceSource("Builderdash");

        private readonly ServerMode _serverMode;
        private readonly Uri _uri;
        private readonly string _certificatePemFile;
        private readonly string _masterCommonName;
        private X509Certificate2 _caCertificate;

        public JobServiceProxy(ServerConfiguration configuration)
        {
            // TODO:config.
            _caCertificate = new X509Certificate2().LoadFromPemFile("ca.crt");

            _serverMode = configuration.Mode;
            _uri = new UriBuilder("net.tcp", configuration.Address, configuration.Port, "master").Uri;
            _masterCommonName = configuration.CommonName;
            
            _certificatePemFile = configuration.CertificatePemFile;
        }

        public ServiceClientWrapper<IJobService> GetService()
        {
            NetTcpBinding binding;
            EndpointAddress remoteAddress;
            X509Certificate2 clientCertificate = null;
            X509CertificateValidator certificateValidator = null;

            if (_serverMode == ServerMode.Secure)
            {
                binding = GetSecureBinding();
                remoteAddress = new EndpointAddress(_uri,
                                           EndpointIdentity.CreateDnsIdentity(_masterCommonName),
                                           (AddressHeaderCollection)null);
                
                clientCertificate = LoadFromFile();
                certificateValidator = new ServerX509CertificateValidator(_caCertificate);
            }
            else
            {
                binding = GetBinding();
                remoteAddress = new EndpointAddress(_uri);
            }

            return new ServiceClientWrapper<IJobService>(binding, 
                    remoteAddress, 
                    new CallbackImpl(), 
                    certificateValidator, 
                    clientCertificate);
        }

        private X509Certificate2 LoadFromFile()
        {
            Trace.Information("Loading certificate from {0}", _certificatePemFile);

            if (!File.Exists(_certificatePemFile))
            {
                Trace.Critical("Unable to load server certificate from '{0}'.", _certificatePemFile);
                throw new FileNotFoundException("The server certificate could not be found.", _certificatePemFile);
            }
            
            return new X509Certificate2().LoadFromPemFile(_certificatePemFile);
        }

        private NetTcpBinding GetSecureBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);

            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            return binding;
        }

        private NetTcpBinding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Builderdash.Configuration;
using Synoptic;
using X509Library;

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

            if (Path.IsPathRooted(configuration.CertificatePemFile))
                _certificatePemFile = configuration.CertificatePemFile;
            else
            {
                _certificatePemFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                   configuration.CertificatePemFile);
            }
        }

        public IJobService GetService()
        {
            NetTcpBinding binding;
            EndpointAddress addr;

            if (_serverMode == ServerMode.Secure)
            {
                binding = GetSecureBinding();
                addr = new EndpointAddress(_uri,
                                           EndpointIdentity.CreateDnsIdentity(_masterCommonName),
                                           (AddressHeaderCollection)null);
            }
            else
            {
                binding = GetBinding();
                addr = new EndpointAddress(_uri);
            }

            ChannelFactory<IJobService> factory = new DuplexChannelFactory<IJobService>(new CallbackImpl(), binding, addr);

            if (_serverMode == ServerMode.Secure)
            {
                Trace.Information("Loading certificate from {0}", _certificatePemFile);

                if (!File.Exists(_certificatePemFile))
                {
                    Trace.Critical("Unable to load server certificate from '{0}'.", _certificatePemFile);
                    throw new FileNotFoundException("The server certificate could not be found.", _certificatePemFile);
                }

                factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode =
                    X509CertificateValidationMode.Custom;
                
                factory.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator =
                    new ServerX509CertificateValidator(_caCertificate);

                factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

                X509Certificate2 cert = new X509Certificate2();
                cert.LoadFromPemFile(_certificatePemFile);

                factory.Credentials.ClientCertificate.Certificate = cert;
            }

            return factory.CreateChannel();
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
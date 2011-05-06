using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using Builderdash.Configuration;
using Synoptic;
using Synoptic.Service;
using X509Library;

namespace Builderdash
{
    public class MasterServer : IDaemon
    {
        private readonly static TraceSource Trace = new TraceSource("Builderdash");

        private readonly ServerMode _serverMode;
        private readonly Uri _uri;
        private readonly string _certificatePemFile;
        private ServiceHost _serviceHost;

        public MasterServer(ServerConfiguration configuration)
        {
            _serverMode = configuration.Mode;
            _uri = new UriBuilder("net.tcp", configuration.Address, configuration.Port).Uri;
            
            if (Path.IsPathRooted(configuration.CertificatePemFile))
                _certificatePemFile = configuration.CertificatePemFile;
            else
            {
                _certificatePemFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                   configuration.CertificatePemFile);
            }
        }

        public void Start()
        {
            _serviceHost = GetServiceHost(_serverMode);
            _serviceHost.Open();

            Trace.Information("Accepting requests in {0} mode on {1}", _serverMode.ToString().ToLower(), _uri);
        }

        public void Stop()
        {
            if(_serviceHost != null && 
               _serviceHost.State == CommunicationState.Opened)
                _serviceHost.Close();
            
            Trace.Information("Stopped");
        }

        private ServiceHost GetServiceHost(ServerMode serverMode)
        {
            ServiceHost serviceHost = new ServiceHost(new JobServiceService(), _uri);

            NetTcpBinding binding;
            if(serverMode == ServerMode.Secure)
            {
                binding = GetSecureBinding();
                SetCertificateOptions(serviceHost);
            }
            else
                binding = GetBinding();
            
            serviceHost.AddServiceEndpoint(typeof(IJobService), binding, "master");
            serviceHost.AddServiceEndpoint(typeof(IAuthenticationRequest), binding, "authreq");

            return serviceHost;
        }

        private NetTcpBinding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }

        private NetTcpBinding GetSecureBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);

            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            return binding;
        }

        private void SetCertificateOptions(ServiceHost serviceHost)
        {
            Trace.Information("Loading certificate from {0}", _certificatePemFile);

            if(!File.Exists(_certificatePemFile))
            {
                Trace.Critical("Unable to load server certificate from '{0}'.", _certificatePemFile);
                throw new FileNotFoundException("The server certificate could not be found.", _certificatePemFile);
            }
            
            X509Certificate2 certificate = new X509Certificate2().
                LoadFromPemFile(_certificatePemFile);

            serviceHost.Credentials.ServiceCertificate.Certificate = certificate;

            serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.Custom;
            serviceHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator =
                new ServerX509CertificateValidator();
            serviceHost.Credentials.ClientCertificate.Authentication.RevocationMode =
                X509RevocationMode.NoCheck;
        }
    }
}
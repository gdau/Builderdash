using System;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using Builderdash.Configuration;
using Synoptic.Service;
using WcfShared;
using X509Library;

namespace Builderdash
{
    public class MasterServer : IDaemon
    {
        private readonly ServerMode _serverMode;
        private readonly Uri _uri;
        private readonly static TraceSource Trace = new TraceSource("bd.master.server");
        private readonly string _certificatePemFile;
        private ServiceHost _serviceHost;

        public MasterServer(MasterConfiguration configuration)
        {
            _serverMode = configuration.Mode;
            _uri = new UriBuilder("net.tcp", configuration.Server.Address, configuration.Server.Port).Uri;
            _certificatePemFile = configuration.CertificatePemFile;
        }

        public void Start()
        {
            Console.Title = "bd.master.server";

            _serviceHost = GetServiceHost(_serverMode);
            _serviceHost.Open();

            Trace.Information("Accepting requests in {0} mode on {1}", _serverMode.ToString().ToLower(), _uri);
        }

        private ServiceHost GetServiceHost(ServerMode serverMode)
        {
            ServiceHost serviceHost = new ServiceHost(new JobServiceService(), _uri);
            NetTcpBinding binding = serverMode == ServerMode.Secure
                                        ? GetSecureBinding()
                                        : GetBinding();

            SetCertificateOptions(serviceHost);

            serviceHost.AddServiceEndpoint(typeof(IJobService), binding, "master");
            serviceHost.AddServiceEndpoint(typeof(ITest2), binding, "authreq");

            return serviceHost;
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

        private static NetTcpBinding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }

        public void Stop()
        {
            if(_serviceHost != null && 
                _serviceHost.State == CommunicationState.Opened)
            _serviceHost.Close();
            
            Trace.Information("Stopped");
        }
    }
}
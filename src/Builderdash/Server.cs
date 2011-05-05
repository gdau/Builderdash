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

        public MasterServer(ServerMode serverMode, string address, int port)
        {
            _serverMode = serverMode;
            _uri = new UriBuilder("net.tcp", address, port).Uri;
        }

        public void Start()
        {
            Console.Title = "Server";

            ServiceHost svc;

            if(_serverMode == ServerMode.Open)
            {
                svc = GetSvcLoose();
            }
            else
            {
                svc = GetSvc();
            }
            svc.Open();

            Trace.Information("Accepting requests in {0} mode on {1}", _serverMode.ToString().ToLower(), _uri);
        }

        private ServiceHost GetSvc()
        {
            ServiceHost svc = new ServiceHost(new JobServiceService(), _uri);

            X509Certificate2 cert = new X509Certificate2();
            cert.LoadFromPemFile(@"c:\\castore\cn1.pem");

            svc.Credentials.ServiceCertificate.Certificate = cert;

            //            svc.Credentials.ClientCertificate.Authentication.CertificateValidationMode =
            //                X509CertificateValidationMode.ChainTrust;

            svc.Credentials.ClientCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.Custom;
            svc.Credentials.ClientCertificate.Authentication.CustomCertificateValidator =
                new ServerX509CertificateValidator();

            svc.Credentials.ClientCertificate.Authentication.RevocationMode =
                X509RevocationMode.NoCheck;

            svc.AddServiceEndpoint(typeof(IJobService), GetTcpBinding(TcpClientCredentialType.Certificate), "master");

            svc.AddServiceEndpoint(typeof(ITest2), GetTcpBinding(TcpClientCredentialType.None), "authreq");
            
            return svc;
        }
        
        private ServiceHost GetSvcLoose()
        {
            ServiceHost svc = new ServiceHost(new JobServiceService(), _uri);

            svc.AddServiceEndpoint(typeof(IJobService), GetTcpBindingLoose(TcpClientCredentialType.Certificate), "master");
            svc.AddServiceEndpoint(typeof(ITest2), GetTcpBindingLoose(TcpClientCredentialType.None), "authreq");
            
            return svc;
        }

        private static NetTcpBinding GetTcpBinding(TcpClientCredentialType tcpClientCredentialType)
        {
            NetTcpBinding tcpBinding = new NetTcpBinding();

            tcpBinding.Security.Mode = SecurityMode.Transport;

            tcpBinding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

            tcpBinding.Security.Transport.ClientCredentialType = tcpClientCredentialType;
            return tcpBinding;
        }
        
        private static NetTcpBinding GetTcpBindingLoose(TcpClientCredentialType tcpClientCredentialType)
        {
            NetTcpBinding tcpBinding = new NetTcpBinding();

            tcpBinding.Security.Mode = SecurityMode.None;
            //tcpBinding.Security.Transport.ProtectionLevel = ProtectionLevel.None;
            //tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            
            return tcpBinding;
        }

        public void Stop()
        {
            Console.WriteLine("stopped");
        }
    }
}
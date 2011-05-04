using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using Synoptic.Service;
using WcfShared;
using X509Library;

namespace Builderdash
{
    public class BmcServer : IDaemon
    {
        public void Start()
        {
            Console.Title = "Server";

            Uri tcpUri = new Uri(@"net.tcp://localhost:55555/");

            ServiceHost svc = GetSvc(tcpUri);
            svc.Open();

            Console.WriteLine("Waiting...");
        }

        private static ServiceHost GetSvc(Uri tcpUri)
        {
            ServiceHost svc = new ServiceHost(new JobServiceService(), tcpUri);


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

            svc.AddServiceEndpoint(typeof(IJobService), GetTcpBinding(TcpClientCredentialType.Certificate), "Test");

            svc.AddServiceEndpoint(typeof(ITest2), GetTcpBinding(TcpClientCredentialType.None), "Test2");
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

        public void Stop()
        {
            Console.WriteLine("stopped");
        }
    }
}
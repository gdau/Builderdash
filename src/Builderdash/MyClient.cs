using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Threading;
using WcfShared;
using X509Library;

namespace Builderdash
{
    public class MyClient
    {
        public IJobService GetProxy()
        {
            NetTcpBinding tcpBinding = new NetTcpBinding();

            tcpBinding.Security.Mode = SecurityMode.Transport;

            tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            //                tcpBinding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
            string baseUri = @"net.tcp://localhost:55555/Test";
            //                string baseUri = @"net.tcp://train-web.kempstoncontrols.com:55555/Test";
            EndpointAddress addr = new EndpointAddress(new Uri(baseUri),
                                                       EndpointIdentity.CreateDnsIdentity("cn1"),
                                                       (AddressHeaderCollection)null);
            ChannelFactory<IJobService> factory = new DuplexChannelFactory<IJobService>(new CallbackImpl(), tcpBinding, addr);

            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.ChainTrust;

            factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            X509Certificate2 cert = new X509Certificate2();
            cert.LoadFromPemFile(Path.Combine(Environment.CurrentDirectory, "cert.pem"));

            factory.Credentials.ClientCertificate.Certificate = cert;

            return factory.CreateChannel();
        }

        public ITest2 GetOtherProxy()
        {
            NetTcpBinding tcpBinding = new NetTcpBinding();

            tcpBinding.Security.Mode = SecurityMode.Transport;

            tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            string baseUri = @"net.tcp://localhost:55555/Test2";
            EndpointAddress addr = new EndpointAddress(new Uri(baseUri), EndpointIdentity.CreateDnsIdentity("cn1"),
                                                       (AddressHeaderCollection)null);
            ChannelFactory<ITest2> factory = new ChannelFactory<ITest2>(tcpBinding, addr);

            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.ChainTrust;

            factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            //
            //                X509Certificate2 cert = new X509Certificate2();
            //                cert.LoadFromPemFile(@"c:\\castore\cn2.pem");

            //                factory.Credentials.ClientCertificate.Certificate = cert;//.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, "894d3a3dc3bbf35236a925bb235526add9c34056");

            return factory.CreateChannel();
        }
    }
}
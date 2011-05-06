using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Builderdash.Configuration;

namespace Builderdash
{
    public class AuthenticationRequestProxy
    {
        private readonly static TraceSource Trace = new TraceSource("Builderdash");

        private readonly ServerMode _serverMode;
        private readonly Uri _uri;

        public AuthenticationRequestProxy(ServerConfiguration configuration)
        {
            _serverMode = configuration.Mode;
            _uri = new UriBuilder("net.tcp", configuration.Address, configuration.Port, "authreq").Uri;
        }

        public IAuthenticationRequest GetService()
        {
            NetTcpBinding binding;
            EndpointAddress addr;

            if (_serverMode == ServerMode.Secure)
            {
                binding = GetSecureBinding();
                addr = new EndpointAddress(_uri,
                                           EndpointIdentity.CreateDnsIdentity("cn1"),
                                           (AddressHeaderCollection)null);
            }
            else
            {
                binding = GetBinding();
                addr = new EndpointAddress(_uri);
            }

            ChannelFactory<IAuthenticationRequest> factory = new ChannelFactory<IAuthenticationRequest>(binding, addr);

            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.ChainTrust;

            factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            return factory.CreateChannel();
        }

        private NetTcpBinding GetSecureBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);

            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            return binding;
        }

        private NetTcpBinding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }
    }
}
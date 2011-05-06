using System;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace Builderdash
{
    public class ServiceClientWrapper<TServiceType> : IDisposable
    {
        private readonly TServiceType _channel;
        public TServiceType Channel
        {
            get { return _channel; }
        }

        public ChannelFactory<TServiceType> ChannelFactory
        {
            get { return _channelFactory; }
        }

        private readonly ChannelFactory<TServiceType> _channelFactory;

        public ServiceClientWrapper(Binding binding, 
            EndpointAddress remoteAddress,
            object callbackObject,
            X509CertificateValidator certificateValidator, 
            X509Certificate2 clientCertificate)
        {
            if (_channelFactory == null)
            {
                _channelFactory =
                    callbackObject == null ?
                        new ChannelFactory<TServiceType>(binding, remoteAddress) :
                        new DuplexChannelFactory<TServiceType>(callbackObject, binding, remoteAddress);
            }

            if(certificateValidator != null 
                && _channelFactory.Credentials != null)
            {
                _channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode =
                    X509CertificateValidationMode.Custom;

                _channelFactory.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator =
                    certificateValidator;

                _channelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
                _channelFactory.Credentials.ClientCertificate.Certificate = clientCertificate;
            }
            
            _channel = _channelFactory.CreateChannel();
            ((IChannel)_channel).Open();
        }

        public void Dispose()
        {
            try
            {
                ((IChannel)_channel).Close();
            }
            catch (CommunicationException e)
            {
                ((IChannel)_channel).Abort();
            }
            catch (TimeoutException e)
            {
                ((IChannel)_channel).Abort();
            }
            catch (Exception e)
            {
                ((IChannel)_channel).Abort();
            }
        }
    }
}
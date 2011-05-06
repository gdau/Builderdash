using System;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using Synoptic;

namespace Builderdash
{
    public class ServerX509CertificateValidator : X509CertificateValidator
    {
        private readonly X509Certificate2 _caCertificate;
        private readonly static TraceSource Trace = new TraceSource("Builderdash");

        public ServerX509CertificateValidator(X509Certificate2 caCertificate)
        {
            _caCertificate = caCertificate;
        }

        public override void Validate(X509Certificate2 certificate)
        {
            Trace.Information("Validating certificate");
            var x509ChainPolicy = new X509ChainPolicy
                                      {
                                          RevocationMode = X509RevocationMode.NoCheck,
                                          VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority
                                      };

            x509ChainPolicy.ExtraStore.Add(_caCertificate);

            CreateChainTrustValidator(false, x509ChainPolicy).Validate(certificate);
        }
    }
}

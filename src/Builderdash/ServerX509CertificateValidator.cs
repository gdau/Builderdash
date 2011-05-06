using System;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using X509Library;

namespace Builderdash
{
    public class ServerX509CertificateValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            //return;
            var ca = new X509Certificate2().LoadFromPemFile("ca.crt");
            var x509ChainPolicy = new X509ChainPolicy
                                      {
                                          RevocationMode = X509RevocationMode.NoCheck
                                      };
                                          ;
            x509ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            x509ChainPolicy.ExtraStore.Add(ca);
            //x509ChainPolicy.

            CreateChainTrustValidator(false, x509ChainPolicy).Validate(certificate);
            Console.WriteLine("Validate called...");
            //return true;
        }
    }
}

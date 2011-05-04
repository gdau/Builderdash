using System;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;

namespace Builderdash
{
    public class ServerX509CertificateValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            var x509ChainPolicy = new X509ChainPolicy();
            x509ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            //x509ChainPolicy.

            CreateChainTrustValidator(true, x509ChainPolicy).Validate(certificate);
            Console.WriteLine("Validate called...");
            //return true;
        }
    }
}

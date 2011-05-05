using System;
using System.IO;
using System.Text;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;

namespace Builderdash.X509
{
    public class CertificateAuthority
    {
        public string GenerateCa(string dn)
        {
            AsymmetricCipherKeyPair caKeyPair = GenerateKey();
            X509Certificate caCert = GenerateCaCert(dn, caKeyPair);

            return GeneratePem(caCert) + "\n" + GeneratePem(caKeyPair.Private);
        }

        public string GenerateCert(string dn, string caPem)
        {
            string[] pem = ParsePem(caPem);

            AsymmetricCipherKeyPair caKeyPair = GetKeyPair(pem[1]);
            X509Certificate caCert = GetCert(pem[0]);

            AsymmetricCipherKeyPair keyPair = GenerateKey();

            X509Certificate cert = GenerateCert(dn, keyPair, caKeyPair, caCert);

            return GeneratePem(cert) + "\n" + GeneratePem(keyPair.Private);
        }

        private static AsymmetricCipherKeyPair GenerateKey()
        {
            RsaKeyPairGenerator generator = new RsaKeyPairGenerator();
            generator.Init(new KeyGenerationParameters(new SecureRandom(), 1024));

            return generator.GenerateKeyPair();
        }

        private static X509Certificate GenerateCaCert(string dn, AsymmetricCipherKeyPair keyPair)
        {
            DateTime startDate = new DateTime(2000, 1, 1);              // time from which certificate is valid
            DateTime expiryDate = new DateTime(2100, 1, 1);             // time after which certificate is not valid
            BigInteger serialNumber = BigInteger.ProbablePrime(120, new SecureRandom());     // serial number for certificate

            X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();

            X509Name dnName = new X509Name(dn);

            certGen.SetSerialNumber(serialNumber);
            certGen.SetIssuerDN(dnName);
            certGen.SetNotBefore(startDate);
            certGen.SetNotAfter(expiryDate);
            certGen.SetSubjectDN(dnName);                       // note: same as issuer
            certGen.SetPublicKey(keyPair.Public);
            certGen.SetSignatureAlgorithm("SHA1WITHRSA");

            return certGen.Generate(keyPair.Private);
        }

        private static X509Certificate GenerateCert(string dn, AsymmetricCipherKeyPair keyPair, AsymmetricCipherKeyPair caKeyPair, X509Certificate caCert)
        {
            var caKey = caKeyPair.Private;              // private key of the certifying authority (ca) certificate

            DateTime startDate = new DateTime(2000, 1, 1);              // time from which certificate is valid
            DateTime expiryDate = new DateTime(2100, 1, 1);             // time after which certificate is not valid
            BigInteger serialNumber = BigInteger.ProbablePrime(120, new SecureRandom());     // serial number for certificate

            X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();
            X509Name subjectName = new X509Name(dn);

            certGen.SetSerialNumber(serialNumber);
            certGen.SetIssuerDN(caCert.SubjectDN);
            certGen.SetNotBefore(startDate);
            certGen.SetNotAfter(expiryDate);
            certGen.SetSubjectDN(subjectName);
            certGen.SetPublicKey(keyPair.Public);
            certGen.SetSignatureAlgorithm("SHA1WITHRSA");

            certGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false,
                                    new AuthorityKeyIdentifierStructure(caCert));
            certGen.AddExtension(X509Extensions.SubjectKeyIdentifier, false,
                                    new SubjectKeyIdentifierStructure(keyPair.Public));

            return certGen.Generate(caKey);
        }

        private string GeneratePem(object input)
        {
            StringBuilder builder = new StringBuilder();

            PemWriter pemWriter = new PemWriter(new StringWriter(builder));
            pemWriter.WriteObject(new MiscPemGenerator(input));
            pemWriter.Writer.Flush();

            return builder.ToString();
        }

        private AsymmetricCipherKeyPair GetKeyPair(string key)
        {
            var reader = new StringReader(key);
            var pem = new PemReader(reader);
            var o = pem.ReadObject();

            return (AsymmetricCipherKeyPair)o;
        }

        private X509Certificate GetCert(string cert)
        {
            var reader = new StringReader(cert);
            var pem = new PemReader(reader);
            var o = pem.ReadObject();

            return (X509Certificate)o;
        }

        private static string[] ParsePem(string pem)
        {
            StringReader stringReader = new StringReader(pem);
            var certData = new StringBuilder();
            var keyData = new StringBuilder();

            string line;
            bool readingCert = false;
            bool readingKey = false;

            while ((line = stringReader.ReadLine()) != null)
            {
                if (line == "-----BEGIN CERTIFICATE-----") readingCert = true;
                if (readingCert)
                {
                    if (line == "-----END CERTIFICATE-----")
                        readingCert = false;

                    certData.AppendLine(line);
                }

                if (line == "-----BEGIN RSA PRIVATE KEY-----") readingKey = true;
                if (readingKey)
                {
                    if (line == "-----END RSA PRIVATE KEY-----")
                        readingKey = false;

                    keyData.AppendLine(line);
                }
            }

            return new[] { certData.ToString(), keyData.ToString() };
        }
    }
}
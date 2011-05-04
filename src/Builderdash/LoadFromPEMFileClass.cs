using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace X509Library
{
    public static class LoadFromPemFileClass
    {
        /*
         * The following function is courtesy of:
         * Michel Gallant Ph.D.
         * JavaScience Consulting
         * http://www.jensign.com.
         * (c) 2008 JavaScience Consulting.  Reproduced with permission.
         */
        private static int GetIntegerSize(BinaryReader binr)
        {
            int count;
            byte bt = binr.ReadByte();

            if (bt != 0x02)       //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte(); // data size in next byte
            else
                if (bt == 0x82)
                {
                    byte highbyte = binr.ReadByte();
                    byte lowbyte = binr.ReadByte();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;          // we already have the data size
                }

            while (binr.ReadByte() == 0x00)
            { //remove high order zeros in data
                count -= 1;
            }

            //last ReadByte wasn't a removed zero, so back up a byte
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;

        }

        /*
         * The following function is courtesy of:
         * Michel Gallant Ph.D.
         * JavaScience Consulting
         * http://www.jensign.com.
         * (c) 2008 JavaScience Consulting.  Reproduced with permission.
         */
        private static RSACryptoServiceProvider DecodeRsaPrivateKey(byte[] privkey)
        {
            byte[] E, D, P, Q, DP, DQ, IQ;

            // ------ Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            //wrap Memory Stream with BinaryReader for easy reading
            BinaryReader binr = new BinaryReader(mem);

            try
            {
                ushort twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)  //data read as little endian order
                    //(actual data order for Sequence is 30 81)
                    binr.ReadByte();     //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)  //version number
                    return null;
                byte bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;

                //------  all private key components are Integer sequences ----
                int elems = GetIntegerSize(binr);
                byte[] modulus = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                // ------ create RSACryptoServiceProvider instance ------
                // ------ and initialize with public key           ------

                var cparams = new CspParameters(1,
                    "Microsoft Base Cryptographic Provider v1.0")
                                  {
                                      KeyNumber = 1,
                                      KeyContainerName = Guid.NewGuid().ToString()
                                  };

                var rsa = new RSACryptoServiceProvider(cparams);
                var rsAparams = new RSAParameters
                                    {
                                        Modulus = modulus,
                                        Exponent = E,
                                        D = D,
                                        P = P,
                                        Q = Q,
                                        DP = DP,
                                        DQ = DQ,
                                        InverseQ = IQ
                                    };
                rsa.ImportParameters(rsAparams);
                return rsa;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        public static X509Certificate2 LoadFromPemFile(this X509Certificate2 x, string pemFile)
        {
            int i;
            string[] certFileContents = File.ReadAllLines(pemFile);
            bool certFound = false;
            bool keyFound = false;
            string certData = "";
            string keyData = "";
            byte[] certBytes = null;
            byte[] keyBytes = null;

            for (i = 0; i < certFileContents.Length; i++)
            {
                var line = certFileContents[i].Trim();

                if (String.Equals(line, "-----BEGIN CERTIFICATE-----"))
                {
                    certFound = false;
                    for (i++; i < certFileContents.Length && !certFound; i++)
                    {
                        if (!String.Equals(certFileContents[i].Trim(),
                            "-----END CERTIFICATE-----"))
                            certData += certFileContents[i];
                        else certFound = true;
                    }
                    certBytes = Convert.FromBase64String(certData.Trim());
                    continue;
                }
                if (String.Equals(line, "-----BEGIN RSA PRIVATE KEY-----"))
                {
                    keyFound = false;
                    for (i++; i < certFileContents.Length && !keyFound; i++)
                    {
                        if (!String.Equals(certFileContents[i].Trim(),
                            "-----END RSA PRIVATE KEY-----"))
                            keyData += certFileContents[i];
                        else keyFound = true;
                    }
                    keyBytes = Convert.FromBase64String(keyData.Trim());
                    continue;
                }
            }

//            var certBytes = Convert.FromBase64String(File.ReadAllText(@"D:\projects\openssl\openssl-bin\xxx_certonly.pem").Trim());
//            var keyBytes = Convert.FromBase64String(File.ReadAllText(@"D:\projects\openssl\openssl-bin\xxx_keyonly.pem").Trim());


            if (!keyFound || !certFound)
                throw new Exception("The PEM file did not contain a valid certificate and private key.");

            RSACryptoServiceProvider crypto = DecodeRsaPrivateKey(keyBytes);
            if (crypto == null)
                throw new Exception("Unable to parse the private key in the PEM file.");

            x.Import(certBytes);
            x.PrivateKey = crypto;

            return x;
        }
    }
}
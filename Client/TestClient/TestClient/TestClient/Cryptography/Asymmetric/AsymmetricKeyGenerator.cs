using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Cryptography.Asymmetric
{
    public static class AsymmetricKeyGenerator
    {
        private const int _KeySize = 2048;

        public static AsymmetricKeyPair GenerateEncryptionKeyPair()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(_KeySize))
            {
                byte[] publicKey = rsa.ExportSubjectPublicKeyInfo();
                RSAParameters privateKey = rsa.ExportParameters(true);

                return new AsymmetricKeyPair(publicKey, privateKey);
            }
        }
    }
}

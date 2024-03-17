using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Cryptography.Asymmetric
{
    public class AsymmetricDecryptor
    {
        private RSACryptoServiceProvider rsa;

        public AsymmetricDecryptor(RSAParameters privateKey)
        {
            rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(privateKey);
        }

        public byte[] Decrypt(byte[] encryptedMessageBytes)
        {
            return rsa.Decrypt(encryptedMessageBytes, true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Cryptography.Asymmetric
{
    public class AsymmetricEncryptor
    {
        private RSACryptoServiceProvider rsa;

        public AsymmetricEncryptor(byte[] publicKey)
        {
            rsa = new RSACryptoServiceProvider();
            rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
        }

        public byte[] Encrypt(byte[] messageBytes)
        {
            return rsa.Encrypt(messageBytes, true);
        }
    }
}

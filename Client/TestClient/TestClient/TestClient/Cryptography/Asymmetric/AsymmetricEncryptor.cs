using System.Security.Cryptography;

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

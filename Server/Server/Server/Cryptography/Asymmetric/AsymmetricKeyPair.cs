using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Cryptography.Asymmetric
{
    public record AsymmetricKeyPair
    {
        public byte[] PublicKey { get; }
        public RSAParameters PrivateKey { get; }

        public AsymmetricKeyPair(byte[] publicKey, RSAParameters privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
    }
}

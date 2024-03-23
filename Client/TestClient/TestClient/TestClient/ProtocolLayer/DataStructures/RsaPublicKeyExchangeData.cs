using Server.HighLevelTcpLayer;
using System.Runtime.InteropServices;
using TestClient.ProtocolLayer.DataStructures;

namespace Server.ProtocolLayer.DataStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct RsaPublicKeyExchangeData : IDataStructure
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 294)] // bsp.: 296 für 256byte key, 550 für 512byte key
        public byte[] publicKey;

        public RsaPublicKeyExchangeData(byte[] publicKey)
        {
            this.publicKey = publicKey;
        }

        public DataStructWithTypeInfo GetDataStructWithTypeInfo()
        {
            return new DataStructWithTypeInfo(this, StructType.RsaPublicKeyExchangeData);
        }
    }
}

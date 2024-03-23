using Server.HighLevelTcpLayer;
using System.Runtime.InteropServices;
using TestClient.ProtocolLayer.DataStructures;

namespace Server.ProtocolLayer.DataStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct ChallengeData : IDataStructure
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string salt;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string random;

        public ChallengeData(string salt, string random)
        {
            this.salt = salt;
            this.random = random;
        }

        public DataStructWithTypeInfo GetDataStructWithTypeInfo()
        {
            return new DataStructWithTypeInfo(this, StructType.ChallengeData);
        }
    }
}

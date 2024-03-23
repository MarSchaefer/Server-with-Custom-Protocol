using Server.HighLevelTcpLayer;
using System.Runtime.InteropServices;
using TestClient.ProtocolLayer.DataStructures;

namespace Server.ProtocolLayer.DataStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct ChallengeResponseData : IDataStructure
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 500)]
        public string response;

        public ChallengeResponseData(string response)
        {
            this.response = response;
        }

        public DataStructWithTypeInfo GetDataStructWithTypeInfo()
        {
            return new DataStructWithTypeInfo(this, StructType.ChallengeResponseData);
        }
    }
}

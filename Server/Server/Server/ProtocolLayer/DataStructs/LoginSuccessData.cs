using Server.HighLevelTcpLayer;
using System.Runtime.InteropServices;
using TestClient.ProtocolLayer.DataStructures;

namespace Server.ProtocolLayer.DataStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct LoginSuccessData : IDataStructure
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 500)]
        public string token;

        public LoginSuccessData(string token)
        {
            this.token = token;
        }

        public DataStructWithTypeInfo GetDataStructWithTypeInfo()
        {
            return new DataStructWithTypeInfo(this, StructType.LoginSuccessData);
        }
    }
}

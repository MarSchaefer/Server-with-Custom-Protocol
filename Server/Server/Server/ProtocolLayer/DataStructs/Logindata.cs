using Server.HighLevelTcpLayer;
using System.Runtime.InteropServices;
using TestClient.ProtocolLayer.DataStructures;

namespace Server.ProtocolLayer.DataStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct LoginData : IDataStructure
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string username;

        public LoginData(string username)
        {
            this.username = username;
        }

        public DataStructWithTypeInfo GetDataStructWithTypeInfo()
        {
            return new DataStructWithTypeInfo(this, StructType.LoginData);
        }
    }
}

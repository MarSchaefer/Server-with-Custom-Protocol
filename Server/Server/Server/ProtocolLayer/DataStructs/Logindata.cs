using Server.HighLevelTcpLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TestClient.ProtocolLayer.DataStructures;

namespace Server.ProtocolLayer.DataStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct LoginData : IDataStructure
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string username;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string password;

        public LoginData(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public DataStructWithTypeInfo GetDataStructWithTypeInfo()
        {
            return new DataStructWithTypeInfo(this, StructType.LoginData);
        }
    }
}

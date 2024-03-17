using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.ProtocolLayer.DataStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct LoginData
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
    }
}

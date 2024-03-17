using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.ProtocolLayer.DataStructs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct RsaPublicKeyExchangeData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 296)] // bsp.: 296 für 256byte key, 550 für 512byte key
        public byte[] publicKey;
    }
}

using Server.ProtocolLayer.DataStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.HighLevelTcpLayer
{
    public record DataStructWithTypeInfo
    {
        public object StructData { get; private set; }

        public StructType StructType { get; private set; }

        public DataStructWithTypeInfo(object structData, StructType structType)
        {
            this.StructData = structData;
            this.StructType = structType;
        }
    }
}

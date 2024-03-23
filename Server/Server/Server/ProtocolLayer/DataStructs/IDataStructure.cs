using Server.HighLevelTcpLayer;
using System.Runtime.InteropServices;

namespace TestClient.ProtocolLayer.DataStructures
{
    public interface IDataStructure
    {
        public DataStructWithTypeInfo GetDataStructWithTypeInfo();
    }
}
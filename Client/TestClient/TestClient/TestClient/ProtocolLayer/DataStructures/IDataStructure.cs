using Server.HighLevelTcpLayer;

namespace TestClient.ProtocolLayer.DataStructures
{
    public interface IDataStructure
    {
        public DataStructWithTypeInfo GetDataStructWithTypeInfo();
    }
}

using Server.ProtocolLayer;
using Server.ProtocolLayer.DataStructs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.HighLevelTcpLayer
{
    // TODO: ggf. IDisposable?
    public sealed class HighLevelTcpClient
    {
        private TcpClient _tcpClient;
        private PackageBundler _packageBundler;
        private NetworkStream _stream;

        public HighLevelTcpClient(TcpClient tcpClient)
        {
            this._tcpClient = tcpClient;
            this._packageBundler = new PackageBundler();
            this._stream = tcpClient.GetStream();
        }

        ~HighLevelTcpClient()
        {
            this.Close();
        }

        public void Close()
        {
            this._tcpClient?.Close();
            this._stream?.Close();
        }

        public async Task<DataStructWithTypeInfo> ReadAsync()
        {
            byte[] buffer = new byte[ProtocolLayerFunctions._DataPackageDataSizeInByte];
            PackageBundle packageBundle = null;

            while (packageBundle == null)
            {
                await this._stream.ReadAsync(buffer, 0, buffer.Length);
                packageBundle = _packageBundler.InsertAndRetrieveCompletePackages(buffer);
            }

            switch (packageBundle.StructType)
            {
                case StructType.LoginData:
                    LoginData loginData = ProtocolLayerFunctions.ConvertDataBundleToStruct<LoginData>(packageBundle);
                    return new DataStructWithTypeInfo((object)loginData, StructType.LoginData);
            }

            throw new Exception("Konnte Packet nicht decoden");
        }

        public async Task WriteAsync(DataStructWithTypeInfo dataStructWithTypeInfo)
        {
            byte[][] dataPackageByteSequence = null;

            switch (dataStructWithTypeInfo.StructType)
            {
                case StructType.LoginData:
                    LoginData loginData = (LoginData)dataStructWithTypeInfo.StructData;
                    dataPackageByteSequence = ProtocolLayerFunctions.ConvertStructDataToByteSequence(loginData);
                    break;
            }

            if (dataPackageByteSequence != null) {
                for (int i = 0; i < dataPackageByteSequence.Length; i++)
                {
                    byte[] packageToSend = dataPackageByteSequence[i];
                    await this._stream.WriteAsync(packageToSend, 0, packageToSend.Length);
                }
            }
        }
    }
}

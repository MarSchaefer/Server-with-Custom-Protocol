using Server.Cryptography.Asymmetric;
using Server.ProtocolLayer;
using Server.ProtocolLayer.DataStructs;
using System.Net.Sockets;
using System.Security.Cryptography;
using TestClient.ProtocolLayer.DataStructures;

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

        //public void Connect(IPAddress ipAdress, int port)
        //{
        //    this._tcpClient.Connect(ipAdress, port);
        //}

        public void Close()
        {
            this._tcpClient?.Close();
            this._stream?.Close();
        }

        public async Task<DataStructWithTypeInfo> ReadAsync()
        {
            byte[] buffer = new byte[ProtocolLayerFunctions.SizeOfDataPackage];
            PackageBundle packageBundle = null;

            while (packageBundle == null)
            {
                await this._stream.ReadAsync(buffer, 0, buffer.Length);
                packageBundle = _packageBundler.InsertAndRetrieveCompletePackages(buffer);
            }

            return PackageBundleToDataStructWithTypeInfo(packageBundle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="privateKey">Eigener privater Schlüssel</param>
        /// <returns></returns>
        public async Task<DataStructWithTypeInfo> ReadAsymmetricDecryptedAsync(RSAParameters privateKey)
        {
            AsymmetricDecryptor decryptor = new AsymmetricDecryptor(privateKey);

            byte[] buffer = new byte[ProtocolLayerFunctions.SizeOfDataPackage]; // andere größe
            PackageBundle packageBundle = null;

            while (packageBundle == null)
            {
                await this._stream.ReadAsync(buffer, 0, buffer.Length);
                byte[] dercyptedBuffer = decryptor.Decrypt(buffer);
                packageBundle = _packageBundler.InsertAndRetrieveCompletePackages(dercyptedBuffer);
            }

            return PackageBundleToDataStructWithTypeInfo(packageBundle);
        }

        public async Task WriteAsync(IDataStructure DataStructure)
        {
            DataStructWithTypeInfo dataStructWithTypeInfo = DataStructure.GetDataStructWithTypeInfo();
            byte[][] dataPackageByteSequence = ConvertDataStructWithTypeInfoToDataPackageByteSequence(dataStructWithTypeInfo);

            if (dataPackageByteSequence != null)
            {
                for (int i = 0; i < dataPackageByteSequence.Length; i++)
                {
                    byte[] packageToSend = dataPackageByteSequence[i];
                    await this._stream.WriteAsync(packageToSend, 0, packageToSend.Length);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataStructure"></param>
        /// <param name="publicKey">Öffentlicher Schlüssel vom Anderen</param>
        /// <returns></returns>
        public async Task WriteAsync(IDataStructure DataStructure, byte[] publicKey)
        {
            AsymmetricEncryptor asymmetricEncryptor = new AsymmetricEncryptor(publicKey);

            DataStructWithTypeInfo dataStructWithTypeInfo = DataStructure.GetDataStructWithTypeInfo();
            byte[][] dataPackageByteSequence = ConvertDataStructWithTypeInfoToDataPackageByteSequence(dataStructWithTypeInfo);

            if (dataPackageByteSequence != null)
            {
                for (int i = 0; i < dataPackageByteSequence.Length; i++)
                {
                    byte[] packageToSend = asymmetricEncryptor.Encrypt(dataPackageByteSequence[i]);
                    await this._stream.WriteAsync(packageToSend, 0, packageToSend.Length);
                }
            }
        }

        private DataStructWithTypeInfo PackageBundleToDataStructWithTypeInfo(PackageBundle packageBundle)
        {
            switch (packageBundle.StructType)
            {
                case StructType.LoginData:
                    LoginData data1 = ProtocolLayerFunctions.ConvertDataBundleToStruct<LoginData>(packageBundle);
                    return new DataStructWithTypeInfo((object)data1, packageBundle.StructType);

                case StructType.RsaPublicKeyExchangeData:
                    RsaPublicKeyExchangeData data2 = ProtocolLayerFunctions.ConvertDataBundleToStruct<RsaPublicKeyExchangeData>(packageBundle);
                    return new DataStructWithTypeInfo((object)data2, packageBundle.StructType);

                case StructType.ChallengeData:
                    ChallengeData data3 = ProtocolLayerFunctions.ConvertDataBundleToStruct<ChallengeData>(packageBundle);
                    return new DataStructWithTypeInfo((object)data3, packageBundle.StructType);

                case StructType.ChallengeResponseData:
                    ChallengeResponseData data4 = ProtocolLayerFunctions.ConvertDataBundleToStruct<ChallengeResponseData>(packageBundle);
                    return new DataStructWithTypeInfo((object)data4, packageBundle.StructType);

                case StructType.LoginSuccessData:
                    LoginSuccessData data5 = ProtocolLayerFunctions.ConvertDataBundleToStruct<LoginSuccessData>(packageBundle);
                    return new DataStructWithTypeInfo((object)data5, packageBundle.StructType);
            }

            throw new Exception("PackageBundleToDataStructWithTypeInfo error");
        }

        byte[][] ConvertDataStructWithTypeInfoToDataPackageByteSequence(DataStructWithTypeInfo dataStructWithTypeInfo)
        {
            switch (dataStructWithTypeInfo.StructType)
            {
                case StructType.LoginData:
                    LoginData data1 = (LoginData)dataStructWithTypeInfo.StructData;
                    return ProtocolLayerFunctions.ConvertStructDataToByteSequence(data1, dataStructWithTypeInfo.StructType);

                case StructType.RsaPublicKeyExchangeData:
                    RsaPublicKeyExchangeData data2 = (RsaPublicKeyExchangeData)dataStructWithTypeInfo.StructData;
                    return ProtocolLayerFunctions.ConvertStructDataToByteSequence(data2, dataStructWithTypeInfo.StructType);

                case StructType.ChallengeData:
                    ChallengeData data3 = (ChallengeData)dataStructWithTypeInfo.StructData;
                    return ProtocolLayerFunctions.ConvertStructDataToByteSequence(data3, dataStructWithTypeInfo.StructType);

                case StructType.ChallengeResponseData:
                    ChallengeResponseData data4 = (ChallengeResponseData)dataStructWithTypeInfo.StructData;
                    return ProtocolLayerFunctions.ConvertStructDataToByteSequence(data4, dataStructWithTypeInfo.StructType);

                case StructType.LoginSuccessData:
                    LoginSuccessData data5 = (LoginSuccessData)dataStructWithTypeInfo.StructData;
                    return ProtocolLayerFunctions.ConvertStructDataToByteSequence(data5, dataStructWithTypeInfo.StructType);
            }

            throw new Exception("ConvertDataStructWithTypeInfoToDataPackageByteSequence error");
        }
    }
}

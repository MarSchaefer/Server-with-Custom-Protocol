using Server.HighLevelTcpLayer;
using System.Net.Sockets;
using System.Net;
using Server.Cryptography.Asymmetric;
using Server.ProtocolLayer.DataStructs;
using System.Text;
using Server.Source.Classes;

class Program
{
    static class Client
    {
        public static void Main(string[] args)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse("192.168.2.30"), 5600);
            HighLevelTcpClient highLevelTcpClient = new HighLevelTcpClient(tcpClient);

            string clientToken = LoginAsync(highLevelTcpClient).GetAwaiter().GetResult();
            Console.WriteLine(clientToken);

            //var readDataTask = ReadDataAsync(highLevelTcpClient);
            //var writeDataTask = WriteDataAsync(highLevelTcpClient);

            //Task.WaitAll(readDataTask, writeDataTask);
        }

        /// <summary> Gibt Token zurück </summary>
        public static async Task<string> LoginAsync(HighLevelTcpClient highLevelTcpClient)
        {
            AsymmetricKeyPair asymmetricKeyPair = AsymmetricKeyGenerator.GenerateEncryptionKeyPair();
            RsaPublicKeyExchangeData publicKeyExchangeData = new RsaPublicKeyExchangeData(asymmetricKeyPair.PublicKey);
            
            await highLevelTcpClient.WriteAsync(publicKeyExchangeData);
            DataStructWithTypeInfo dataStructWithTypeInfo = await highLevelTcpClient.ReadAsync();

            RsaPublicKeyExchangeData publicKeyOfServerData = (RsaPublicKeyExchangeData)dataStructWithTypeInfo.StructData;
            byte[] publicKeyOfServer = publicKeyOfServerData.publicKey;

            await Console.Out.WriteLineAsync("Client Key: " + asymmetricKeyPair.PublicKey.Length);
            await Console.Out.WriteLineAsync("Server Key: " + publicKeyOfServer.Length);

            string name = "Joshbert";
            string passwort = "1337Abc";

            Verschluesselung.StringToSha512(password + salt);

            return await Task.FromResult("");
        }


        // Zum Testen
        //public static async Task WriteDataAsync(HighLevelTcpClient highLevelTcpClient)
        //{
        //    while (true)
        //    {
        //        LoginData loginData = new LoginData("bob", "1337");

        //        DataStructWithTypeInfo dataStructWithTypeInfo =
        //            new DataStructWithTypeInfo(loginData, StructType.LoginData);

        //        await highLevelTcpClient.WriteAsync(dataStructWithTypeInfo);
        //        await Task.Delay(1000);
        //    }
        //}

        //public static async Task ReadDataAsync(HighLevelTcpClient highLevelTcpClient)
        //{
        //    while (true)
        //    {
        //        DataStructWithTypeInfo dataStructWithTypeInfo = await highLevelTcpClient.ReadAsync();

        //        switch (dataStructWithTypeInfo.StructType)
        //        {
        //            case StructType.LoginData:
        //                LoginData loginData = (LoginData)dataStructWithTypeInfo.StructData;
        //                    await Console.Out.WriteLineAsync(loginData.username + " " + loginData.password);
        //                break;
        //        }
        //    }
        //}
    }

}
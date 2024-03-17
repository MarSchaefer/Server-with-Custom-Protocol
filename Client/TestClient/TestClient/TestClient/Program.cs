using Server.HighLevelTcpLayer;
using System.Net.Sockets;
using System.Net;
using Server.ProtocolLayer.DataStructs;

class Program
{
    static class Client
    {
        public static void Main(string[] args)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse("192.168.2.30"), 5600);
            HighLevelTcpClient highLevelTcpClient = new HighLevelTcpClient(tcpClient);

            var readDataTask = ReadDataAsync(highLevelTcpClient);
            var writeDataTask = WriteDataAsync(highLevelTcpClient);

            Task.WaitAll(readDataTask, writeDataTask);

            Thread.Sleep(10000);
        }

        public static async Task WriteDataAsync(HighLevelTcpClient highLevelTcpClient)
        {
            while (true)
            {
                LoginData loginData = new LoginData("bob", "1337");

                DataStructWithTypeInfo dataStructWithTypeInfo =
                    new DataStructWithTypeInfo(loginData, StructType.LoginData);

                await highLevelTcpClient.WriteAsync(dataStructWithTypeInfo);
                await Task.Delay(1000);
            }
        }

        public static async Task ReadDataAsync(HighLevelTcpClient highLevelTcpClient)
        {
            while (true)
            {
                DataStructWithTypeInfo dataStructWithTypeInfo = await highLevelTcpClient.ReadAsync();

                switch (dataStructWithTypeInfo.StructType)
                {
                    case StructType.LoginData:
                        LoginData loginData = (LoginData)dataStructWithTypeInfo.StructData;
                            await Console.Out.WriteLineAsync(loginData.username + " " + loginData.password);
                        break;
                }
            }
        }
    }

}
using System.Net;
using System.Net.Sockets;

namespace Server.HighLevelTcpLayer
{
    public sealed class HighLevelTcpListener
    {
        private TcpListener _tcpListener;

        public HighLevelTcpListener(IPAddress localaddr, int port)
        {
            this._tcpListener = new TcpListener(localaddr, port);
        }

        public void Start()
        {
            this._tcpListener.Start();
        }

        public void Stop()
        {
            this._tcpListener.Stop();
        }

        public async Task<HighLevelTcpClient> AcceptTcpClientAsync()
        {
            TcpClient tcpClient = await this._tcpListener.AcceptTcpClientAsync();
            return new HighLevelTcpClient(tcpClient);
        }
    }
}

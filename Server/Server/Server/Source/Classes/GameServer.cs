using Server.Source.DataModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Source.Classes
{
    public sealed class GameServer
    {
        private readonly ServerConfig _serverConfig;

        private IReadOnlyDictionary<string, RegisteredUser> _registeredUsers;

        private readonly TcpListener _tcpListener;

        // TODO: Anderes Model nehmen
        // string ist token, 
        // TODO: Code umbauen:
        private static ConcurrentDictionary<string, TcpClient> _clients = new ConcurrentDictionary<string, TcpClient>();

        /// <summary>
        /// key ist die challenge, value ist der user
        /// </summary>
        private static ConcurrentDictionary<string, TcpClient> _usersInLoginProcess = new ConcurrentDictionary<string, TcpClient>();


        /// <summary>
        /// key ist ein token, value ist der user
        /// </summary>
        private static ConcurrentDictionary<string, TcpClient> _loggedInUsers = new ConcurrentDictionary<string, TcpClient>();

        public GameServer(ServerConfig serverConfig, IReadOnlyDictionary<string, RegisteredUser> registeredUsers)
        {
            this._serverConfig = serverConfig;
            this._registeredUsers = registeredUsers;
            this._tcpListener = new TcpListener(IPAddress.Any, serverConfig.Port);
        }

        public async Task RunAsync()
        {
            _tcpListener.Start();
            Console.WriteLine("Server gestartet...");

            try
            {
                while (true)
                {
                    TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                    // check ob user registriert ist
                    // check ob user gueltigen pw-hash hat
                    // gib user ein session-token
                    
                    _ = HandleClientAsync(client);
                }
            }
            finally
            {
                _tcpListener.Stop();
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient)
        {
            string clientId = Guid.NewGuid().ToString();
            _clients.TryAdd(clientId, tcpClient);

            Console.WriteLine($"Neuer Client verbunden: {clientId}");

            try
            {
                NetworkStream stream = tcpClient.GetStream();
                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Nachricht von {clientId}: {message}");

                    // Broadcast Nachricht an alle Clients
                    foreach (var otherClient in _clients)
                    {
                        if (otherClient.Key != clientId)
                        {
                            NetworkStream otherStream = otherClient.Value.GetStream();
                            byte[] messageBytes = Encoding.UTF8.GetBytes($"Client {clientId}: {message}");
                            await otherStream.WriteAsync(messageBytes, 0, messageBytes.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client {clientId} getrennt: {ex.Message}");
            }
            finally
            {
                tcpClient.Close();
                _clients.TryRemove(clientId, out _);
                Console.WriteLine($"Client {clientId} getrennt.");
            }
        }
    }
}

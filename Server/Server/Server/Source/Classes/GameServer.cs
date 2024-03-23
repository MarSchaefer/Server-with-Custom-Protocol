﻿using Server.Cryptography.Asymmetric;
using Server.HighLevelTcpLayer;
using Server.ProtocolLayer;
using Server.ProtocolLayer.DataStructs;
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

        private readonly HighLevelTcpListener _tcpListener;

        // TODO: Anderes Model nehmen
        // string ist token, 
        // TODO: Code umbauen:
        private static ConcurrentDictionary<string, HighLevelTcpClient> _clients = new ConcurrentDictionary<string, HighLevelTcpClient>();

        /// <summary>
        /// key ist die challenge, value ist der user
        /// </summary>
        private static ConcurrentDictionary<string, HighLevelTcpClient> _usersInLoginProcess = new ConcurrentDictionary<string, HighLevelTcpClient>();


        /// <summary>
        /// key ist ein token, value ist der user
        /// </summary>
        private static ConcurrentDictionary<string, HighLevelTcpClient> _loggedInUsers = new ConcurrentDictionary<string, HighLevelTcpClient>();

        public GameServer(ServerConfig serverConfig, IReadOnlyDictionary<string, RegisteredUser> registeredUsers)
        {
            this._serverConfig = serverConfig;
            this._registeredUsers = registeredUsers;
            this._tcpListener = new HighLevelTcpListener(IPAddress.Any, serverConfig.Port);
        }

        public async Task RunAsync()
        {
            _tcpListener.Start();
            Console.WriteLine("Server gestartet...");

            try
            {
                while (true)
                {
                    HighLevelTcpClient client = await _tcpListener.AcceptTcpClientAsync();
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

        private async Task HandleClientAsync(HighLevelTcpClient tcpClient)
        {
            string clientId = Guid.NewGuid().ToString();
            _clients.TryAdd(clientId, tcpClient);

            Console.WriteLine($"Neuer Client verbunden: {clientId}");

            try
            {
                while (true)
                {
                    DataStructWithTypeInfo dataStructWithTypeInfo = await tcpClient.ReadAsync();

                    switch (dataStructWithTypeInfo.StructType)
                    {
                        //case StructType.LoginData:
                        //    LoginData loginData = (LoginData)dataStructWithTypeInfo.StructData;

                        //    foreach (KeyValuePair<string, HighLevelTcpClient> clientKeyValuePair in _clients)
                        //    {
                        //        // only other clients
                        //        //if (clientKeyValuePair.Key != clientId)
                        //        //{
                        //        await clientKeyValuePair.Value.WriteAsync(loginData);
                        //        //}
                        //    }

                        //    break;

                        // TODO: Switch Auslagern

                        case StructType.RsaPublicKeyExchangeData:
                            // LOGIN
                            RsaPublicKeyExchangeData publicKeyOfClientData = (RsaPublicKeyExchangeData)dataStructWithTypeInfo.StructData;
                            byte[] publicKeyOfClient = publicKeyOfClientData.publicKey;

                            AsymmetricKeyPair asymmetricKeyPair = AsymmetricKeyGenerator.GenerateEncryptionKeyPair();
                            RsaPublicKeyExchangeData publicKeyExchangeData = new RsaPublicKeyExchangeData(asymmetricKeyPair.PublicKey);

                            await tcpClient.WriteAsync(publicKeyExchangeData);

                            await Console.Out.WriteLineAsync("Server Key: " + asymmetricKeyPair.PublicKey.Length);
                            await Console.Out.WriteLineAsync("Client Key: " + publicKeyOfClient.Length);

                            //------------------------------------------------------------------------
                            Random random = new Random();
                            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                            string salt = _serverConfig.Salt;
                            string randomString = new string(Enumerable.Repeat(chars, 50).Select(s => s[random.Next(s.Length)]).ToArray());



                            Verschluesselung.StringToSha512(password + salt);

                            //------------------------------------------------------------------------
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                tcpClient.Close();
                _clients.TryRemove(clientId, out _);
                Console.WriteLine($"Client {clientId} getrennt: {ex.Message}");

            }
        }
    }
}

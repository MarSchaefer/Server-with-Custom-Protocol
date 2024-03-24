using Server.Cryptography.Asymmetric;
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
    public sealed class LoginController
    {
        private readonly ServerConfig _serverConfig;

        private IReadOnlyDictionary<string, RegisteredUser> _registeredUsers;

        private readonly HighLevelTcpListener _tcpListener;

        /// <summary>
        /// key ist ein token, value ist der user
        /// </summary>
        public static ConcurrentDictionary<string, HighLevelTcpClient> _loggedInUsers = new ConcurrentDictionary<string, HighLevelTcpClient>();

        public LoginController(ServerConfig serverConfig, IReadOnlyDictionary<string, RegisteredUser> registeredUsers)
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
            Console.WriteLine($"Neuer Client verbunden: {clientId}");
            string token = null;
            KeyValuePair<string, HighLevelTcpClient>? loggedInUser = null;

            try
            {
                while (true)
                {
                    DataStructWithTypeInfo dataStructWithTypeInfo = await tcpClient.ReadAsync();

                    switch (dataStructWithTypeInfo.StructType)
                    {
                        //case StructType.LoginData:
                        //    LoginData loginData2 = (LoginData)dataStructWithTypeInfo.StructData;
                        //    // dataStructWithTypeInfo
                        //    await tcpClient.WriteAsync(loginData2);
                        //    break;

                        #region neuer code
                        case StructType.RsaPublicKeyExchangeData:
                            // Lesen Public Key von Client

                            // Schicken Public Key vom Server

                            // Lese LoginData

                            // Schicke Challenge

                            // Lese Challenge Response

                            // Schicke Token

                            // ----------------------------------------------------------------------------------
                            // Public Key austausch
                            RsaPublicKeyExchangeData publicKeyOfClientData = (RsaPublicKeyExchangeData)dataStructWithTypeInfo.StructData;
                            byte[] publicKeyOfClient = publicKeyOfClientData.publicKey;

                            AsymmetricKeyPair asymmetricKeyPair = AsymmetricKeyGenerator.GenerateEncryptionKeyPair();
                            RsaPublicKeyExchangeData publicKeyExchangeData = new RsaPublicKeyExchangeData(asymmetricKeyPair.PublicKey);
                            var privateKey = asymmetricKeyPair.PrivateKey;

                            // Schreibe Client Public Key
                            await tcpClient.WriteAsync(publicKeyExchangeData);

                            await Console.Out.WriteLineAsync("Server Key: " + asymmetricKeyPair.PublicKey.Length);
                            await Console.Out.WriteLineAsync("Client Key: " + publicKeyOfClient.Length);
                            // ----------------------------------------------------------------------------------

                            // Lies Client Benutzernamen
                            DataStructWithTypeInfo loginDataAsDataStructWithTypeInfo = await tcpClient.ReadAsymmetricDecryptedAsync(privateKey);
                            LoginData loginData = (LoginData)loginDataAsDataStructWithTypeInfo.StructData;
                            string userName = loginData.username;

                            if (this._registeredUsers.TryGetValue(userName, out RegisteredUser registeredUser) == false)
                            {
                                await Console.Out.WriteLineAsync(userName + " wollte sich anmelden, name existiert aber nicht");
                                // TODO: in implementierung throw exception oder sowas
                                continue;
                            }

                            //------------ Lies Client Benutzernamen >
                            //------------ < Anmeldeprozess
                            Random random = new Random();
                            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                            string salt = _serverConfig.Salt;
                            string randomString = new string(Enumerable.Repeat(chars, 50).Select(s => s[random.Next(s.Length)]).ToArray());

                            ChallengeData challengeData = new ChallengeData(salt, randomString);

                            // Schreibe Client Challenge
                            await tcpClient.WriteAsymmetricEncryptedAsync(challengeData, publicKeyOfClient);

                            // Lese Client Challenge Response
                            DataStructWithTypeInfo challengeResponseDataAsDataStructWithTypeInfo = await tcpClient.ReadAsymmetricDecryptedAsync(privateKey);
                            ChallengeResponseData challengeResponseData = (ChallengeResponseData)challengeResponseDataAsDataStructWithTypeInfo.StructData;
                            string challengeSolution = Verschluesselung.StringToSha512(registeredUser.Password + randomString);

                            if (challengeSolution != challengeResponseData.result)
                            {
                                // TODO: in implementierung throw exception oder sowas
                                await Console.Out.WriteLineAsync(userName + " wollte sich anmelden, hat die challenge aber nicht gelöst");
                                continue;
                            }

                            // TODO: besser machen
                            token = Guid.NewGuid().ToString();

                            LoginSuccessData loginSuccessData = new LoginSuccessData(token);
                            await tcpClient.WriteAsymmetricEncryptedAsync(loginSuccessData, publicKeyOfClient);

                            // TODO: ...

                            loggedInUser = new KeyValuePair<string, HighLevelTcpClient>(token, tcpClient);
                            LoginController._loggedInUsers.TryAdd(loggedInUser.Value.Key, loggedInUser.Value.Value);
                            //------------ Anmeldeprozess >
                            break;
                            #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                tcpClient.Close();
                // TODO: ...
                if (token != null && loggedInUser != null)
                {
                    LoginController._loggedInUsers.TryRemove(loggedInUser.Value);
                }
                Console.WriteLine($"Client {clientId} getrennt: {ex.Message}");

            }
        }
    }
}

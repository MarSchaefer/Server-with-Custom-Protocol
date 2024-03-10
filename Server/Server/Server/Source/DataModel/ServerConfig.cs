namespace Server.Source.DataModel
{
    public record ServerConfig
    {
        public int Port { get; }

        public string Salt { get; }

        public ServerConfig(int Port, string Salt)
        {
            this.Port = Port;
            this.Salt = Salt;
        }
    }
}

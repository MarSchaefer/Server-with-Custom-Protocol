namespace Server.Source.StaticClasses
{
    public static class ServerPaths
    {
        private static readonly string _serverConfigJsonFilePath = @"\Config\server-config.json";
        private static readonly string _serverClientDataFilePath = @"\Data\clients.json";
        private static readonly string _serverDataDirectory = @"\Data\";

        private static string? GetPathOfProject()
        {
            string pathOfBinarys = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));
            return Directory.GetParent(pathOfBinarys).Parent.ToString() + @"\Server";
        }

        public static string ServerConfigFile()
        {
            return GetPathOfProject() + _serverConfigJsonFilePath;
        }

        public static string ServerClientFile()
        {
            return GetPathOfProject() + _serverClientDataFilePath;
        }

        public static string ServerDataDirectory()
        {
            return GetPathOfProject() + _serverDataDirectory;
        }
    }
}

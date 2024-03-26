using Server.Source.Classes;
using Server.Source.DataModel;
using Server.Source.StaticClasses;

sealed class Program
{
    public static ServerConfig LoadServerConfig()
    {
        SerializableData<ServerConfig> configDataSerializer =
            SerializableDataFactory.CreateServerConfigSerializable();

        return configDataSerializer.LoadValue();
    }

    public static IReadOnlyDictionary<string, RegisteredUser> LoadRegisteredUsers()
    {
        var registeredUsersSerializer =
            SerializableDataFactory.CreateRegisteredUserSerializable();

        var registeredUsers = registeredUsersSerializer.LoadValue();
        return registeredUsers.ToDictionary(key => key.UserName);
    }

    public static LoginController GetInitializedServer()
    {
        ServerConfig serverConfig = LoadServerConfig();
        IReadOnlyDictionary<string, RegisteredUser> registeredUsers = LoadRegisteredUsers();
        return new LoginController(serverConfig, registeredUsers);
    }

    public static void Main(string[] args)
    {
        LoginController loginController = GetInitializedServer();
        var serverTask = loginController.RunAsync();
        Task.WaitAll(serverTask);
    }
}
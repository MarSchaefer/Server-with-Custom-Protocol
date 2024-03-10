﻿using Server.Source.Classes;
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

    public static GameServer GetInitializedServer()
    {
        ServerConfig serverConfig = LoadServerConfig();
        IReadOnlyDictionary<string, RegisteredUser> registeredUsers = LoadRegisteredUsers();
        return new GameServer(serverConfig, registeredUsers);
    }

    public static void Main(string[] args)
    {
        GameServer gameServer = GetInitializedServer();
        var serverTask = gameServer.RunAsync();
        Task.WaitAll(serverTask);
    }
}
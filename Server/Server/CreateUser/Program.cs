using Server.Source.Classes;
using Server.Source.DataModel;
using Server.Source.StaticClasses;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var registeredUserSerializer = SerializableDataFactory.CreateRegisteredUserSerializable();
        var serverConfigSerializer = SerializableDataFactory.CreateServerConfigSerializable();

        var registeredUsers = registeredUserSerializer.LoadValue();
        var serverConfig = serverConfigSerializer.LoadValue();

        var (username, password) = EnterUserNameAndPassword();

        // wenn user mit namen bereits existiert, breche ab
        if (registeredUsers.Find(x => x.UserName == username) != null)
        {
            Console.WriteLine("User mit diesem Namen existiert bereits");
            return;
        }

        registeredUsers.Add(new RegisteredUser(username, HashPasswordWithSalt(password, serverConfig.Salt)));
        registeredUserSerializer.SaveValue(registeredUsers);
    }

    public static string HashPasswordWithSalt(string password, string salt)
    {
        return HashAlgorithms.StringToSha512(password + salt);
    }

    public static (string userName, string password) EnterUserNameAndPassword()
    {
        Console.Write("Username: ");
        string userName = Console.ReadLine();
        Console.WriteLine();

        Console.Write("Passwort: ");
        string password = Console.ReadLine();
        Console.WriteLine();

        return (userName, password);
    }
}

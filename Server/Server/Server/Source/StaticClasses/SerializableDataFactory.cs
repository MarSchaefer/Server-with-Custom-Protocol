using Server.Source.Classes;
using Server.Source.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Source.StaticClasses
{
    public static class SerializableDataFactory
    {
        public static SerializableData<ServerConfig> CreateServerConfigSerializable()
        {
            return new SerializableData<ServerConfig>(
                        ServerPaths.ServerConfigFile(),
                        "Config",
                        new ServerConfig(0, "as$d123said/SAdjio121"));
        }

        public static SerializableData<List<RegisteredUser>> CreateRegisteredUserSerializable()
        {
            return new SerializableData<List<RegisteredUser>>(
                            ServerPaths.ServerClientFile(),
                            "RegistedUsers",
                            new List<RegisteredUser>());
        }
    }
}

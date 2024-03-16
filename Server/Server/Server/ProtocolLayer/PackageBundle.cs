using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ProtocolLayer
{
    public class PackageBundle
    {
        public DataPackage[] DataPackages;
        public StructType StructType;

        public PackageBundle(DataPackage[] dataPackages, StructType structType)
        {
            DataPackages = dataPackages;
            StructType = structType;
        }
    }
}

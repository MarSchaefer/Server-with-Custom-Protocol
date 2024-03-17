using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ProtocolLayer
{
    public class PackageBundler
    {
        private readonly Dictionary<long, PackageGroupBuffer> _PackageGroupBufferDictionary;

        public PackageBundler()
        {
            _PackageGroupBufferDictionary = new Dictionary<long, PackageGroupBuffer>();
        }

        /// <summary>
        /// Wenn Packete vollständig gesammelt wurden, werden diese zurückgegeben
        /// Wenn in diesem Durchlauf Packete nicht vollständig gesammelt wurden, wird null zurück gegeben.
        /// </summary>
        /// <param name="dataPackageInBytes"></param>
        /// <returns></returns>

        public PackageBundle InsertAndRetrieveCompletePackages(byte[] dataPackageInBytes)
        {
            DataPackage dataPackage = ProtocolLayerFunctions.ConvertBytesToStructure<DataPackage>(dataPackageInBytes);
            long dataPackageGroupId = dataPackage.dataPackageGroupId;
            int countOfPackagesInGroup = dataPackage.countOfPackagesInGroup;

            InsertIntoBuffer(dataPackage, dataPackageGroupId, countOfPackagesInGroup);

            PackageGroupBuffer usedPackageGroupBuffer = _PackageGroupBufferDictionary[dataPackageGroupId];
            if (usedPackageGroupBuffer.IsBufferComplete())
            {
                DataPackage[] dataPackages = usedPackageGroupBuffer.dataPackages;
                RemovePackageGroupBuffer(dataPackageGroupId);
                return new PackageBundle(dataPackages, dataPackages[0].structType);
            }

            return null;
        }

        private void InsertIntoBuffer(DataPackage dataPackage, long dataPackageGroupId, int countOfPackagesInGroup)
        {
            // Wenn ein Buffer schon existiert, füge das Packet in den vorhanden Buffer ein
            if (_PackageGroupBufferDictionary.TryGetValue(dataPackageGroupId, out PackageGroupBuffer packageGroupBuffer))
            {
                packageGroupBuffer?.InsertDataPackage(dataPackage);
            }
            // Anderenfalls erzeuge einen neuen Buffer und füge das erste Packet hinzu
            else
            {
                PackageGroupBuffer newPackageGroupBuffer = new PackageGroupBuffer(countOfPackagesInGroup, dataPackage);
                _PackageGroupBufferDictionary.Add(dataPackageGroupId, newPackageGroupBuffer);
            }
        }

        private void RemovePackageGroupBuffer(long dataPackageGroupId)
        {
            _PackageGroupBufferDictionary.Remove(dataPackageGroupId);
        }
    }
}

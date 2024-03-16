using Server.Source.StaticClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.Source.StaticClasses
{
    public enum StructType
    {
        LoginData,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct LoginData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string username;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string password;

        public LoginData(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct DataPackage
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] data;

        public int countOfExtraBytesToIgnore;
        public int countOfPackagesInGroup;
        public int sequenceNumber;
        public long dataPackageGroupId;
        public StructType structType;

        public DataPackage(
            byte[] data,
            int countOfExtraBytesToIgnore,
            int countOfPackagesInGroup,
            int sequenceNumber,
            long dataPackageGroupId,
            StructType structType)
        {
            this.data = data;
            this.countOfPackagesInGroup = countOfPackagesInGroup;
            this.sequenceNumber = sequenceNumber;
            this.countOfExtraBytesToIgnore = countOfExtraBytesToIgnore;
            this.dataPackageGroupId = dataPackageGroupId;
            this.structType = structType;
        }
    }

    public class PackageGroupBuffer
    {
        public DataPackage[] DataPackages;
        public int countOfPackagesInGroup;
        private int lastInsertedPackageIndex;

        public PackageGroupBuffer(int countOfPackagesInGroup, DataPackage firstPackage)
        {
            this.DataPackages = new DataPackage[countOfPackagesInGroup];
            this.countOfPackagesInGroup = countOfPackagesInGroup;
            this.DataPackages[0] = firstPackage;
            this.lastInsertedPackageIndex = 0;
        }

        public void InsertDataPackage(DataPackage firstPackage)
        {
            if (lastInsertedPackageIndex < DataPackages.Length)
            {
                this.DataPackages[this.lastInsertedPackageIndex + 1] = firstPackage;
                this.lastInsertedPackageIndex++;
            }
        }

        public bool IsBufferComplete()
        {
            return countOfPackagesInGroup == lastInsertedPackageIndex + 1;
        }
    }

    public class PackageBundle
    {
        public DataPackage[] DataPackages;
        public StructType StructType;

        public PackageBundle(DataPackage[] dataPackages, StructType structType)
        {
            this.DataPackages = dataPackages;
            this.StructType = structType;
        }
    }

    public class PackageBundler
    {
        private Dictionary<long, PackageGroupBuffer> PackageGroupBufferDictionary = new Dictionary<long, PackageGroupBuffer>();

        /// <summary>
        /// Wenn Packete vollständig gesammelt wurden, werden diese zurückgegeben
        /// Wenn in diesem Durchlauf Packete nicht vollständig gesammelt wurden, wird null zurück gegeben.
        /// </summary>
        /// <param name="dataPackageInBytes"></param>
        /// <returns></returns>

        public PackageBundle? InsertAndRetrieveCompletePackages(byte[] dataPackageInBytes)
        {
            DataPackage dataPackage = ProtocolLayerFunctions.BytesToStructure<DataPackage>(dataPackageInBytes);
            long dataPackageGroupId = dataPackage.dataPackageGroupId;
            int countOfPackagesInGroup = dataPackage.countOfPackagesInGroup;

            InsertIntoBuffer(dataPackage, dataPackageGroupId, countOfPackagesInGroup);

            PackageGroupBuffer usedPackageGroupBuffer = PackageGroupBufferDictionary[dataPackageGroupId];
            if (usedPackageGroupBuffer.IsBufferComplete())
            {
                DataPackage[] dataPackages = usedPackageGroupBuffer.DataPackages;
                RemovePackageGroupBuffer(dataPackageGroupId);
                // TODO: dataPackages vor dem Einfügen sortieren
                return new PackageBundle(dataPackages, dataPackages[0].structType);
            }

            return null;
        }

        private void InsertIntoBuffer(DataPackage dataPackage, long dataPackageGroupId, int countOfPackagesInGroup)
        {
            // Wenn ein Buffer schon existiert, füge das Packet in den vorhanden Buffer ein
            if (PackageGroupBufferDictionary.ContainsKey(dataPackageGroupId))
            {
                PackageGroupBufferDictionary[dataPackageGroupId].InsertDataPackage(dataPackage);
            }
            // Anderenfalls erzeuge einen neuen Buffer und füge das erste Packet hinzu
            else
            {
                PackageGroupBuffer newPackageGroupBuffer = new PackageGroupBuffer(countOfPackagesInGroup, dataPackage);
                PackageGroupBufferDictionary.Add(dataPackageGroupId, newPackageGroupBuffer);
            }
        }

        private void RemovePackageGroupBuffer(long dataPackageGroupId)
        {
            if (PackageGroupBufferDictionary.ContainsKey(dataPackageGroupId))
            {
                PackageGroupBufferDictionary.Remove(dataPackageGroupId);
            }
        }
    }
    
    public static class ProtocolLayerFunctions
    {
        #region Beispiel
        /*
        public static void Test()
        {
            LoginData loginDataToSend = new LoginData("Bob1337", "sudo");
            DataPackage[] dataPackages = ConvertStructDataToDataPackages(loginDataToSend);
            byte[][] dataPackageByteSequence = ConvertDataPackagesToByteSequence(dataPackages);

            //// Bytes Senden ---->

            //// Bytes Empfangen <----
           
            PackageBundler packageBundler = new PackageBundler();

            for (int i = 0; i < dataPackageByteSequence.Length; i++)
            {
                PackageBundle? dataPackageBundle = packageBundler.InsertAndRetrieveCompletePackages(dataPackageByteSequence[i]);
                if (dataPackageBundle != null)
                {
                    // MessageDispatcher, !TypeConverter!
                    switch (dataPackageBundle.StructType) {
                        case StructType.LoginData:
                            LoginData loginDataToRetrieve = ConvertDataBundleToStruct<LoginData>(dataPackageBundle);
                            Console.WriteLine(loginDataToRetrieve.username);
                            Console.WriteLine(loginDataToRetrieve.password);
                            break;
                    }
                }
            }
        }
        */
        #endregion

        private static int _DataPackageDataSizeInByte = 8;

        public static byte[][] ConvertDataPackagesToByteSequence(DataPackage[] dataPackages)
        {
            byte[][] dataPackageByteSequence = new byte[dataPackages.Length][];
            for (int i = 0; i < dataPackages.Length; i++)
            {
                dataPackageByteSequence[i] = StructureToBytes(dataPackages[i]);
            }
            return dataPackageByteSequence;
        }

        public static DataPackage[] ConvertStructDataToDataPackages(LoginData loginData)
        {
            byte[] structureInBytes = StructureToBytes(loginData);
            int sizeOfStructureInBytes = structureInBytes.Length;
            int countOfExtraBytesToMatchPackageSize = sizeOfStructureInBytes % _DataPackageDataSizeInByte;
            long packageGroupId = DateTime.Now.Ticks;

            return BuildDataPackagesWithMatchedByteSize(structureInBytes, countOfExtraBytesToMatchPackageSize, packageGroupId);
        }

        public static T ConvertDataBundleToStruct<T>(PackageBundle dataBundle) where T : struct
        {
            DataPackage[] dataPackages = dataBundle.DataPackages;
            int extraBytesToIgnore = dataPackages.Last().countOfExtraBytesToIgnore;

            if (extraBytesToIgnore != 0)
            {
                return BuildStructure<T>(dataPackages, extraBytesToIgnore);
            }

            return BuildStructure<T>(dataPackages);
        }


        public static byte[] StructureToBytes<T>(T structure) where T : struct
        {
            int size = Marshal.SizeOf(structure);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return arr;
        }

        public static T BytesToStructure<T>(byte[] bytes) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            if (bytes.Length < size)
            {
                throw new Exception("Invalid parameter");
            }

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, ptr, size);
                return Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        private static DataPackage[] BuildDataPackagesWithMatchedByteSize(
            byte[] structureInBytes,
            int countOfExtraBytesToMatchPackageSize,
            long packageGroupId)
        {
            int sizeOfStructureInBytes = structureInBytes.Length;
            int packageSize = sizeOfStructureInBytes + countOfExtraBytesToMatchPackageSize;

            byte[] bytesFilledWithZerosAtEnd = new byte[packageSize];
            int countOfPackages = bytesFilledWithZerosAtEnd.Length / _DataPackageDataSizeInByte;

            Array.Copy(structureInBytes, bytesFilledWithZerosAtEnd, sizeOfStructureInBytes);

            int sizeOfExtraBytes = packageSize - sizeOfStructureInBytes;

            DataPackage[] dataPackages = new DataPackage[packageSize / _DataPackageDataSizeInByte];
            bool isInLastLoopCircle;

            for (int offset = 0, packageIndex = 0; offset < bytesFilledWithZerosAtEnd.Length; offset += _DataPackageDataSizeInByte, packageIndex++)
            {
                isInLastLoopCircle = (offset + _DataPackageDataSizeInByte) == bytesFilledWithZerosAtEnd.Length;
                byte[] dataBlock = new byte[_DataPackageDataSizeInByte];

                Array.Copy(bytesFilledWithZerosAtEnd, offset, dataBlock, 0, _DataPackageDataSizeInByte);

                int countOfExtraBytesToIgnore = isInLastLoopCircle ? sizeOfExtraBytes : 0;

                dataPackages[packageIndex] = new DataPackage(
                    dataBlock,
                    countOfExtraBytesToIgnore,
                    countOfPackages,
                    packageIndex + 1,
                    packageGroupId,
                    StructType.LoginData);
            }

            return dataPackages;
        }

        private static T BuildStructure<T>(
            DataPackage[] dataPackages,
            int countOfExtraBytesToIgnore) where T : struct
        {
            byte[] accumulatedBytes = new byte[(dataPackages.Length * _DataPackageDataSizeInByte) - countOfExtraBytesToIgnore];
            for (int i = 0; i < dataPackages.Length - 1; i++)
            {
                byte[] data = dataPackages[i].data;
                Array.Copy(data, 0, accumulatedBytes, i * _DataPackageDataSizeInByte, _DataPackageDataSizeInByte);
            }

            int lastIndex = (dataPackages.Length - 1) * _DataPackageDataSizeInByte;
            Array.Copy(dataPackages.Last().data, 0, accumulatedBytes, lastIndex, countOfExtraBytesToIgnore);
            return BytesToStructure<T>(accumulatedBytes);
        }


        private static T BuildStructure<T>(
           DataPackage[] dataPackages) where T : struct
        {
            byte[] accumulatedBytes = new byte[dataPackages.Length * _DataPackageDataSizeInByte];
            for (int i = 0; i < dataPackages.Length; i++)
            {
                byte[] data = dataPackages[i].data;
                Array.Copy(data, 0, accumulatedBytes, i, _DataPackageDataSizeInByte);
            }

            return BytesToStructure<T>(accumulatedBytes);
        }
    }
}
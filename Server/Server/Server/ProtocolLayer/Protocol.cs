using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;
using Server.ProtocolLayer.DataStructs;

namespace Server.ProtocolLayer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct DataPackage
    {
        public int countOfExtraBytesToIgnore;
        public int countOfPackagesInGroup;
        public int sequenceNumber;
        public long dataPackageGroupId;
        public StructTypes structType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ProtocolLayerFunctions._DataPackageDataSizeInByte)]
        public byte[] data;

        public DataPackage(
            byte[] data,
            int countOfExtraBytesToIgnore,
            int countOfPackagesInGroup,
            int sequenceNumber,
            long dataPackageGroupId,
            StructTypes structType)
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
        public DataPackage[] dataPackages;
        public int countOfMaximalPackagesInGroup;
        private int countOfInsertedPackagesInGroup = 0;

        public PackageGroupBuffer(int countOfPackagesInGroup, DataPackage firstPackage)
        {
            dataPackages = new DataPackage[countOfPackagesInGroup];
            countOfMaximalPackagesInGroup = countOfPackagesInGroup;
            int packageInsertionIndex = firstPackage.sequenceNumber - 1;
            dataPackages[packageInsertionIndex] = firstPackage;
            countOfInsertedPackagesInGroup = 1;
        }

        public void InsertDataPackage(DataPackage dataPackage)
        {
            if (countOfInsertedPackagesInGroup < dataPackages.Length)
            {
                int packageInsertionIndex = dataPackage.sequenceNumber - 1;
                dataPackages[packageInsertionIndex] = dataPackage;
                countOfInsertedPackagesInGroup++;
            }
        }

        public bool IsBufferComplete()
        {
            return countOfMaximalPackagesInGroup == countOfInsertedPackagesInGroup;
        }
    }

    public static class ProtocolLayerFunctions
    {
        public const int _DataPackageDataSizeInByte = 64;

        public static void Test()
        {
            LoginData loginDataToSend = new LoginData("Bob1337", "sudo");
            byte[][] dataPackageByteSequence = ConvertStructDataToByteSequence(loginDataToSend);
            //// Bytes Senden ---->

            //// Bytes Empfangen <----
            PackageBundler packageBundler = new PackageBundler();
            for (int i = 0; i < dataPackageByteSequence.Length; i++)
            {
                PackageBundle? dataPackageBundle = packageBundler.InsertAndRetrieveCompletePackages(dataPackageByteSequence[i]);

                if (dataPackageBundle != null)
                {
                    // MessageDispatcher, !TypeConverter!
                    switch (dataPackageBundle.StructType)
                    {
                        case StructTypes.LoginData:
                            LoginData loginDataToRetrieve = ConvertDataBundleToStruct<LoginData>(dataPackageBundle);
                            Console.WriteLine(loginDataToRetrieve.username);
                            Console.WriteLine(loginDataToRetrieve.password);
                            break;
                    }
                }
            }
        }

        public static byte[][] ConvertStructDataToByteSequence<T>(T structData) where T : struct
        {
            DataPackage[] dataPackages = ConvertStructDataToDataPackages(structData);
            byte[][] dataPackageByteSequence = ConvertDataPackagesToByteSequence(dataPackages);
            return dataPackageByteSequence;
        }

        public static T ConvertBytesToStructure<T>(byte[] bytes) where T : struct
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

        private static byte[][] ConvertDataPackagesToByteSequence(DataPackage[] dataPackages)
        {
            byte[][] dataPackageByteSequence = new byte[dataPackages.Length][];
            for (int i = 0; i < dataPackages.Length; i++)
            {
                dataPackageByteSequence[i] = ConvertStructureToBytes(dataPackages[i]);
            }
            return dataPackageByteSequence;
        }

        private static DataPackage[] ConvertStructDataToDataPackages<T>(T dataStruct) where T : struct
        {
            byte[] structureInBytes = ConvertStructureToBytes(dataStruct);
            int sizeOfStructureInBytes = structureInBytes.Length;
            int countOfExtraBytesToMatchPackageSize = (_DataPackageDataSizeInByte - (sizeOfStructureInBytes % _DataPackageDataSizeInByte)) % _DataPackageDataSizeInByte;
            long packageGroupId = DateTime.Now.Ticks;

            return BuildDataPackagesWithMatchedByteSize(structureInBytes, countOfExtraBytesToMatchPackageSize, packageGroupId);
        }

        private static T ConvertDataBundleToStruct<T>(PackageBundle dataBundle) where T : struct
        {
            DataPackage[] dataPackages = dataBundle.DataPackages;
            int extraBytesToIgnore = dataPackages.Last().countOfExtraBytesToIgnore;

            if (extraBytesToIgnore != 0)
            {
                // Falls Übertragungspackete kleiner oder größer sind als das Gesamtpacket für Struct:
                return BuildStructure<T>(dataPackages, extraBytesToIgnore);
            }

            // Falls Übertragungspacket genau so groß wie das Gesamtpacket für Struct ist:
            return BuildStructure<T>(dataPackages);
        }

        private static byte[] ConvertStructureToBytes<T>(T structure) where T : struct
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

        private static DataPackage[] BuildDataPackagesWithMatchedByteSize(
            byte[] structureInBytes,
            int countOfExtraBytesToMatchPackageSize,
            long packageGroupId)
        {
            Console.WriteLine("Auseinandernehmen extraBytes: " + countOfExtraBytesToMatchPackageSize);

            int sizeOfStructureInBytes = structureInBytes.Length;
            byte[] bytesFilledWithZerosAtEnd = new byte[structureInBytes.Length + countOfExtraBytesToMatchPackageSize];
            int countOfPackages = bytesFilledWithZerosAtEnd.Length / _DataPackageDataSizeInByte;

            Array.Copy(structureInBytes, bytesFilledWithZerosAtEnd, sizeOfStructureInBytes);

            DataPackage[] dataPackages = new DataPackage[countOfPackages];
            bool isInLastLoopCircle;

            for (int offset = 0, packageIndex = 0; offset < bytesFilledWithZerosAtEnd.Length; offset += _DataPackageDataSizeInByte, packageIndex++)
            {
                isInLastLoopCircle = (offset + _DataPackageDataSizeInByte) == bytesFilledWithZerosAtEnd.Length;
                byte[] dataBlock = new byte[_DataPackageDataSizeInByte];

                Array.Copy(bytesFilledWithZerosAtEnd, offset, dataBlock, 0, _DataPackageDataSizeInByte);

                int countOfExtraBytesToIgnore = isInLastLoopCircle ? countOfExtraBytesToMatchPackageSize : 0;

                dataPackages[packageIndex] = new DataPackage(
                    dataBlock,
                    countOfExtraBytesToIgnore,
                    countOfPackages,
                    packageIndex + 1,
                    packageGroupId,
                    StructTypes.LoginData);
            }

            return dataPackages;
        }

        private static T BuildStructure<T>(
            DataPackage[] dataPackages,
            int countOfExtraBytesToIgnore) where T : struct
        {
            Console.WriteLine("Zusammenbauen extraBytes: " + countOfExtraBytesToIgnore);

            byte[] accumulatedBytes = new byte[(dataPackages.Length * _DataPackageDataSizeInByte) - countOfExtraBytesToIgnore];

            // Falls Übertragungspackete kleiner sind als das Gesamtpacket für Struct:
            if (_DataPackageDataSizeInByte < accumulatedBytes.Length)
            {
                for (int i = 0; i < dataPackages.Length - 1; i++)
                {
                    byte[] data = dataPackages[i].data;
                    Array.Copy(data, 0, accumulatedBytes, i * _DataPackageDataSizeInByte, _DataPackageDataSizeInByte);
                }

                int lastIndex = (dataPackages.Length - 1) * _DataPackageDataSizeInByte;
                Array.Copy(dataPackages.Last().data, 0, accumulatedBytes, lastIndex, countOfExtraBytesToIgnore);
                return ConvertBytesToStructure<T>(accumulatedBytes);
            }

            // Falls Übertragungspacket größer als das Gesamtpacket für Struct ist:
            Array.Copy(dataPackages[0].data, 0, accumulatedBytes, 0, accumulatedBytes.Length);
            return ConvertBytesToStructure<T>(accumulatedBytes);
        }


        private static T BuildStructure<T>(
           DataPackage[] dataPackages) where T : struct
        {
            Console.WriteLine("Zusammenbauen extraBytes: " + 0);
            byte[] accumulatedBytes = new byte[dataPackages.Length * _DataPackageDataSizeInByte];
            for (int i = 0; i < dataPackages.Length; i++)
            {
                byte[] data = dataPackages[i].data;
                Array.Copy(data, 0, accumulatedBytes, i * _DataPackageDataSizeInByte, _DataPackageDataSizeInByte);
            }

            return ConvertBytesToStructure<T>(accumulatedBytes);
        }
    }
}
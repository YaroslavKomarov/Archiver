using System.Collections.Generic;
using Archiver.Domain.Interfaces;
using Archiver.Infrastructure;

namespace Archiver.Domain.Models.ArchivesFiles
{
    public class ArchiveFile
    {
        public Dictionary<string, byte[]> AccessoryData { get; }
        public string InitExtension { get; }
        public FileHandler FHandler { get; }

        public ArchiveFile(string initExtension, Dictionary<string, byte[]> accessoryData, FileHandler fHandler)
        {
            InitExtension = initExtension;
            AccessoryData = accessoryData;
            FHandler = fHandler;
        }

        public static void WriteInitFile(FileHandler fHandler, IArchiverBase rightImplementation)
        {
            var archiveFile = new ArchiveFile(fHandler);
            rightImplementation.AccessoryData = archiveFile.AccessoryData;
            foreach (var compressedBytes in fHandler.TryReadCompressedBytesFromReader())
            {
                var formatCompressedBytes = ToRightFormatConverter.RemoveZerosFromBytesCompressedData(compressedBytes);
                var decompressedBytes = rightImplementation.DecompressData(formatCompressedBytes);
                fHandler.TryWriteBytesPortion(decompressedBytes);
            }
        }

        public void WriteArchiveFile(IArchiverBase rightImplementation)
        {
            FHandler.InitializeWriter(rightImplementation.AlgorithmExtension);
            WriteInitExtension();
            var count = 0;
            foreach (var bytes in FHandler.TryReadBytesInPortions())
            {
                var compressedBytes = rightImplementation.CompressData(bytes);
                if (count++ == 0)
                    WriteAccessoryData();
                WriteCompressedDataPortion(compressedBytes);
            }
        }

        private ArchiveFile(FileHandler fHandler)
        {
            FHandler = fHandler;
            FHandler.InitializeReader();
            while (true)
            {
                if (InitExtension == null)
                {
                    var bytes = fHandler.TryReadAccessoryDataFromReader();
                    var formatBytes = ToRightFormatConverter.RemoveZerosFromBytesData(bytes);
                    InitExtension = ToRightFormatConverter.GetStringFromBytes(bytes);
                }
                else if (AccessoryData == null)
                {
                    var bytes = fHandler.TryReadAccessoryDataFromReader();
                    var formatBytes = ToRightFormatConverter.RemoveZerosFromBytesData(bytes);
                    AccessoryData = ToRightFormatConverter.ConvertAccessoryDataToDictionary(bytes);
                }
                else break;
            }
            FHandler.InitializeWriter(InitExtension);
        }

        private void WriteInitExtension()
        {
            var extensionBytes = ToRightFormatConverter.GetBytesFromString(InitExtension);
            var initExtensionBytes = ToRightFormatConverter.GetBytesWithZerosDataSeparator(extensionBytes);
            FHandler.TryWriteBytesPortion(initExtensionBytes);
            FHandler.TryWriteBytesPortion(ToRightFormatConverter.DataSeparator);
        }

        private void WriteAccessoryData()
        {
            var accessoryDataBytes = ToRightFormatConverter.ConvertAccessoryDictToByteArray(AccessoryData);
            FHandler.TryWriteBytesPortion(accessoryDataBytes);
            FHandler.TryWriteBytesPortion(ToRightFormatConverter.DataSeparator);
        }

        private void WriteCompressedDataPortion(byte[] compressedBytes)
        {
            var formatCompressedBytes = ToRightFormatConverter.GetBytesWithZerosCompressedDataSep(compressedBytes);
            FHandler.TryWriteBytesPortion(formatCompressedBytes);
            FHandler.TryWriteBytesPortion(ToRightFormatConverter.CompressedDataSeparator);
        }
    }
}
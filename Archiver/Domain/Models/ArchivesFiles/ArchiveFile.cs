using System.Collections.Generic;
using Archiver.Domain.Interfaces;
using Archiver.Infrastructure;

namespace Archiver.Domain.Models.ArchivesFiles
{
    public class ArchiveFile
    {
        public Dictionary<string, byte[]> AccessoryData { get; }
        public string InitExtension { get; }

        public ArchiveFile(string initExtension, Dictionary<string, byte[]> accessoryData)
        {
            InitExtension = initExtension;
            AccessoryData = accessoryData;
        }

        public ArchiveFile(FileHandler fHandler)
        {
            while (true)
            {
                if (InitExtension == null)
                {
                    var compressedBytes = fHandler.TryReadAccessoryDataFromReader();
                    InitExtension = ToRightFormatConverter.GetStringFromBytes(compressedBytes);
                }
                else if (AccessoryData == null)
                {
                    var compressedBytes = fHandler.TryReadAccessoryDataFromReader();
                    AccessoryData = ToRightFormatConverter.ConvertAccessoryDataToDictionary(compressedBytes);
                }
                else break;
            }
        }

        public static void WriteInitFile(FileHandler fHandler, IArchiverBase rightImplementation)
        {
            var archiveFile = new ArchiveFile(fHandler);
            rightImplementation.AccessoryData = archiveFile.AccessoryData;
            fHandler.InitializeCompressedWriter(archiveFile.InitExtension);
            foreach (var compressedBytes in fHandler.TryReadCompressedBytesFromReader())
            {
                var formatCompressedBytes = ToRightFormatConverter.RemoveInsignificantZerosFromBytes(compressedBytes);
                var decompressedBytes = rightImplementation.DecompressData(formatCompressedBytes);
                fHandler.TryWriteBytesPortion(decompressedBytes);
            }
        }

        public void WriteArchiveFile(FileHandler fHandler, IArchiverBase rightImplementation)
        {
            var initExtensionBytes = ToRightFormatConverter.GetRightFomatExtension(InitExtension);
            fHandler.TryWriteBytesPortion(initExtensionBytes);
            var count = 0;
            foreach (var bytes in fHandler.TryReadBytesInPortions())
            {
                var compressedBytes = rightImplementation.CompressData(bytes);
                if (count++ == 0)
                {
                    var accessoryDataBytes = ToRightFormatConverter.ConvertAccessoryDictToByteArray(AccessoryData);
                    fHandler.TryWriteBytesPortion(accessoryDataBytes);
                }
                var formatCompressedBytes = ToRightFormatConverter.GetBytesWithInsignificantZeros(compressedBytes);
                fHandler.TryWriteBytesPortion(formatCompressedBytes);
            }
            fHandler.TryWriteBytesPortion(ToRightFormatConverter.DataSeparator);
        }
    }
}
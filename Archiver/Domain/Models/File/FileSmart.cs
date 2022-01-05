using System.Collections.Generic;
using System.Reflection;
using Archiver.Infrastructure;

namespace Archiver.Domain.Models.File
{
    public class FileSmart
    {
        public byte[] accecoryData { get; }
        public IEnumerable<byte[]> compressedData { get; }
        public byte[] initExtensionBytes { get; }
        public byte[] algExtensionBytes { get; }

        public FileSmart(string initExtension, string algExtension, IEnumerable<byte[]> compressedData, Dictionary<string, byte[]> accecoryData)
        {
            this.initExtensionBytes = ToRightFormatConverter.GetBytesFromString(initExtension);
            this.algExtensionBytes = ToRightFormatConverter.GetBytesFromString(algExtension);
            this.accecoryData = ToRightFormatConverter.ConvertAccessoryDictToByteArray(accecoryData);
            this.compressedData = compressedData;
        }

        public FileSmart(string path)
        {
            var compressedReader = new CompressedDataReader(path);
            while (true)
            {
                if (initExtensionBytes == null)
                    initExtensionBytes = compressedReader.GetBytesFromStream();
                else if (algExtensionBytes == null)
                    algExtensionBytes = compressedReader.GetBytesFromStream();
                else if (compressedData == null)
                    compressedData = compressedReader.GetCompressedBytesFromStream();
                else if (accecoryData == null)
                    accecoryData = compressedReader.GetBytesFromStream();
                else break;
            }
        }

        public void WriteSmartFile(FileHandler fHandler)
        {
            var algExtensionStr = ToRightFormatConverter.GetStringFromBytes(algExtensionBytes);
            fHandler.TryWriteDecompressedBytesInPortions(GetEnumerationOfByteArrays(), algExtensionStr);
        }

        private IEnumerable<byte[]> GetEnumerationOfByteArrays()
        {
            for (var i = 0; i < 4; i++)
            {
                if (i == 0)
                    yield return ToRightFormatConverter.GetBytesWithInsignificantZeros(initExtensionBytes);
                else if (i == 1)
                    yield return ToRightFormatConverter.GetBytesWithInsignificantZeros(algExtensionBytes);
                else if (i == 2)
                    foreach (var bytes in compressedData)
                        yield return ToRightFormatConverter.GetBytesWithInsignificantZeros(bytes);
                else
                    yield return accecoryData;
                yield return ToRightFormatConverter.DataSeparator;
            }
        }
    }
}
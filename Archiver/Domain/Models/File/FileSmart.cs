using System.IO;
using Archiver.Infrastructure;

namespace Archiver.Domain.Models.File
{
    public class FileSmart
    {
        public byte[] accecoryData { get; }
        public byte[] compressedData { get; }
        public string algExtension { get; }
        public string initExtension { get; }

        public FileSmart(string initExtension, string algExtension, byte[] compressedData, byte[] accecoryData)
        {
            this.initExtension = initExtension;
            this.algExtension = algExtension;
            this.compressedData = compressedData;
            this.accecoryData = accecoryData;
        }

        public FileSmart(byte[] bytes)
        {
            var bytesList = ConverterToFormat.GetByteArraysFromByteData(bytes);
            if (bytesList.Count != 4)
                throw new FileFormatException();
            initExtension = ConverterToFormat.GetStringFromBytes(bytesList[0]);
            algExtension = ConverterToFormat.GetStringFromBytes(bytesList[1]);
            compressedData = bytesList[2];
            accecoryData = bytesList[3];
        }

        public void WriteSmartFile(FileHandler fHandler)
        {
            var bytes = GetByteArrayFromFields();
            fHandler.TryWriteAllBytes(bytes, algExtension);
        }

        private byte[] GetByteArrayFromFields()
        {
            var initExtensionBytes = ConverterToFormat.GetBytesFromString(initExtension);
            var newExtensionBytes = ConverterToFormat.GetBytesFromString(algExtension);
            return ConverterToFormat.CollectDataIntoByteArray(initExtensionBytes, newExtensionBytes, compressedData, accecoryData);
        }
    }
}

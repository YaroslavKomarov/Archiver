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
            var bytesList = ToRightFormatConverter.GetByteArraysFromByteData(bytes);
            if (bytesList.Count != 4)
                throw new FileFormatException();
            initExtension = ToRightFormatConverter.GetStringFromBytes(bytesList[0]);
            algExtension = ToRightFormatConverter.GetStringFromBytes(bytesList[1]);
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
            var initExtensionBytes = ToRightFormatConverter.GetBytesFromString(initExtension);
            var newExtensionBytes = ToRightFormatConverter.GetBytesFromString(algExtension);
            return ToRightFormatConverter.CollectDataIntoByteArray(initExtensionBytes, newExtensionBytes, compressedData, accecoryData);
        }
    }
}

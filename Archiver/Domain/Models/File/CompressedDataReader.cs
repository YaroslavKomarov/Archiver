using Archiver.Infrastructure;
using System.Collections.Generic;
using System.IO;

namespace Archiver.Domain.Models.File
{
    public class CompressedDataReader
    {
        public CompressedDataReader(string path)
        {
            this.bufferSize = FileHandler.BufferSize;
            this.iterationCount = FileSmart.PublicPropertiesCount;
            this.underlyingStream = new BufferedStream(new FileStream(path, FileMode.Open, FileAccess.Read), bufferSize);
        }

        public IEnumerable<byte[]> ReadBytesInPortions()
        {
            for (var i = 0; i < iterationCount; i++)
            {
                if (i == 0)
                    yield return GetBytesFromStream();
                else if (i == 1)
                    yield return GetBytesFromStream();
                else if (i == 2)
                    foreach (var bytes in GetCompressedBytesFromStream())
                        yield return bytes;
                else
                    yield return GetBytesFromStream();
            }
        }

        public byte[] GetBytesFromStream()
        {
            var currentByte = underlyingStream.ReadByte();
            var lstBytes = new List<byte>();

            while (currentByte != -1)
            {
                var nextByte = underlyingStream.ReadByte();
                if (lstBytes.Count > 0)
                    if (lstBytes[lstBytes.Count - 1] != ToRightFormatConverter.DataSeparator[0]
                        && currentByte == ToRightFormatConverter.DataSeparator[0]
                        && nextByte == ToRightFormatConverter.DataSeparator[1])
                        break;
                lstBytes.Add((byte)currentByte);
                currentByte = nextByte;
            }
            return lstBytes.ToArray();
        }

        public IEnumerable<byte[]> GetCompressedBytesFromStream()
        {
            var currentByte = underlyingStream.ReadByte();
            var lstBytes = new List<byte>();

            while (currentByte != -1)
            {
                var nextByte = underlyingStream.ReadByte();
                if (lstBytes.Count == bufferSize)
                {
                    yield return lstBytes.ToArray();
                    lstBytes = new List<byte>();
                }
                if (lstBytes.Count > 0)
                    if (lstBytes[lstBytes.Count - 1] != ToRightFormatConverter.DataSeparator[0]
                        && currentByte == ToRightFormatConverter.DataSeparator[0]
                        && nextByte == ToRightFormatConverter.DataSeparator[1])
                        break;
                lstBytes.Add((byte)currentByte);
                currentByte = nextByte;
            }
        }

        private BufferedStream underlyingStream;
        private int iterationCount;
        private int bufferSize;
    }
}

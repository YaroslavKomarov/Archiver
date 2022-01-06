using Archiver.Domain.Models.File;
using System;
using System.Collections.Generic;
using System.IO;

namespace Archiver.Infrastructure
{
    public class CompressedDataReader : IDisposable
    {
        public CompressedDataReader(string path)
        {
            this.bufferSize = FileHandler.BufferSize;
            this.underlyingStream = new BufferedStream(new FileStream(path, FileMode.Open, FileAccess.Read), bufferSize);
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
            yield return lstBytes.ToArray();
        }

        private BufferedStream underlyingStream;
        private int bufferSize;
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    underlyingStream.Close();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

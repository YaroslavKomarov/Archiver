using System;
using System.Collections.Generic;
using System.IO;

namespace Archiver.Infrastructure
{
    public class DataReader : IDisposable
    {
        public DataReader(string path, int bufferSize)
        {
            this.underlyingStream = new BufferedStream(File.Open(path, FileMode.Open, FileAccess.Read), bufferSize);
        }

        public byte[] GetBytesFromStreamUntilDataSep()
        {
            var currentByte = underlyingStream.ReadByte();
            var lstBytes = new List<byte>();

            while (currentByte != -1)
            {
                var nextByte = underlyingStream.ReadByte();
                if (IsSeparatorFound(lstBytes, currentByte, nextByte, false))
                    break;
                lstBytes.Add((byte)currentByte);
                currentByte = nextByte;
            }
            return lstBytes.ToArray();
        }

        public IEnumerable<byte[]> GetBytesFromStreamUntilCompressedDataSep()
        {
            var currentByte = underlyingStream.ReadByte();
            var lstBytes = new List<byte>();

            while (currentByte != -1)
            {
                var nextByte = underlyingStream.ReadByte();
                if (IsSeparatorFound(lstBytes, currentByte, nextByte, true))
                {
                    yield return lstBytes.ToArray();
                    lstBytes = new List<byte>();
                    currentByte = underlyingStream.ReadByte();
                    continue;
                }
                lstBytes.Add((byte)currentByte);
                currentByte = nextByte;
            }
            if (lstBytes.Count > 0)
                yield return lstBytes.ToArray();
        }

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

        private bool IsSeparatorFound(List<byte> lstBytes, int currentByte, int nextByte, bool isCompressed)
        {
            var separator = isCompressed ? ToRightFormatConverter.CompressedDataSeparator : ToRightFormatConverter.DataSeparator;
            return lstBytes.Count > 0
                && lstBytes[lstBytes.Count - 1] != separator[0]
                && currentByte == separator[0]
                && nextByte == separator[1];
        }

        private BufferedStream underlyingStream;
        private bool disposedValue;
    }
}

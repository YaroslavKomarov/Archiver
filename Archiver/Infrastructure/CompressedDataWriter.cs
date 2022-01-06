using Archiver.Domain.Models.File;
using System;
using System.IO;

namespace Archiver.Infrastructure
{
    public class CompressedDataWriter : IDisposable
    {
        public CompressedDataWriter(string path)
        {
            this.bufferSize = FileHandler.BufferSize;
            this.underlyingStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        }

        public void WriteBytesInPortions(byte[] bytes)
        {
            underlyingStream.Write(bytes, 0, bytes.Length);
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

        private FileStream underlyingStream;
        private int bufferSize;
        private bool disposedValue;
    }
}

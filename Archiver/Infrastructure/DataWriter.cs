using System;
using System.IO;
using System.Text;

namespace Archiver.Infrastructure
{
    public class DataWriter : IDisposable
    {
        public DataWriter(string path)
        {
            underlyingStream = File.Open(path, FileMode.Create, FileAccess.Write);
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
        private bool disposedValue;
    }
}

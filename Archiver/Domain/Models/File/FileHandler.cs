using Archiver.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;

namespace Archiver.Domain.Models.File
{
    public class FileHandler : IDisposable
    {
        public static readonly int BufferSize = 10 * 1024 * 1024;
        public string PathFrom { get; }
        public string PathTo { get; }

        public FileHandler(string pathFrom, string pathTo, string extension, bool isCompressed)
        {
            PathFrom = pathFrom;
            PathTo = pathTo;

            if (isCompressed)
                compressedWriter = new CompressedDataWriter(GetRightPathToFile(extension));
            else
                compressedReader = new CompressedDataReader(PathFrom);
        }

        public void InitializeCompressedWriter(string extension)
        {
            compressedWriter = new CompressedDataWriter(GetRightPathToFile(extension));
        }

        public IEnumerable<byte[]> TryReadBytesInPortions()
        {
            using (var fs = new FileStream(PathFrom, FileMode.Open, FileAccess.Read))
            using (var bs = new BufferedStream(fs, BufferSize))
            {
                var buffer = new byte[BufferSize];
                var readBytes = 0;
                while ((readBytes = bs.Read(buffer, 0, BufferSize)) > 0)
                {
                    yield return buffer;
                }
            }
        }

        public void TryWriteBytesPortion(byte[] bytes)
        {
            compressedWriter.WriteBytesInPortions(bytes);
        }

        public byte[] TryReadAccessoryDataFromReader()
        {
            return compressedReader.GetBytesFromStream();
        }

        public IEnumerable<byte[]> TryReadCompressedBytesFromReader()
        {

            return compressedReader.GetCompressedBytesFromStream();
        }

        public static void CheckPathExtension(string path, Dictionary<string, string> extensionDict)
        {
            var extension = GetFileExtensionFromPath(path);
            if (!extensionDict.ContainsKey(extension))
                throw new FileFormatException($"Передан файл с неверным расширением: {path}");
        }

        public static string GetFileExtensionFromPath(string path)
        {
            return Path.GetExtension(path);
        }

        private string GetRightPathToFile(string extension)
        {
            var fileName = Path.GetFileNameWithoutExtension(PathFrom) + extension;
            return PathTo + $"\\{fileName}";
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (compressedReader != null) compressedReader.Dispose();
                    if (compressedWriter != null) compressedWriter.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private CompressedDataReader compressedReader;
        private CompressedDataWriter compressedWriter;
        private bool disposedValue;
    }
}
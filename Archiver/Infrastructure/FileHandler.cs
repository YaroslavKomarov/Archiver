using System;
using System.Collections.Generic;
using System.IO;

namespace Archiver.Infrastructure
{
    public class FileHandler
    {
        public static readonly int BufferSize = 10 * 1024 * 1024;

        public FileHandler(string pathFrom, string pathTo)
        {
            this.pathFrom = pathFrom;
            this.pathTo = pathTo;
        }

        public void TryWriteBytesInPortions(IEnumerable<byte[]> allData)
        {
            using (var fs = new FileStream(pathFrom, FileMode.Create, FileAccess.Write))
            using (var bs = new BufferedStream(fs, BufferSize))
            {
                foreach (var bytes in allData)
                {
                    var buffer = new byte[BufferSize];
                    Array.Copy(bytes, buffer, bytes.Length);
                    bs.Write(buffer, 0, bytes.Length);
                }
            }
        }

        public IEnumerable<byte[]> TryReadCompressedDataBytes(Dictionary<string, string> extensions)
        {   
            foreach (var path in Directory.GetFiles(pathFrom))
            {
                ThrowExceptIfUnidentifiedExtension(path, extensions);
                yield return File.ReadAllBytes(path);
            }
        }

        public IEnumerable<byte[]> TryReadAllBytesInPortions()
        {
            using (var fs = new FileStream(pathFrom, FileMode.Open, FileAccess.Read))
            using (var bs = new BufferedStream(fs, BufferSize))
            {
                var buffer = new byte[BufferSize];
                var offset = 0;
                var readBytes = 0;
                while ((readBytes = bs.Read(buffer, offset, BufferSize)) > 0)
                {
                    yield return buffer;
                    offset += readBytes;
                }
            }
        }

        public void TryWriteAllBytes(byte[] bytes, string newExtension)
        {
            var compressedFileName = Path.GetFileNameWithoutExtension(pathFrom);
            var pathToNewDirectory = GetPathToNewDirectory(compressedFileName);
            var path = pathToNewDirectory + $"\\{compressedFileName}_{filesCount++}" + newExtension;

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var bs = new BufferedStream(fs, BufferSize))
            {
                var buffer = new byte[BufferSize];
                Array.Copy(bytes, buffer, bytes.Length);
                bs.Write(buffer, 0, bytes.Length);
            }
        }

        public static string GetFileExtensionFromPath(string path)
        {
            return Path.GetExtension(path);
        }

        private void ThrowExceptIfUnidentifiedExtension(string path, Dictionary<string, string> extensions)
        {
            if (!extensions.ContainsKey(GetFileExtensionFromPath(path)))
                throw new FileFormatException($"Неверное расширение файла: {path}");
        }

        private string GetPathToNewDirectory(string compressedFileName)
        {
            var pathToNewDirectory = pathTo + $"\\{compressedFileName}_Archive";
            Directory.CreateDirectory(pathToNewDirectory);
            return pathToNewDirectory;
        }

        private readonly string pathFrom;
        private readonly string pathTo;
        private int filesCount;
    }
}

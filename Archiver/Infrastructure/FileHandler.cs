using System;
using System.Collections.Generic;
using System.IO;

namespace Archiver.Infrastructure
{
    public class FileHandler
    {
        public static readonly int BufferSize = 10 * 1024 * 1024;

        public string PathFrom { get; }
        public string PathTo { get; }

        public FileHandler(string pathFrom, string pathTo)
        {
            this.PathFrom = pathFrom;
            this.PathTo = pathTo;
        }

        public IEnumerable<byte[]> TryReadBytesInPortions()
        {
            using (var fs = new FileStream(PathFrom, FileMode.Open, FileAccess.Read))
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

        public void TryWriteDecompressedBytesInPortions(IEnumerable<byte[]> allData, string newExtension)
        {
            var path = Path.GetFileNameWithoutExtension(PathFrom) + newExtension;
            TryWriteBytesInPortions(allData, path);
        }

        public void TryWriteBytesInPortions(IEnumerable<byte[]> allData, string path = "")
        {
            path = path == "" ? PathTo : path;
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var bs = new BufferedStream(fs, BufferSize))
            {
                foreach (var bytes in allData)
                {
                    var buffer = new byte[bytes.Length];
                    Array.Copy(bytes, buffer, bytes.Length);
                    bs.Write(buffer, 0, bytes.Length);
                }
            }
        }

        public void CheckPathExtension(Dictionary<string, string> extensionDict)
        {
            var extension = GetFileExtensionFromPath(PathFrom);
            if (!extensionDict.ContainsKey(extension))
                throw new FileFormatException($"Передан файл с неверным расширением: {PathFrom}");
        }

        public static string GetFileExtensionFromPath(string path)
        {
            return Path.GetExtension(path);
        }
    }
}
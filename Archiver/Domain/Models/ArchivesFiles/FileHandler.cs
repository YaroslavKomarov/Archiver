using Archiver.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ude;

namespace Archiver.Domain.Models.ArchivesFiles
{
    public class FileHandler : IDisposable
    {
        public static readonly int BufferSize = 4 * 1024 * 1024;
        public string PathFrom { get; }
        public string PathTo { get; }

        public FileHandler(string pathFrom, string pathTo)
        {
            if (pathFrom == null || pathTo == null) 
                throw new ArgumentNullException("Переданный путь не может быть пустым!");
            CheckPathIsDirectory(pathTo);
            CheckPathIsFile(pathFrom);
            CheckPathFromNotEmptyFile(pathFrom);
            PathFrom = pathFrom;
            PathTo = pathTo;
        }

        public void InitializeWriter(string extension)
        {
            writer = new DataWriter(GetRightPathToFile(extension));
        }

        public void InitializeReader()
        {
            reader = new DataReader(PathFrom, BufferSize);
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
                    yield return buffer.Take(readBytes).ToArray();
                }
            }
        }

        public void TryWriteBytesPortion(byte[] bytes)
        {
            writer.WriteBytesInPortions(bytes);
        }

        public byte[] TryReadAccessoryDataFromReader()
        {
            return reader.GetBytesFromStreamUntilDataSep();
        }

        public IEnumerable<byte[]> TryReadCompressedBytesFromReader()
        {
            return reader.GetBytesFromStreamUntilCompressedDataSep();
        }

        public static Encoding GetFileEncoding(string path)
        {
            var buffer = new byte[12000];
            var readBytes = 0;
            using (var fs = File.OpenRead(path))
            {
                readBytes = fs.Read(buffer, 0, buffer.Length);
            }

            var detector = new CharsetDetector();
            detector.Feed(buffer, 0, readBytes);
            detector.DataEnd();
            return Encoding.GetEncoding(detector.Charset);
        }

        public static void CheckPathExtension(string path, Dictionary<string, string> extensionDict)
        {
            var extension = GetFileExtensionFromPath(path);
            if (!extensionDict.ContainsKey(extension))
                throw new FileFormatException($"Переданрый файл ( путь: {path} ) имеет недопустимое расширение");
        }

        public static string GetFileExtensionFromPath(string path)
        {
            return Path.GetExtension(path);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (reader != null) reader.Dispose();
                    if (writer != null) writer.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private static void CheckPathIsDirectory(string path)
        {
            var attr = File.GetAttributes(path);

            if (!attr.HasFlag(FileAttributes.Directory))
                throw new DirectoryNotFoundException($"Конечный путь ( путь: {path} ) должен вести к каталогу");
        }

        private static void CheckPathIsFile(string path)
        {
            var attr = File.GetAttributes(path);

            if (attr.HasFlag(FileAttributes.Directory))
                throw new FileFormatException($"Начальный путь ( путь: {path} ) должен вести к сжимаемому файлу");
        }

        public static void CheckPathFromNotEmptyFile(string path)
        {
            using (var fs = File.OpenRead(path))
                if (fs.Length == 0)
                    throw new EndOfStreamException($"Попытка сжатия пустого файла ( путь: {path} )");
        }

        private string GetRightPathToFile(string extension)
        {
            var fileName = Path.GetFileNameWithoutExtension(PathFrom) + extension;
            return PathTo + $"\\{fileName}";
        }

        private DataReader reader;
        private DataWriter writer;
        private bool disposedValue;
    }
}
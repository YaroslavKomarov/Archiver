using Archiver.Domain.Models.ArchivesFiles;
using NUnit.Framework;
using System.IO;
using System;

namespace ArchiverTest
{
    public class FileHandlerTests
    {
        string pathFrom = @"C:\Users\Yarik\OneDrive\Рабочий стол\флешка\Twin.docx";

        string pathTo = @"C:\Users\Yarik\OneDrive\Рабочий стол\флешка";

        string pathToEmptyFile = @"C:\Users\Yarik\OneDrive\Рабочий стол\флешка\tmp.docx";

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ThrowArgumentExceptionWihtNullPath()
        {
            string pathFrom = null;
            string pathTo = null;

            Assert.Throws<ArgumentNullException>(() => new FileHandler(pathFrom, pathTo));
        }

        [Test]
        public void ThrowArgumentExceptionWihtEmptyPath()
        {
            var pathFrom = "";
            var pathTo = "";

            Assert.Throws<ArgumentException>(() => new FileHandler(pathFrom, pathTo));
        }

        [Test]
        public void ThrowFileNotFoundExceptionWihtInvalidPaths()
        {
            var pathFrom = "invalidPath";
            var pathTo = "invalidPath";

            Assert.Throws<FileNotFoundException>(() => new FileHandler(pathFrom, pathTo));
        }

        [Test]
        public void ThrowArgumentExceptionWihtInvalidPathTo()
        {
            var wrongPathTo = @"C:\Users\Yarik\OneDrive\Рабочий стол\флешка\Twin.docx";
            var extens = FileHandler.GetFileExtensionFromPath(pathFrom);

            Assert.Throws<DirectoryNotFoundException>(() => new FileHandler(pathFrom, wrongPathTo));
        }

        [Test]
        public void ThrowArgumentExceptionWihtInvalidPathFrom()
        {
            var wrongPathFrom = @"C:\Users\Yarik\OneDrive\Рабочий стол\флешка";

            Assert.Throws<FileFormatException>(() => new FileHandler(wrongPathFrom, pathTo));
        }

        [Test]
        public void SizeOfReadDataNotExceedSizeOfBuffer()
        {
            var fHandler = new FileHandler(pathFrom, pathTo);

            foreach (var porion in fHandler.TryReadBytesInPortions())
                Assert.IsTrue(FileHandler.BufferSize >= porion.Length);
            fHandler.Dispose();
        }

        [Test]
        public void ThrowEndOfStreamExceptionWihtEmptyFilePathFrom()
        {
            var pathTo = @"C:\Users\Yarik\OneDrive\Рабочий стол\флешка";

            Assert.Throws<EndOfStreamException>(() => new FileHandler(pathToEmptyFile, pathTo));
        }
    }
}
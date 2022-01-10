using Archiver.Domain.Models.ArchivesFiles;
using Archiver.Domain.Models.Haffman;
using Archiver.Application;
using NUnit.Framework;
using System.IO;
using System;

namespace ArchiverTest
{
    public class ArchiveFileTests
    {
        string pathToInitFile = @"C:\Users\Yarik\OneDrive\Рабочий стол\флешка\Twin.docx";

        string pathToDirectory = @"C:\Users\Yarik\OneDrive\Рабочий стол\флешка";

        string pathToDecompressDirectory = @"C:\Users\Yarik\OneDrive\Рабочий стол";

        string pathToDecompressedInitFile = @"C:\Users\Yarik\OneDrive\Рабочий стол\Twin.docx";

        string pathToArchive = @"C:\Users\Yarik\OneDrive\Рабочий стол\флешка\Twin.huf";

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void CheckExistenceArchiveFile()
        {
            if (File.Exists(pathToArchive)) File.Delete(pathToArchive);
            var extens = FileHandler.GetFileExtensionFromPath(pathToInitFile);
            var fHandler = new FileHandler(pathToInitFile, pathToDirectory);
            var archiver = new HuffmanArchiver();
            var archiveFile = new ArchiveFile(extens, archiver.AccessoryData, fHandler);
            archiveFile.WriteArchiveFile(archiver);
            fHandler.Dispose();

            Assert.IsTrue(File.Exists(pathToArchive));
        }

        [Test]
        public void CheckExistenceInitFile()
        {
            if (File.Exists(pathToDecompressedInitFile)) File.Delete(pathToDecompressedInitFile);
            var fHandler = new FileHandler(pathToArchive, pathToDecompressDirectory);
            var archiver = new HuffmanArchiver();
            ArchiveFile.WriteInitFile(fHandler, archiver);
            fHandler.Dispose();

            Assert.IsTrue(File.Exists(pathToDecompressedInitFile));
            File.Delete(pathToArchive);
        }
    }
}
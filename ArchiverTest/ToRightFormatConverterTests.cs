using Archiver.Infrastructure;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArchiverTest
{
    public class ToRightFormatConverterTests
    {
        string pathToFile = @"C:\Users\Yarik\OneDrive\Рабочий стол\TEST.txt";
        
        string pathToDirectory = @"C:\Users\Yarik\OneDrive\Рабочий стол";

        [Test]
        public void CompareInitialBytesWithBytesAfterAddingZeros()
        {
            var bytes = File.ReadAllBytes(pathToFile);
            var bytesWithZeros = ToRightFormatConverter.GetBytesWithZerosDataSeparator(bytes);
            var bytesWithoutZeros = ToRightFormatConverter.RemoveZerosFromBytesCompressedData(bytesWithZeros);
            CollectionAssert.AreEqual(bytes, bytesWithoutZeros);
        }

        [Test]
        public void CheckReaderBytesUntilDataSeparator()
        {
            var portionLength = 64 * 1024;
            var bytes = GetPortion(portionLength);
            var path = $"{pathToDirectory}\\Tmp.txt";
            var formatBytes = ToRightFormatConverter.GetBytesWithZerosDataSeparator(bytes);
            File.WriteAllBytes(path, formatBytes.Concat(ToRightFormatConverter.DataSeparator).ToArray());

            var reader = new DataReader(path, portionLength);
            var readBytes = reader.GetBytesFromStreamUntilDataSep();
            var readBytesFormat = ToRightFormatConverter.RemoveZerosFromBytesData(readBytes);

            Assert.IsTrue(portionLength == readBytesFormat.Length);
            CollectionAssert.AreEqual(bytes, readBytesFormat);
        }

        [Test]
        public void CheckReaderBytesUntilCompressedDataSeparator()
        {
            var path = $"{pathToDirectory}\\Tmp.txt";
            var portionLength = 64 * 1024;
            var example = new byte[portionLength];
            using (var fs = File.Open(path, FileMode.Create, FileAccess.Write))
            {
                for (var i = 0; i < 4; i++)
                {
                    var bytes = GetPortion(portionLength);
                    var formatBytes = ToRightFormatConverter.GetBytesWithZerosCompressedDataSep(bytes);
                    fs.Write(formatBytes.Concat(ToRightFormatConverter.CompressedDataSeparator).ToArray());
                    example = bytes;
                }
            }

            var reader = new DataReader(path, portionLength);
            foreach (var portion in reader.GetBytesFromStreamUntilCompressedDataSep())
            {
                var formatBytes = ToRightFormatConverter.RemoveZerosFromBytesCompressedData(portion);
                Assert.IsTrue(formatBytes.Length == portionLength);
                CollectionAssert.AreEqual(example, formatBytes);
            }
        }

        private byte[] GetPortion(int portionLength)
        {
            var bytes = new byte[portionLength];
            for (var i = 0; i < portionLength; i++)
                bytes[i] = (byte)(i % 256);
            return bytes;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Archiver.Domain.Models.Lzw;

namespace ArchiverTest
{
    public class LzwArchiverTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ThrowArgumentExceptionTryCompressEmptyArray()
        {
            var bytes = new byte[0];
            var lzwArchiver = new LzwArchiver();
            Assert.Throws<ArgumentException>(() => lzwArchiver.CompressData(bytes));
        }

        [Test]
        public void ThrowArgumentExceptionTryDecompressEmptyArray()
        {
            var bytes = new byte[0];
            var lzwArchiver = new LzwArchiver();
            Assert.Throws<ArgumentException>(() => lzwArchiver.DecompressData(bytes));
        }
    }
}

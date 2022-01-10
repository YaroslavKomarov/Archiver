using Archiver.Domain.Models.Haffman;
using NUnit.Framework;
using System;

namespace ArchiverTest
{
    public class HuffmanArchiverTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ThrowArgumentExceptionTryCompressEmptyArray()
        {
            var bytes = new byte[0];
            var hufArchiver = new HuffmanArchiver();
            Assert.Throws<ArgumentException>(() => hufArchiver.CompressData(bytes));
        }

        [Test]
        public void ThrowArgumentExceptionTryDecompressEmptyArray()
        {
            var bytes = new byte[0];
            var hufArchiver = new HuffmanArchiver();
            Assert.Throws<ArgumentException>(() => hufArchiver.DecompressData(bytes));
        }

        [Test]
        public void HeaderHasFixedLength()
        {
            var headerLen = 260;
            var bytes = new byte[10];
            var hufArchiver = new HuffmanArchiver();
            var compressedBytesWithOnlyHeader = hufArchiver.CompressData(bytes);
            Assert.IsTrue(compressedBytesWithOnlyHeader.Length == headerLen);
        }

        [Test]
        public void ThrowArgumentOutOfRangeExceptionDequeueEmptyPriorityQueue()
        {
            var priorityQueue = new PriorityQueue<int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => priorityQueue.Dequeue());
        }


    }
}
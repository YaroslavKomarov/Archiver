using System;
using System.Collections.Generic;
using Archiver.Domain.Interfaces;

namespace Archiver.Domain.Models
{
    class LzwArchiver : IArchiverBase
    {
        public string AlgorithmExtension => ".lzw";

        public Dictionary<string, byte[]> AccessoryData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<byte[]> CompressData(IEnumerable<byte[]> data)
        {
            throw new NotImplementedException();
        }

        public byte[] DecompressData(byte[] compressedData, Dictionary<string, byte[]> dictionary)
        {
            throw new NotImplementedException();
        }
    }
}

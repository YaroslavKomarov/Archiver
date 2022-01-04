using System;
using System.Collections.Generic;
using Archiver.Domain.Interfaces;

namespace Archiver.Domain.Models
{
    class LzwArchiver : IArchiverBase
    {
        public string AlgorithmExtension => ".lzw";

        public Tuple<byte[], Dictionary<string, byte[]>> CompressData(byte[] byteArray)
        {
            throw new NotImplementedException();
        }

        public byte[] DecompressData(byte[] compressedData, Dictionary<string, byte[]> dictionary)
        {
            throw new NotImplementedException();
        }
    }
}

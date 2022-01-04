using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archiver.Domain.Models.File;
using Archiver.Domain.Interfaces;

namespace Archiver.Domain.Models.Haffman
{
    public class HaffmanArchiver : IArchiverBase
    {
        public string AlgorithmExtension => ".haf";

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

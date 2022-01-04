using System;
using System.Collections.Generic;

namespace Archiver.Domain.Interfaces
{
    public interface IArchiverBase
    {
        string AlgorithmExtension { get; }
        Tuple<byte[], Dictionary<string, byte[]>> CompressData(byte[] byteArray);
        byte[] DecompressData(byte[] compressedData, Dictionary<string, byte[]> dictionary);
    }
}

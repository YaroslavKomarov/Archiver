using System;
using System.Collections.Generic;

namespace Archiver.Domain.Interfaces
{
    public interface IArchiverBase
    {
        string AlgorithmExtension { get; }
        byte[] CompressData(byte[] byteArray);
        byte[] DecompressData(byte[] compressedData, Dictionary<string, byte[]> dictionary);
        Dictionary<string, byte[]> AccessoryData { get; set; }
    }
}

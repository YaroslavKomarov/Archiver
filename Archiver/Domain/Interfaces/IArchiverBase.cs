using System;
using System.Collections.Generic;

namespace Archiver.Domain.Interfaces
{
    public interface IArchiverBase
    {
        string AlgorithmExtension { get; }
        Dictionary<string, byte[]> AccessoryData { get; set; }
        IEnumerable<byte[]> CompressData(IEnumerable<byte[]> data);
        IEnumerable<byte[]> DecompressData(IEnumerable<byte[]> compressedData);
    }
}

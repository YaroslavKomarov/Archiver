using System;
using System.Collections.Generic;

namespace Archiver.Domain.Interfaces
{
    public interface IArchiverBase
    {
        string AlgorithmExtension { get; }
        Dictionary<string, byte[]> AccessoryData { get; set; }
        byte[] CompressData(byte[] data); 
        byte[] DecompressData(byte[] compressedData);
    }
} 

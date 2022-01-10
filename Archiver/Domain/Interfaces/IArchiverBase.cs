using System;
using System.Collections.Generic;

namespace Archiver.Domain.Interfaces
{
    public interface IArchiverBase
    {
        string AlgorithmExtension { get; }
        // Уже на данном этапе можно игнорировать использование вспомогательного словаря, т.к. каждая реализация сама отвечает за его создание
        Dictionary<string, byte[]> AccessoryData { get; set; }
        byte[] CompressData(byte[] data); 
        byte[] DecompressData(byte[] compressedData);
    }
} 

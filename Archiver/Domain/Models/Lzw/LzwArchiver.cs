using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archiver.Domain.Interfaces;

namespace Archiver.Domain.Models.Lzw
{
    public class LzwArchiver : IArchiverBase
    {
        public string AlgorithmExtension => ".lzw";

        public Dictionary<string, byte[]> AccessoryData { get; set; }

        public LzwArchiver()
        {
            AccessoryData = new Dictionary<string, byte[]>();
        }

        public byte[] CompressData(byte[] data) // TODO: Улучшить алгоритм: сжатие выполняется только для .txt файлов
        {
            if (data.Length == 0)
                throw new ArgumentException("LzwArchiver был передан пустой массив на сжатие");

            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(((char)i).ToString(), i);

            string w = string.Empty;
            List<int> compressed = new List<int>();

            foreach (char c in data)
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    compressed.Add(dictionary[w]);
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }

            if (!string.IsNullOrEmpty(w))
                compressed.Add(dictionary[w]);

            return ConvertToByteArray(compressed);
        }

        public byte[] DecompressData(byte[] compressedData)
        {
            if (compressedData.Length == 0)
                throw new ArgumentException("HuffmanArchiver был передан пустой массив на разархивацию");

            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            for (int i = 0; i < 256; i++)
                dictionary.Add(i, ((char)i).ToString());

            var compressed = ConvertByteListToIntList(compressedData.ToArray());

            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);

            foreach (var k in compressed)
            {
                string entry = null;
                if (dictionary.ContainsKey(k))
                    entry = dictionary[k];
                else if (k == dictionary.Count)
                    entry = w + w[0];

                decompressed.Append(entry);

                dictionary.Add(dictionary.Count, w + entry[0]);

                w = entry;
            }

            return ConvertStringToByteArray(decompressed.ToString());
        }

        private static byte[] ConvertToByteArray(List<int> list)
        {
            var result = new List<byte>();

            foreach (var e in list)
                result.AddRange(BitConverter.GetBytes(e));

            return result.ToArray();
        }

        private static byte[] ConvertStringToByteArray(string str)
        {
            var result = new List<byte>();

            foreach (var e in str)
                result.Add((byte)e);

            return result.ToArray();
        }

        private static List<int> ConvertByteListToIntList(byte[] array)
        {
            var result = new List<int>();
            var source = array.ToList();

            while (source.Count % 4 != 0)
                source.RemoveAt(source.Count - 1);

            while (source.Count > 0)
            {
                result.Add(BitConverter.ToInt32(source.Take(4).ToArray(), 0));
                source.RemoveRange(0, 4);
            }

            return result;    
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archiver.Domain.Interfaces;

namespace Archiver.Domain.Models
{
    public class LzwArchiver : IArchiverBase
    {
        public string AlgorithmExtension => ".lzw";

        public Dictionary<string, byte[]> AccessoryData { get; set; }

        public LzwArchiver()
        {
            AccessoryData = new Dictionary<string, byte[]>();
        }

        public byte[] CompressData(byte[] data)
        {
            // build the dictionary
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
                    // write w to output
                    compressed.Add(dictionary[w]);
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }

            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
                compressed.Add(dictionary[w]);

            var result = new List<byte>();
            foreach (var e in compressed)
            {
                result.AddRange(BitConverter.GetBytes(e));
            }
            return result.ToArray();
        }

        public byte[] DecompressData(byte[] compressedData)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(i, ((char)i).ToString());

            var compressed = compressedData.ToList();

            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);

            foreach (int k in compressed)
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

            var result = new List<byte>();
            var str = decompressed.ToString().ToList();
            foreach (var e in str)
            {
                var c = (byte)e;
                result.Add((byte)e);
            }
                
            return result.ToArray();
        }

        public static byte[] ConvertIntToByteArray(int number)
        {
            var list = new List<byte>();
            foreach (var e in number.ToString())
                list.Add((byte)e);
            return list.ToArray();
        }

        public static int ConvertByteArrayToInt(byte[] array)
        {
            var result = "";
            foreach (var e in array)
                result += e;
            return Convert.ToInt32(result);
        }


    }
}

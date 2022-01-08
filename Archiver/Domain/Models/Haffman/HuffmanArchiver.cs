using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Archiver.Domain.Interfaces;

namespace Archiver.Domain.Models.Haffman
{
    public class HuffmanArchiver : IArchiverBase
    {
        public string AlgorithmExtension => ".haf";

        public Dictionary<string, byte[]> AccessoryData { get; set; }

        public HuffmanArchiver()
        {
            AccessoryData = new Dictionary<string, byte[]>();
        }

        public byte[] CompressData(byte[] data)
        {
            var freqs = CalculateFreq(data);
            var header = CreateHeader(data.Length, freqs);
            var root = CreateHuffmanTree(freqs);
            var codes = CreateHuffmanCodes(root);
            var bits = Compress(data, codes);
            return header.Concat(bits).ToArray();
        }

        public byte[] DecompressData(byte[] compressedData)
        {
            ParseHeader(compressedData, out int length, out int startIndex, out int[] freqs);
            var root = CreateHuffmanTree(freqs);
            return Decompress(compressedData, startIndex, length, root);
        }

        private byte[] Decompress(byte[] compressedData, int startIndex, int length, Node root)
        {
            var size = 0;
            var currentNode = root;
            var data = new List<byte>();
            for (var i = startIndex; i < compressedData.Length; i++)
                for (var bit = 1; bit <= 128; bit <<= 1)
                {
                    var isZero = (compressedData[i] & bit) == 0;
                    if (isZero)
                        currentNode = currentNode.Bit0;
                    else
                        currentNode = currentNode.Bit1;
                    if (currentNode.Bit0 != null)
                        continue;
                    if (size++ < length)
                        data.Add(currentNode.Symbol);
                    currentNode = root;
                }
            return data.ToArray();
        }

        private void ParseHeader(byte[] compressedData,
            out int length,
            out int startIndex,
            out int[] freqs)
        {
            length = compressedData[0] |
                (compressedData[1] << 8) |
                (compressedData[1] << 16) |
                (compressedData[1] << 24);

            freqs = new int[256];
            for (var i = 0; i < 256; i++)
                freqs[i] = compressedData[4 + i];
            startIndex = 4 + 256;
        }

        private byte[] CreateHeader(int length, int[] freqs)
        {
            var header = new List<byte>();
            header.Add((byte)(length & 255));
            header.Add((byte)((length >> 8) & 255));
            header.Add((byte)((length >> 16) & 255));
            header.Add((byte)((length >> 24) & 255));
            for (var i = 0; i < 256; i++)
                header.Add((byte)freqs[i]);
            return header.ToArray();
        }

        private string[] CreateHuffmanCodes(Node root)
        {
            var codes = new string[256];
            Next(root, "");
            return codes;

            void Next(Node node, string code)
            {
                if (node.Bit0 == null)
                    codes[node.Symbol] = code;
                else
                {
                    Next(node.Bit0, code + "0");
                    Next(node.Bit1, code + "1");
                }
            }
        }

        private Node CreateHuffmanTree(int[] freqs)
        {
            var queue = new PriorityQueue<Node>();
            for (var i = 0; i < 256; i++)
                if (freqs[i] > 0)
                    queue.Enqueue(freqs[i], new Node((byte)i, freqs[i]));

            while (queue.Size > 1)
            {
                var bit0 = queue.Dequeue();
                var bit1 = queue.Dequeue();
                var sumFreq = bit0.Freq + bit1.Freq;
                var nextNode = new Node(bit0, bit1, sumFreq);
                queue.Enqueue(sumFreq, nextNode);
            }
            return queue.Dequeue();
        }

        private int[] CalculateFreq(byte[] data)
        {
            var freqs = new int[256];
            foreach (var d in data)
                freqs[d]++;
            NormalizeFreqs();
            return freqs;

            void NormalizeFreqs()
            {
                var max = freqs.Max();
                if (max < 255) return;
                for (var i = 0; i < 256; i++)
                    if (freqs[i] > 0)
                        freqs[i] = 1 + freqs[i] * 255 / (max + 1);
            }
        }

        private byte[] Compress(byte[] data, string[] codes)
        {
            var bits = new List<byte>();
            byte sum = 0;
            byte bit = 1;
            foreach (var symbol in data)
                foreach (var c in codes[symbol])
                {
                    if (c == '1')
                        sum |= bit;
                    if (bit < 128)
                        bit <<= 1;
                    else
                    {
                        bits.Add(sum);
                        sum = 0;
                        bit = 1;
                    }
                }
            if (bit > 1)
                bits.Add(sum);
            return bits.ToArray();
        }
    }
}

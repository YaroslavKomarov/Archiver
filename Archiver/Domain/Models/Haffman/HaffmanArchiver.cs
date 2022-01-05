using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archiver.Domain.Interfaces;

namespace Archiver.Domain.Models.Haffman
{
    public class HaffmanArchiver : IArchiverBase
    {
        private string allCode;
        private StringBuilder content;
        private List<Tuple<int, string>> codeList;
        private Dictionary<string, char> decompressedDict;
        private Encoding encoding = Encoding.ASCII;

        public string AlgorithmExtension => ".haf";

        public Dictionary<string, byte[]> AccessoryData
        {
            get;
            set;
        }

        public HaffmanArchiver()
        {
            content = new StringBuilder();
            codeList = new List<Tuple<int, string>>();
            AccessoryData = new Dictionary<string, byte[]>();
        }

        public Tuple<byte[], Dictionary<string, byte[]>> CompressData(byte[] bytes)
        {
            var frequencyDict = new Dictionary<char, int>();

            foreach (var b in bytes)
            {
                var symbol = (char)b;
                content.Append(symbol);
                if (!frequencyDict.ContainsKey(symbol))
                    frequencyDict.Add(symbol, 0);

                frequencyDict[symbol]++;
            }

            BypassTree(MakeTree(frequencyDict), "");
            return Tuple.Create(CreateCompressedFile(GetCodes(frequencyDict)), GetReformatedDecodedDictionary());
        }

        public byte[] DecompressData(byte[] compressedData, Dictionary<string, byte[]> dictionary)
        {
            var decompressedBytes = new List<byte>();
            decompressedDict = ReformatedReceivedDictionary(dictionary);
            var code = "";
            foreach (var b in compressedData)
            {
                var nextByte = Convert.ToString(b, 2).PadLeft(8, '0');
                foreach (var c in nextByte)
                {
                    code += c;
                    if (decompressedDict.ContainsKey(code))
                    {
                        decompressedBytes.Add((byte)decompressedDict[code]);
                        code = "";
                    }
                }
            }
            return decompressedBytes.ToArray();
        }

        private byte[] CreateCompressedFile(Dictionary<char, string> codeDictionary)
        {
            //Превращает текст в закодированную строку "0" и "1"
            var strBuilder = new StringBuilder();
            foreach (var c in content.ToString())
            {
                strBuilder.Append(codeDictionary[c]);
            }

            allCode = strBuilder.ToString();
            return BitsToByteConverter.Compress(allCode);
        }

        private Tree MakeTree(Dictionary<char, int> frequencyDictionary)
        {
            var frequenciesLst = frequencyDictionary.Select(p => p.Value).ToList();
            var queue = new Queue<Tree>();

            while (frequenciesLst.Count != 1)
            {
                frequenciesLst = frequenciesLst.OrderByDescending(c => c).ToList();
                var leftValue = frequenciesLst[frequenciesLst.Count - 1];
                var rightValue = frequenciesLst[frequenciesLst.Count - 2];
                var newNode = new Tree(leftValue, rightValue);
                var count = queue.Count;
                while (count-- > 0)
                {
                    var node = queue.Dequeue();
                    if (node.Value == leftValue && newNode.Left.Left == null)
                        newNode.Left = node;
                    else if (node.Value == rightValue && newNode.Right.Left == null)
                        newNode.Right = node;
                    else
                        queue.Enqueue(node);
                }
                queue.Enqueue(newNode);
                frequenciesLst.RemoveRange(frequenciesLst.Count - 2, 2);
                frequenciesLst.Add(leftValue + rightValue);
            }
            return queue.Dequeue();
        }

        private Dictionary<char, string> GetCodes(Dictionary<char, int> frequencyDictionary)
        {
            decompressedDict = new Dictionary<string, char>();
            var result = new Dictionary<char, string>();
            var visited = new HashSet<string>();
            foreach (var pair in frequencyDictionary)
                foreach (var tuple in codeList)
                    if (tuple.Item1 == pair.Value && !visited.Contains(tuple.Item2))
                    {
                        decompressedDict.Add(tuple.Item2, pair.Key);
                        result.Add(pair.Key, tuple.Item2);
                        visited.Add(tuple.Item2);
                        break;
                    }

            return result;
        }

        private void BypassTree(Tree root, string code)
        {
            if (root == null)
                return;
            BypassTree(root.Left, code + "1");

            if (root.Right == null && root.Left == null)
                codeList.Add(new Tuple<int, string>(root.Value, code));

            BypassTree(root.Right, code + "0");
        }

        private Dictionary<string, byte[]> GetReformatedDecodedDictionary()
        {
            var refDict = new Dictionary<string, byte[]>(decompressedDict.Count);
            foreach (var pair in decompressedDict)
            {
                refDict.Add(pair.Key, encoding.GetBytes(pair.Value.ToString()));
            }
            return refDict;
        }

        private Dictionary<string, char> ReformatedReceivedDictionary(Dictionary<string, byte[]> recDict)
        {
            var decDict = new Dictionary<string, char>(recDict.Count);
            foreach (var pair in recDict)
            {
                decDict.Add(pair.Key, char.Parse(encoding.GetString(pair.Value)));
            }
            return decDict;
        }

        public IEnumerable<byte[]> CompressData(IEnumerable<byte[]> data)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte[]> DecompressData(IEnumerable<byte[]> compressedData)
        {
            throw new NotImplementedException();
        }
    }
}

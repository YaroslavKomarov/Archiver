using System.Collections.Generic;
using System.Text;

namespace Archiver.Infrastructure
{
    public static class ConverterToFormat
    {
        public static readonly byte[] AccessoryDataSeparator = new byte[] { 0, 1 };

        public static readonly byte[] DataSeparator = new byte[] { 0, 2 };

        public static readonly Encoding UsedEncoding = Encoding.ASCII;

        public static string GetStringFromBytes(byte[] bytes)
        {
            return UsedEncoding.GetString(bytes);
        }

        public static byte[] GetBytesFromString(string str)
        {
            return UsedEncoding.GetBytes(str);
        }

        public static List<byte[]> GetByteArraysFromByteData(byte[] data)
        {
            var result = new List<byte[]>();
            var tmpBytesList = new List<byte>();
            for (var i = 0; i < data.Length; i++)
            {
                var b = data[i];
                if (i + 1 < data.Length && b == DataSeparator[0] && data[i + 1] == DataSeparator[1])
                {
                    i++;
                    result.Add(tmpBytesList.ToArray());
                    tmpBytesList = new List<byte>();
                }
                else tmpBytesList.Add(b);
            }
            return result;
        }

        public static Dictionary<string, byte[]> ConvertAccessoryDataToDictionary(byte[] accessoryData)
        {
            var dictionary = new Dictionary<string, byte[]>();
            var keyBytes = new List<byte>();
            var valBytes = new List<byte>();
            var isKeyAccumulation = true;

            for (var i = 0; i < accessoryData.Length; i++)
            {
                var b = accessoryData[i];
                if (i + 1 < accessoryData.Length && b == AccessoryDataSeparator[0] && accessoryData[i + 1] == AccessoryDataSeparator[1])
                {
                    i++;
                    isKeyAccumulation = !isKeyAccumulation;
                    dictionary.Add(UsedEncoding.GetString(keyBytes.ToArray()), valBytes.ToArray());
                    keyBytes = new List<byte>();
                    valBytes = new List<byte>();
                }
                else if (isKeyAccumulation)
                    keyBytes.Add(b);
                else
                    valBytes.Add(b);
            }
            return dictionary;
        }

        public static byte[] ConvertAccessoryDictToByteArray(Dictionary<string, byte[]> accessoryData)
        {
            var result = new List<byte>();
            foreach (var pair in accessoryData)
            {
                var keyBytes = UsedEncoding.GetBytes(pair.Key);
                AddBytesWithInsignificantZeros(keyBytes, result);
                foreach (var b in AccessoryDataSeparator)
                    result.Add(b);
                AddBytesWithInsignificantZeros(pair.Value, result);
                foreach (var b in AccessoryDataSeparator)
                    result.Add(b);
            }
            return result.ToArray();
        }

        public static byte[] CollectDataIntoByteArray(params IEnumerable<byte>[] data)
        {
            var allData = new List<byte>();
            foreach (var bytes in data)
            {
                AddBytesWithInsignificantZeros(bytes, allData);
                foreach (var b in DataSeparator)
                    allData.Add(b);
            }
            return allData.ToArray();
        }

        private static void AddBytesWithInsignificantZeros(IEnumerable<byte> bytes, List<byte> lstBytes)
        {
            foreach (var b in bytes)
            {
                if (b == 0) lstBytes.Add(0);
                lstBytes.Add(b);
            }
        }
    }
}

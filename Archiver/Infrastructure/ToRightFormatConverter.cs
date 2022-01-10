using System.Collections.Generic;
using System.Text;

namespace Archiver.Infrastructure
{
    public static class ToRightFormatConverter
    {
        public static readonly byte[] DataSeparator = new byte[] { 0, 1 };

        public static readonly byte[] AccessoryDataSeparator = new byte[] { 0, 2 };

        public static readonly byte[] CompressedDataSeparator = new byte[] { 0, 3 };

        public static readonly Encoding UsedEncoding = Encoding.UTF8;

        public static byte[] ConvertAccessoryDictToByteArray(Dictionary<string, byte[]> accessoryDict)
        {
            var result = new List<byte>();
            foreach (var pair in accessoryDict)
            {
                var keyBytes = UsedEncoding.GetBytes(pair.Key);
                AddAccessoryBytesWithZeros(keyBytes, result);
                foreach (var b in AccessoryDataSeparator)
                    result.Add(b);
                AddAccessoryBytesWithZeros(pair.Value, result);
                foreach (var b in AccessoryDataSeparator)
                    result.Add(b);
            }
            if (result.Count == 0) result.Add(1);
            return result.ToArray();
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
                if (IsAccessorySeparatorFound(accessoryData, i))
                {
                    i++;
                    if (!isKeyAccumulation)
                    {
                        dictionary.Add(UsedEncoding.GetString(keyBytes.ToArray()), valBytes.ToArray());
                        keyBytes = new List<byte>();
                        valBytes = new List<byte>(); 
                    }
                    isKeyAccumulation = !isKeyAccumulation;
                }
                else if (isKeyAccumulation)
                    keyBytes.Add(b);
                else
                    valBytes.Add(b);
            }
            return dictionary;
        }

        public static byte[] RemoveZerosFromBytes(byte[] bytes)
        {
            var separator = CompressedDataSeparator;
            var lstBytes = new List<byte>();
            for (var i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];
                if (i + 2 < bytes.Length)
                    if (b == separator[0] && bytes[i + 1] == separator[0] && bytes[i + 2] == separator[1])
                        continue;
                lstBytes.Add(b);
            }
            return lstBytes.ToArray();
        }

        public static string GetStringFromBytes(byte[] bytes)
        {
            return UsedEncoding.GetString(bytes);
        }

        public static byte[] GetBytesFromString(string str)
        {
            return UsedEncoding.GetBytes(str);
        }

        public static byte[] GetBytesWithZerosDataSeparator(byte[] bytes)
        {
            var lstBytes = new List<byte>();
            AddBytesWithZeros(bytes, lstBytes, DataSeparator);
            return lstBytes.ToArray();
        }

        public static byte[] GetBytesWithZerosCompressedDataSep(byte[] bytes)
        {
            var lstBytes = new List<byte>();
            AddBytesWithZeros(bytes, lstBytes, CompressedDataSeparator);
            return lstBytes.ToArray();
        }

        private static void AddBytesWithZeros(byte[] bytes, List<byte> lstBytes, byte[] separator)
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];
                if (i + 1 < bytes.Length && b == separator[0] && bytes[i + 1] == separator[1]) 
                    lstBytes.Add(0);
                lstBytes.Add(b);
            }
        }

        private static void AddAccessoryBytesWithZeros(byte[] bytes, List<byte> lstBytes)
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];
                if (i + 1 < bytes.Length)
                    if ((b == AccessoryDataSeparator[0] && bytes[i + 1] == AccessoryDataSeparator[1])
                        || (b == DataSeparator[0] && bytes[i + 1] == DataSeparator[1]))
                        lstBytes.Add(0);
                lstBytes.Add(b);
            }
        }

        private static bool IsAccessorySeparatorFound(byte[] accessoryData, int i)
        {
            return i + 1 < accessoryData.Length && i - 1 >= 0 
                && accessoryData[i] == AccessoryDataSeparator[0]
                && accessoryData[i + 1] == AccessoryDataSeparator[1]
                && accessoryData[i - 1] != AccessoryDataSeparator[0];
        }
    }
}
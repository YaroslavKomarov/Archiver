using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archiver.Infrastructure
{
    public static class ToRightFormatConverter
    {
        public static readonly byte[] AccessoryDataSeparator = new byte[] { 0, 1 };

        public static readonly byte[] DataSeparator = new byte[] { 0, 2 };

        public static readonly Encoding UsedEncoding = Encoding.UTF8;

        public static byte[] GetRightFomatExtension(string extension)
        {
            var extensionBytes = GetBytesFromString(extension);
            var formatExtensionBytes = GetBytesWithInsignificantZeros(extensionBytes);
            return formatExtensionBytes.Concat(DataSeparator).ToArray();
        }

        public static byte[] ConvertAccessoryDictToByteArray(Dictionary<string, byte[]> accessoryDict)
        {
            var result = new List<byte>();
            foreach (var pair in accessoryDict)
            {
                var keyBytes = UsedEncoding.GetBytes(pair.Key);
                AddBytesWithInsignificantZeros(keyBytes, result, true);
                foreach (var b in AccessoryDataSeparator)
                    result.Add(b);
                AddBytesWithInsignificantZeros(pair.Value, result, true);
                foreach (var b in AccessoryDataSeparator)
                    result.Add(b);
            }
            if (result.Count == 0) result.Add(1);
            return result.Concat(DataSeparator).ToArray();
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
                if (IsSeparatorFound(accessoryData, i))
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

        public static byte[] RemoveInsignificantZerosFromBytes(byte[] bytes)
        {
            var separator = DataSeparator;
            var lstBytes = new List<byte>();
            for (var i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];
                if (i + 2 < bytes.Length && b == separator[0] && bytes[i + 1] == separator[0] && bytes[i + 2] == separator[1])
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

        public static byte[] GetBytesWithInsignificantZeros(byte[] bytes, bool isAccessory = false)
        {
            var lstBytes = new List<byte>();
            if (isAccessory)
                AddBytesWithInsignificantZeros(bytes, lstBytes, isAccessory);
            else
                AddBytesWithInsignificantZeros(bytes, lstBytes, isAccessory);
            return lstBytes.ToArray();
        }

        private static void AddBytesWithInsignificantZeros(byte[] bytes, List<byte> lstBytes, bool isAccessory)
        {
            var separartor = isAccessory ? AccessoryDataSeparator : DataSeparator;
            for (var i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];
                if (i + 1 < bytes.Length && b == separartor[0] && bytes[i + 1] == separartor[1]) 
                    lstBytes.Add(0);
                lstBytes.Add(b);
            }
        }

        private static bool IsSeparatorFound(byte[] accessoryData, int i)
        {
            return i + 1 < accessoryData.Length && i - 1 > 0 
                && accessoryData[i] == AccessoryDataSeparator[0]
                && accessoryData[i + 1] == AccessoryDataSeparator[1]
                && accessoryData[i - 1] != AccessoryDataSeparator[0];
        }
    }
}
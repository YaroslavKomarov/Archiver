using System;
using System.Collections.Generic;

namespace Archiver.Domain.Models.Haffman
{
    public static class CompressBits
    {
        public static byte[] Compress(string bits)
        {
            var window = "";
            var compressedBytes = new List<byte>();
            for (var i = 0; i < bits.Length; i++)
            {
                if (i % 8 == 0 && i != 0)
                {
                    compressedBytes.Add(Convert.ToByte(window, 2));
                    window = "";
                }
                window += bits[i];
                if (i == bits.Length - 1)
                {
                    window = FillZeroAtWindow(window);
                    compressedBytes.Add(Convert.ToByte(window, 2));
                }
            }
            return compressedBytes.ToArray();
        }

        private static string FillZeroAtWindow(string window)
        {
            var len = 8 - window.Length;
            for (var i = 0; i < len; i++)
                window += '0';
            return window;
        }
    }
}

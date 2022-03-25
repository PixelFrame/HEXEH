using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HEXEH.Core.Utility
{
    public static class Common
    {
        public static byte[] StringToBytes(string strBlob)
        {
            var regDeNoise = new Regex(@"[\\\r\n\t, -]|0x");
            strBlob = regDeNoise.Replace(strBlob, "");
            byte[] res = Array.Empty<byte>();
            if (strBlob.Length % 2 == 1) return res;
            res = new byte[strBlob.Length >> 1];
            try
            {
                var halflen = strBlob.Length >> 1;
                var arr = new byte[halflen];
                for(int i = 0; i < halflen; i++)
                {
                    res[i] = (byte)((GetHexVal(strBlob[i << 1]) << 4) + GetHexVal(strBlob[(i << 1) + 1]));
                }
            }
            catch (InvalidDataException)
            {
                return Array.Empty<byte>();
            }
            return res;
        }
        private static byte GetHexVal(char h)
        {
            var val = (byte)h;
            val -= val switch
            {
                > 47 and < 58 => 48,
                > 64 and < 71 => 55,
                > 96 and < 103 => 87,
                _ => throw new InvalidDataException($"{h} is not a valid byte char"),
            };
            return val;
        }
    }
}

﻿using System.Runtime.InteropServices;
using System.Text;

namespace HEXEH.Core.DataType
{
    [Guid("37d68d01-f2ab-4674-9f8f-11e942d49abb")]
    public class HexDump : IDataType
    {
        public static string Name { get; } = "HexDump";
        public static string Description { get; } = "Raw Hex Data";

        private string _formattedHex = "";
        private string _formattedChars = "";
        private string _lineNum = "";
        public string LineNum { get => _lineNum; }
        public string FormattedHex { get => _formattedHex; }
        public string FormattedChars { get => _formattedChars; }

        public byte[] Blob { get; set; } = Array.Empty<byte>();

        public HexDump(byte[] blob)
        {
            Blob = blob;
            DoConvertFromBytes(16);
        }

        private void DoConvertFromBytes(uint step)
        {
            StringBuilder line = new();
            StringBuilder hex = new();
            StringBuilder chars = new();
            uint count = 1;
            line.Append("00000000");
            foreach (byte b in Blob)
            {
                hex.Append(b.ToString("X2"));
                if (b < 0x20 || (b > 0x7e && b < 0xA0)) chars.Append('.');
                else chars.Append((char)b);
                if (count % step == 0)
                {
                    hex.Append('\n');
                    line.Append('\n');
                    line.Append(count.ToString("X8"));
                    chars.Append('\n');
                } else
                {
                    hex.Append(' ');
                }
                ++count;
            }
            _lineNum = line.ToString();
            _formattedHex = hex.ToString();
            _formattedChars = chars.ToString();
        }

        public static HexDump ConvertFromBytes(byte[] blob)
        {
            return new HexDump(blob);
        }

        IDataType IDataType.ConvertFromBytes(byte[] blob)
        {
            return ConvertFromBytes(blob);
        }

        IDataType IDataType.ConvertFromBytes(byte[] blob, Dictionary<string, object> settingMap)
        {
            return ConvertFromBytes(blob);
        }

        public DataTree ToDataTree()
        {
            throw new NotImplementedException();
        }
    }
}
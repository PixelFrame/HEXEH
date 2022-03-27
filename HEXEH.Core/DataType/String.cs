using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.Core.DataType
{
    public class String : IDataType
    {
        public static string Name { get; } = "String";
        public static string Description { get; } = "HEX to Plain String";
        public static Dictionary<string, List<string>?>? SettingMap { get; } = new Dictionary<string, List<string>?>()
        {
            { "Encoding#multi", new List<string>() { "ASCII", "UTF8", "Unicode", "gb2312", "GB18030", "shift_jis", "big5" } },
        };
        public Dictionary<string, List<string>>? InputSettingMap { get; set; } = null;
        private byte[] _blob = Array.Empty<byte>();
        public byte[] Blob
        { 
            get { return _blob; } 
            set
            {
                _blob = value;
                DoConvertFromBytes();
            }
        }
        private Dictionary<string, string> convertedStrings = new();
        private void DoConvertFromBytes()
        {
            if (InputSettingMap != null)
            {
                var encodings = InputSettingMap["Encoding"];
                foreach (var encoding in encodings)
                {
                    var __ = Encoding.GetEncoding(encoding).GetString(this.Blob);
                    convertedStrings[encoding] = __;
                }
            }
            else
            {
                convertedStrings["ASCII"] = Encoding.ASCII.GetString(this.Blob);
                convertedStrings["UTF8"] = Encoding.UTF8.GetString(this.Blob);
                convertedStrings["Unicode"] = Encoding.Unicode.GetString(this.Blob);
                convertedStrings["Default"] = Encoding.Default.GetString(this.Blob);
            }
        }
        public static String ConvertFromBytes(byte[] blob, Dictionary<string, object>? settingMap)
        {
            String newObj = new();
            if (settingMap == null || settingMap.Count == 0) newObj.InputSettingMap = null;
            else
            {
                var castedSettingMap = settingMap.ToDictionary(x => x.Key, x => (List<string>)x.Value);
                newObj.InputSettingMap = castedSettingMap;
            }
            newObj.Blob = blob;
            return newObj;
        }

        IDataType IDataType.ConvertFromBytes(byte[] blob, Dictionary<string, object>? settingMap)
        {
            return ConvertFromBytes(blob, settingMap);
        }

        public DataTree ToDataTree()
        {
            var tree = new DataTree(Name, Description);
            foreach(var str in convertedStrings)
            {
                tree.Head.Childs.Add(new DataTreeNode(str.Key, str.Value));
            }
            return tree;
        }
    }
}

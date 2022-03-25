using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.Core.DataType
{
    public interface IDataType
    {
        public static string Name { get; } = "IType";
        public static string Description { get; } = "Data Type Interface";
        public byte[] Blob { get; }
        public IDataType ConvertFromBytes(byte[] blob);
        public IDataType ConvertFromBytes(byte[] blob, Dictionary<string, object> settingMap);
        public DataTree ToDataTree();
        public static Dictionary<string, List<string>?>? SettingMap { get; }
    }
}

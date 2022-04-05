using HEXEH.Core.Helper.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.Core.DataType
{
    public class DNS_AD : IDataType
    {
        public static string Name { get; } = "AD DNS Record";
        public static string Description { get; } = "Active Directory Metadata of Microsoft DNS";
        public static Dictionary<string, List<string>?>? SettingMap { get; } = null;
        public byte[] Blob { get; set; } = Array.Empty<byte>();

        public static DNS_AD ConvertFromBytes(byte[] blob, Dictionary<string, object>? settingMap)
        {
            var dnsad = new DNS_AD();
            dnsad.Blob = blob;
            return dnsad;
        }
        IDataType IDataType.ConvertFromBytes(byte[] blob, Dictionary<string, object>? settingMap)
        {
            return ConvertFromBytes(blob, settingMap);
        }

        public DataTree ToDataTree()
        {
            var dt = new DataTree(Name, Description);
            dt.Head.Childs.AddRange(DNS_RECORD.ToDataTreeNodes(Blob));
            
            return dt;
        }
    }
}

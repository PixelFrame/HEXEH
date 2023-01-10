using HEXEH.Core.Helper.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.Core.DataType
{
    public class DNS : IDataType
    {
        public static string Name { get; } = "DNS Resource Record";
        public static string Description { get; } = "";
        public static Dictionary<string, List<string>?>? SettingMap { get; } = new Dictionary<string, List<string>?>()
        {
            { "RR Type#single", new List<string>() { "A", "AAAA", "NS/MD/MF/CNAME/MB/MG/MR/PTR/DNAME", "SOA", "SRV", "MX/AFSDB/RT", "TXT/X25/ISDN" } },
        };
        private DNSType _type = 0;
        public byte[] Blob { get; set; } = Array.Empty<byte>();

        public static DNS ConvertFromBytes(byte[] blob, Dictionary<string,object>? settingMap)
        {
            var dns = new DNS();
            dns.Blob = blob;
            if (settingMap == null) return dns;
            var selectedType = (string)settingMap["RR Type"];

            switch (selectedType)
            {
                case "A": dns._type = DNSType.A; break;
                case "AAAA": dns._type = DNSType.AAAA; break;
                case "NS, MD, MF, CNAME, MB, MG, MR, PTR, DNAME": dns._type = DNSType.CNAME; break;
                case "SOA": dns._type = DNSType.SOA; break;
                case "SRV": dns._type = DNSType.SRV; break;
                case "MX, AFSDB, RT": dns._type = DNSType.MX; break;
                case "TXT, X25, ISDN": dns._type = DNSType.TXT; break;
            }

            return dns;
        }
        IDataType IDataType.ConvertFromBytes(byte[] blob, Dictionary<string, object>? settingMap)
        {
            return ConvertFromBytes(blob, settingMap);
        }

        public DataTree ToDataTree()
        {
            var dt = new DataTree(Name, Description);
            dt.Head.Childs.Add(DNSRR.ToDataTreeNode(Blob, _type));
            return dt;
        }
    }
}

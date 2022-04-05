using HEXEH.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.Core.Helper.Windows
{
    public static class DNSRR
    {
        public static DataTreeNode ToDataTreeNode(byte[] blob, DNSType type)
        {
            var handle = GCHandle.Alloc(blob, GCHandleType.Pinned);
            switch (type)
            {
                case DNSType.A: 
                    var RR_A = (A)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(A));
                    return RR_A.GetDataTreeNode();
                case DNSType.AAAA:
                    var RR_AAAA = (AAAA)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(AAAA));
                    return RR_AAAA.GetDataTreeNode();
                case DNSType.NS:
                case DNSType.MD:
                case DNSType.MF:
                case DNSType.CNAME:
                case DNSType.MB:
                case DNSType.MG:
                case DNSType.MR:
                case DNSType.PTR:
                case DNSType.DNAME:
                    var nameMarsharler = new DnsNameMarshaler();
                    var RR_NAME = (NAME)nameMarsharler.MarshalNativeToManaged(handle.AddrOfPinnedObject());
                    return RR_NAME.GetDataTreeNode();
                case DNSType.SOA:
                    var SOAMarsharler = new DnsSoaMarshaler();
                    var RR_SOA = (SOA)SOAMarsharler.MarshalNativeToManaged(handle.AddrOfPinnedObject());
                    return RR_SOA.GetDataTreeNode();
                case DNSType.SRV:
                    var SRVMarsharler = new DnsSrvMarshaler();
                    var RR_SRV = (SRV)SRVMarsharler.MarshalNativeToManaged(handle.AddrOfPinnedObject());
                    return RR_SRV.GetDataTreeNode();
                case DNSType.TXT:
                case DNSType.X25:
                case DNSType.ISDN:
                    var TXTMarsharler = new DnsTxtMarshaler();
                    var RR_TXT = (TXT)TXTMarsharler.MarshalNativeToManaged(handle.AddrOfPinnedObject());
                    return RR_TXT.GetDataTreeNode();
                case DNSType.MX:
                case DNSType.AFSDB:
                case DNSType.RT:
                    var namePrefMarsharler = new DnsNamePreferenceMarshaler();
                    var RR_MX = (NAME_PREFERENCE)namePrefMarsharler.MarshalNativeToManaged(handle.AddrOfPinnedObject());
                    return RR_MX.GetDataTreeNode();
                default:
                    return new DataTreeNode($"DNS Type {type}", "Unsupported");
            }
        }
    }

    public enum DNSType : ushort
    {
        ZERO    = 0x0,
        A       = 0x1,
        NS      = 0x2,
        MD      = 0x3,
        MF      = 0x4,
        CNAME   = 0x5,
        SOA     = 0x6,
        MB      = 0x7,
        MG      = 0x8,
        MR      = 0x9,
        NULL    = 0xA,
        WKS     = 0xB,
        PTR     = 0xC,
        HINFO   = 0xD,
        MINFO   = 0xE,
        MX      = 0xF,
        TXT     = 0x10,
        RP      = 0x11,
        AFSDB   = 0x12,
        X25     = 0x13,
        ISDN    = 0x14,
        RT      = 0x15,
        SIG     = 0x18,
        KEY     = 0x19,
        AAAA    = 0x1C,
        LOC     = 0x1D,
        NXT     = 0x1E,
        SRV     = 0x21,
        ATMA    = 0x22,
        NAPTR   = 0x23,
        DNAME   = 0x27,
        DS      = 0x2B,
        RRSIG   = 0x2E,
        NSEC    = 0x2F,
        DNSKEY  = 0x30,
        DHCID   = 0x31,
        NSEC3   = 0x32,
        NSEC3PARAM = 0x33,
        TLSA    = 0x34,
        ALL     = 0xFF,
        WINS    = 0xFF01,
        WINSR   = 0xFF02
    }

    [StructLayout(LayoutKind.Explicit)]

    public struct A
    {
        [FieldOffset(0)]
        public uint dwAddress;

        [FieldOffset(0)]
        public byte bOctet0;

        [FieldOffset(1)]
        public byte bOctet1;

        [FieldOffset(2)]
        public byte bOctet2;

        [FieldOffset(3)]
        public byte bOctet3;

        public override string ToString()
        {
            return $"{bOctet0}.{bOctet1}.{bOctet2}.{bOctet3}";
        }

        public string CastToIPv6()
        {
            return $"::ffff:{ToString()}";
        }

        public DataTreeNode GetDataTreeNode()
        {
            var dtn = new DataTreeNode("IPv4 Address", ToString());
            dtn.Childs.Add(new DataTreeNode("Octet", bOctet0.ToString("X2")));
            dtn.Childs.Add(new DataTreeNode("Octet", bOctet1.ToString("X2")));
            dtn.Childs.Add(new DataTreeNode("Octet", bOctet2.ToString("X2")));
            dtn.Childs.Add(new DataTreeNode("Octet", bOctet3.ToString("X2")));
            return dtn;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct AAAA
    {
        [FieldOffset(0)]
        public ulong qwAddress0;

        [FieldOffset(8)]
        public ulong qwAddress1;


        [FieldOffset(0)]
        public uint dwAddress0;

        [FieldOffset(4)]
        public uint dwAddress1;

        [FieldOffset(8)]
        public uint dwAddress2;

        [FieldOffset(12)]
        public uint dwAddress3;

        [FieldOffset(0)]
        public ushort wAddress0;

        [FieldOffset(2)]
        public ushort wAddress1;

        [FieldOffset(4)]
        public ushort wAddress2;

        [FieldOffset(6)]
        public ushort wAddress3;

        [FieldOffset(8)]
        public ushort wAddress4;

        [FieldOffset(10)]
        public ushort wAddress5;

        [FieldOffset(12)]
        public ushort wAddress6;

        [FieldOffset(14)]
        public ushort wAddress7;
        
        public ushort[] Words { 
            get 
            {
                return new ushort[] { wAddress0 , wAddress1 , wAddress2 , wAddress3 , wAddress4 , wAddress5 , wAddress6 , wAddress7 };
            } 
            set
            {
                wAddress0 = value[0];
                wAddress1 = value[1];
                wAddress2 = value[2];
                wAddress3 = value[3];
                wAddress4 = value[4];
                wAddress5 = value[5];
                wAddress6 = value[6];
                wAddress7 = value[7];
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            int longestLen = 0;
            int longestStartIdx = -1;
            int len = 0;
            int startIdx = -1;
            bool cnting = false;
            for (int i = 0; i < Words.Length; i++)
            {
                if (Words[i] == 0)
                {
                    if (cnting)
                    {
                        len++;
                        if (i == Words.Length - 1 && len > 1 && len > longestLen)
                        {
                            longestLen = len;
                            longestStartIdx = startIdx;
                        }
                    }
                    else
                    {
                        cnting = true;
                        len = 1;
                        startIdx = i;
                    }
                }
                else
                {
                    if (cnting)
                    {
                        cnting = false;
                        if (len > 1 && len > longestLen)
                        {
                            longestLen = len;
                            longestStartIdx = startIdx;
                        }
                    }
                }
            }
            for (int i = 0; i < Words.Length; i++)
            {
                if (i == longestStartIdx) sb.Append("::");
                else if (i > longestStartIdx && i < longestStartIdx + longestLen) continue;
                else { sb.Append(Words[i].ToString("X")); if (i != Words.Length - 1 && i != longestStartIdx - 1) sb.Append(':'); }
            }
            return sb.ToString();
        }

        public DataTreeNode GetDataTreeNode()
        {
            var dtn = new DataTreeNode("IPv6 Address", ToString());
            foreach (var word in Words)
            {
                dtn.Childs.Add(new DataTreeNode("WORD", word.ToString("X4")));
            }
            return dtn;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NAME  //NS, MD, MF, CNAME, MB, MG, MR, PTR, DNAME
    {
        public byte bLength;
        public byte bNameCount;
        public NameSeg[] NameSegs;

        public struct NameSeg
        {
            public byte bSegLength;
            public string strSegName;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append('(');
            sb.Append(bLength);
            sb.Append(")(");
            sb.Append(bNameCount);
            sb.Append(')');
            sb.Append(' ');
            foreach (var seg in NameSegs)
            {
                sb.Append('(');
                sb.Append(seg.bSegLength);
                sb.Append(')');
                sb.Append(seg.strSegName);
            }
            return sb.ToString();
        }

        public DataTreeNode GetDataTreeNode()
        {
            var dtn = new DataTreeNode("DNS Name", ToString());
            dtn.Childs.Add(new DataTreeNode("Length", bLength.ToString()));
            dtn.Childs.Add(new DataTreeNode("Name Count", bNameCount.ToString()));
            foreach (var seg in NameSegs)
            {
                dtn.Childs.Add(new DataTreeNode("Name Length", seg.bSegLength.ToString()));
                dtn.Childs.Add(new DataTreeNode("Name", seg.strSegName));
            }
            return dtn;
        }
    }

    public struct SOA
    {
        public uint dwSerialNo;
        public uint dwRefresh;
        public uint dwRetry;
        public uint dwExpire;
        public uint dwMinimumTtl;
        public NAME namePrimaryServer;
        public NAME nameZoneAdministratorEmail;

        public override string ToString()
        {
            return $"({dwSerialNo})({dwRefresh})({dwRetry})({dwExpire})({dwMinimumTtl}) {namePrimaryServer} {nameZoneAdministratorEmail}";
        }

        public DataTreeNode GetDataTreeNode()
        {
            var dtn = new DataTreeNode("DNS Start of Authority", ToString());
            dtn.Childs.Add(new DataTreeNode("Serial Number", dwSerialNo.ToString()));
            dtn.Childs.Add(new DataTreeNode("Refresh Time", dwRefresh.ToString()));
            dtn.Childs.Add(new DataTreeNode("Retry Time", dwRetry.ToString()));
            dtn.Childs.Add(new DataTreeNode("Expire Time", dwExpire.ToString()));
            dtn.Childs.Add(new DataTreeNode("Default TTL", dwMinimumTtl.ToString()));
            dtn.Childs.Add(new DataTreeNode("Primary Server", "", namePrimaryServer.GetDataTreeNode()));
            dtn.Childs.Add(new DataTreeNode("Zone Administrator Email", "", nameZoneAdministratorEmail.GetDataTreeNode()));

            return dtn;
        }
    }

    public struct SRV
    {
        public ushort wPriority;
        public ushort wWeight;
        public ushort wPort;
        public NAME nameTarget;
        
        public override string ToString()
        {
            return $"({wPriority})({wWeight})({wPort}) {nameTarget}";
        }

        public DataTreeNode GetDataTreeNode()
        {
            var dtn = new DataTreeNode("DNS Service Record", ToString());
            dtn.Childs.Add(new DataTreeNode("Priority", wPriority.ToString()));
            dtn.Childs.Add(new DataTreeNode("Weight", wWeight.ToString()));
            dtn.Childs.Add(new DataTreeNode("Port", wPort.ToString()));
            dtn.Childs.Add(new DataTreeNode("Target", "", nameTarget.GetDataTreeNode()));

            return dtn;
        }
    }

    public struct TXT  // TXT, X25, ISDN
    {
        public byte bLength;
        public string strText;

        public override string ToString()
        {
            return $"({bLength})strText";
        }

        public DataTreeNode GetDataTreeNode()
        {
            var dtn = new DataTreeNode("DNS Text Record", ToString());
            dtn.Childs.Add(new DataTreeNode("Length", bLength.ToString()));
            dtn.Childs.Add(new DataTreeNode("Text", strText));

            return dtn;
        }
    }

    public struct NAME_PREFERENCE  // MX, X25, ISDN
    {
        public ushort wPriority;
        public NAME nameExchange;

        public override string ToString()
        {
            return $"({wPriority}) {nameExchange}";
        }

        public DataTreeNode GetDataTreeNode()
        {
            var dtn = new DataTreeNode("DNS Name with Priority", ToString());
            dtn.Childs.Add(new DataTreeNode("Priority", wPriority.ToString()));
            dtn.Childs.Add(new DataTreeNode("Exchange", "", nameExchange.GetDataTreeNode()));

            return dtn;
        }
    }

    public class DnsNameMarshaler : ICustomMarshaler
    {
        public void CleanUpManagedData(object ManagedObj)
        { }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            throw new NotImplementedException();
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            var obj = new NAME();
            obj.bLength = Marshal.ReadByte(pNativeData);
            pNativeData += 1;
            obj.bNameCount = Marshal.ReadByte(pNativeData);
            pNativeData += 1;
            obj.NameSegs = new NAME.NameSeg[obj.bNameCount];

            for (int i = 0; i < obj.bNameCount; i++)
            {
                obj.NameSegs[i] = new NAME.NameSeg();
                obj.NameSegs[i].bSegLength = Marshal.ReadByte(pNativeData);
                pNativeData += 1;
                obj.NameSegs[i].strSegName = Marshal.PtrToStringUTF8(pNativeData, (int)obj.NameSegs[i].bSegLength);
                pNativeData += (int)obj.NameSegs[i].bSegLength;
            }

            return obj;
        }
    }

    public class DnsSoaMarshaler : ICustomMarshaler
    {
        public void CleanUpManagedData(object ManagedObj)
        { }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            throw new NotImplementedException();
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            var obj = new SOA();
            obj.dwSerialNo = MarshalEx.ReadUInt32BE(pNativeData);
            pNativeData += 4;
            obj.dwRefresh = MarshalEx.ReadUInt32BE(pNativeData);
            pNativeData += 4;
            obj.dwRetry = MarshalEx.ReadUInt32BE(pNativeData);
            pNativeData += 4;
            obj.dwExpire = MarshalEx.ReadUInt32BE(pNativeData);
            pNativeData += 4;
            obj.dwMinimumTtl = MarshalEx.ReadUInt32BE(pNativeData);
            pNativeData += 4;
            var nameMarshaler = new DnsNameMarshaler();
            obj.namePrimaryServer = (NAME)nameMarshaler.MarshalNativeToManaged(pNativeData);
            pNativeData += obj.namePrimaryServer.bLength + 2;
            obj.nameZoneAdministratorEmail = (NAME)nameMarshaler.MarshalNativeToManaged(pNativeData);

            return obj;
        }
    }

    public class DnsSrvMarshaler : ICustomMarshaler
    {
        public void CleanUpManagedData(object ManagedObj)
        { }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            throw new NotImplementedException();
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            var obj = new SRV();
            obj.wPriority = MarshalEx.ReadUInt16BE(pNativeData);
            pNativeData += 2;
            obj.wWeight = MarshalEx.ReadUInt16BE(pNativeData);
            pNativeData += 2;
            obj.wPort = MarshalEx.ReadUInt16BE(pNativeData);
            pNativeData += 2;
            var nameMarshaler = new DnsNameMarshaler();
            obj.nameTarget = (NAME)nameMarshaler.MarshalNativeToManaged(pNativeData);
            
            return obj;
        }
    }

    public class DnsTxtMarshaler : ICustomMarshaler
    {
        public void CleanUpManagedData(object ManagedObj)
        { }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            throw new NotImplementedException();
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            var obj = new TXT();
            obj.bLength = Marshal.ReadByte(pNativeData);
            pNativeData += 1;
            obj.strText = Marshal.PtrToStringUTF8(pNativeData, obj.bLength);

            return obj;
        }
    }

    public class DnsNamePreferenceMarshaler : ICustomMarshaler
    {
        public void CleanUpManagedData(object ManagedObj)
        { }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            throw new NotImplementedException();
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            var obj = new NAME_PREFERENCE();
            obj.wPriority = MarshalEx.ReadUInt16BE(pNativeData);
            pNativeData += 2;
            var nameMarshaler = new DnsNameMarshaler();
            obj.nameExchange = (NAME)nameMarshaler.MarshalNativeToManaged(pNativeData);

            return obj;
        }
    }
}

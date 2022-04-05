using HEXEH.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.Core.Helper.Windows
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DNS_RECORD
    {
        public ushort DataLength;
        public DNSType Type;
        public byte Version;
        public byte Rank;
        public ushort Flags;
        public uint SerialNumber;
        public uint TTL;
        public uint Reserved;
        public uint Timestamp;
        public byte[] RecordData;

        public static DataTreeNode[] ToDataTreeNodes(byte[] blob)
        {
            var handle = GCHandle.Alloc(blob, GCHandleType.Pinned);
            var recordMarshaler = new DnsRecordMarshaler();
            var record = (DNS_RECORD)recordMarshaler.MarshalNativeToManaged(handle.AddrOfPinnedObject());
            handle.Free();

            var nodes = new DataTreeNode[10];
            nodes[0] = new DataTreeNode("DataLength", record.DataLength.ToString());
            nodes[1] = new DataTreeNode("Type", record.Type.ToString());
            nodes[2] = new DataTreeNode("Version", record.Version.ToString());
            nodes[3] = new DataTreeNode("Rank", record.Rank.ToString());
            nodes[4] = new DataTreeNode("Flags", record.Flags.ToString());
            nodes[5] = new DataTreeNode("SerialNumber", record.SerialNumber.ToString());
            nodes[6] = new DataTreeNode("TTL", record.TTL.ToString());
            nodes[7] = new DataTreeNode("Reserved", record.Reserved.ToString());
            nodes[8] = new DataTreeNode("Timestamp", (new DateTime(1601,1,1)).AddHours(record.Timestamp).ToString());
            nodes[9] = DNSRR.ToDataTreeNode(record.RecordData, record.Type);
            return nodes;
        }
    }

    public class DnsRecordMarshaler : ICustomMarshaler
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
            var obj = new DNS_RECORD();
            obj.DataLength = (ushort)Marshal.ReadInt16(pNativeData);
            pNativeData += 2;
            obj.Type = (DNSType)Marshal.ReadInt16(pNativeData);
            pNativeData += 2;
            obj.Version = Marshal.ReadByte(pNativeData);
            pNativeData += 1;
            obj.Rank = Marshal.ReadByte(pNativeData);
            pNativeData += 1;
            obj.Flags = (ushort)Marshal.ReadInt16(pNativeData);
            pNativeData += 2;
            obj.SerialNumber = (uint)Marshal.ReadInt32(pNativeData);
            pNativeData += 4;
            obj.TTL = MarshalEx.ReadUInt32BE(pNativeData);
            pNativeData += 4;
            obj.Reserved = (uint)Marshal.ReadInt32(pNativeData);
            pNativeData += 4;
            obj.Timestamp = (uint)Marshal.ReadInt32(pNativeData);
            pNativeData += 4;
            obj.RecordData = new byte[obj.DataLength];
            Marshal.Copy(pNativeData, obj.RecordData, 0, obj.DataLength);

            return obj;
        }
    }
}

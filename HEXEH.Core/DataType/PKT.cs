﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.Core.DataType
{
    [Guid("3983ca60-901e-43af-bd6d-381e88767328")]
    public class PKT : IDataType
    {
        public static string Name { get; } = "PKT";
        public static string Description { get; } = "DFS NamespaceV1 Metadata";
        public byte[] Blob { get; set; } = Array.Empty<byte>();
        private DFSNamespace dfsNamespace;

        public static PKT ConvertFromBytes(byte[] blob)
        {
            var newObj = new PKT();
            newObj.Blob = blob;
            var handle = GCHandle.Alloc(blob, GCHandleType.Pinned);
            try
            {
                var dfsMarshaler = new DFSNamespaceMarshaler();
                newObj.dfsNamespace = (DFSNamespace)dfsMarshaler.MarshalNativeToManaged(handle.AddrOfPinnedObject());
            }
            catch
            {
                handle.Free();
            }
            return newObj;
        }

        IDataType IDataType.ConvertFromBytes(byte[] blob)
        {
            return ConvertFromBytes(blob);
        }

        IDataType IDataType.ConvertFromBytes(byte[] blob, Dictionary<string, object> settingMap)
        {
            return ConvertFromBytes(blob);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DFSNamespace
        {
            public uint Version;
            public uint ElementCount;
            public DFSNamespaceElement[] Elements;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DFSNamespaceElement
        {
            public ushort NameSize;
            public string Name;
            public uint DataSize;
            public DFSNamespaceRootOrLink DataRootOrLink;
            public SiteInformation DataSite;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DFSNamespaceRootOrLink
        {
            public Guid RootOrLinkGuid;
            public ushort PrefixSize;
            public string Prefix;
            public ushort ShortPrefixSize;
            public string ShortPrefix;
            public RootOrLinkType Type;
            public RootOrLinkState State;
            public ushort CommentSize;
            public string Comment;
            public TimeStamp PrefixTimeStamp;
            public TimeStamp StateTimeStamp;
            public TimeStamp CommentTimeStamp;
            public uint Version;
            public uint TargetListSize;
            public TargetList Targets;
            public uint ReservedBlobSize;
            public uint ReservedBlob;
            public uint ReferralTTL;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SiteInformation
        {
            public Guid SiteTableGuid;
            public uint SiteEntryCount;
            public SiteEntry[] SiteEntries;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TargetList
        {
            public uint TargetCount;
            public TargetEntry[] TargetEntries;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TargetEntry
        {
            public uint TargetEntrySize;
            public TimeStamp TargetTimeStamp;
            public TargetState State;
            public uint TargetType; // Should always be 0x2
            public ushort ServerNameSize;
            public string ServerName;
            public ushort ShareNameSize;
            public string ShareName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SiteEntry
        {
            public ushort ServerNameSize;
            public string ServerName;
            public uint SiteNameInfoCount;
            public SiteNameInfo[] SiteNames;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SiteNameInfo
        {
            public uint Flags;
            public ushort SiteNameSize;
            public string SiteName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TimeStamp
        {
            public uint DateTimeL;
            public uint DateTimeH;
        }

        [Flags()]
        private enum RootOrLinkType
        {
            PKT_ENTRY_TYPE_DFS = 0x1,
            PKT_ENTRY_TYPE_OUTSIDE_MY_DOM = 0x10,
            PKT_ENTRY_TYPE_INSITE_ONLY = 0x20,
            PKT_ENTRY_TYPE_COST_BASED_SITE_SELECTION = 0x40,
            PKT_ENTRY_TYPE_REFERRAL_SVC = 0x80,
            PKT_ENTRY_TYPE_ROOT_SCALABILITY = 0x200,
            PKT_ENTRY_TYPE_TARGET_FAILBACK = 0x8000
        }

        [Flags()]
        private enum RootOrLinkState
        {
            DFS_VOLUME_STATE_OK = 0x1,
            RESERVED = 0x2,
            DFS_VOLUME_STATE_OFFLINE = 0x3,
            DFS_VOLUME_STATE_ONLINE = 0x4
        }

        [Flags()]
        private enum TargetState
        {
            DFS_STORAGE_STATE_OFFLINE = 0x1,
            DFS_STORAGE_STATE_ONLINE = 0x2,
            DFS_STORAGE_STATE_ACTIVE = 0x4
        }

        [Flags()]
        private enum PriorityClass
        {
            DFS_TARGET_PRIORITY_CLASS_SITE_COST_NORMAL = 0x0,
            DFS_TARGET_PRIORITY_CLASS_GLOBAL_HIGH = 0x1,
            DFS_TARGET_PRIORITY_CLASS_SITE_COST_HIGH = 0x2,
            DFS_TARGET_PRIORITY_CLASS_SITE_COST_LOW = 0x3,
            DFS_TARGET_PRIORITY_CLASS_GLOBAL_LOW = 0x4
        }

#pragma warning disable CS8605 // Unboxing a possibly null value.
        private class DFSNamespaceMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj)
            {
            }

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
                var dfsn = new DFSNamespace();
                dfsn.Version = (uint)Marshal.ReadInt32(pNativeData);
                dfsn.ElementCount = (uint)Marshal.ReadInt32(pNativeData, 4);
                dfsn.Elements = new DFSNamespaceElement[dfsn.ElementCount];

                var pElementData = pNativeData + 8;
                var dfsneMarshaler = new DFSNamespaceElementMarshaler();
                for (int i = 0; i < dfsn.ElementCount; ++i)
                {
                    dfsn.Elements[i] = (DFSNamespaceElement)dfsneMarshaler.MarshalNativeToManaged(pElementData);
                    pElementData += (int)(6 + dfsn.Elements[i].DataSize + dfsn.Elements[i].NameSize);
                }
                return dfsn;
            }
        }

        private class DFSNamespaceElementMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj)
            {
            }

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
                DFSNamespaceElement dfsne = new();
                dfsne.NameSize = (ushort)Marshal.ReadInt16(pNativeData);
                pNativeData += 2;
                dfsne.Name = Marshal.PtrToStringUni(pNativeData, dfsne.NameSize >> 1);
                pNativeData += dfsne.NameSize;
                dfsne.DataSize = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;

                if (dfsne.Name == @"\siteroot")
                {
                    var siteRootMarshaler = new SiteRootMarshaler();
                    dfsne.DataSite = (SiteInformation)siteRootMarshaler.MarshalNativeToManaged(pNativeData);
                }
                else
                {
                    var dfsnLinkMarshaler = new DFSNamespaceRootOrLinkMarshaler();
                    dfsne.DataRootOrLink = (DFSNamespaceRootOrLink)dfsnLinkMarshaler.MarshalNativeToManaged(pNativeData);
                }

                return dfsne;
            }
        }

        private class DFSNamespaceRootOrLinkMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj)
            {
            }

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
                var dfsnlink = new DFSNamespaceRootOrLink();
                dfsnlink.RootOrLinkGuid = (Guid)Marshal.PtrToStructure(pNativeData, typeof(Guid));
                pNativeData += 16;
                dfsnlink.PrefixSize = (ushort)Marshal.ReadInt16(pNativeData);
                pNativeData += 2;
                dfsnlink.Prefix = Marshal.PtrToStringUni(pNativeData, dfsnlink.PrefixSize >> 1);
                pNativeData += dfsnlink.PrefixSize;
                dfsnlink.ShortPrefixSize = (ushort)Marshal.ReadInt16(pNativeData);
                pNativeData += 2;
                dfsnlink.ShortPrefix = Marshal.PtrToStringUni(pNativeData, dfsnlink.ShortPrefixSize >> 1);
                pNativeData += dfsnlink.ShortPrefixSize;
                dfsnlink.Type = (RootOrLinkType)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                dfsnlink.State = (RootOrLinkState)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                dfsnlink.CommentSize = (ushort)Marshal.ReadInt16(pNativeData);
                pNativeData += 2;
                dfsnlink.Comment = Marshal.PtrToStringUni(pNativeData, dfsnlink.CommentSize >> 1);
                pNativeData += dfsnlink.CommentSize;
                dfsnlink.PrefixTimeStamp = (TimeStamp)Marshal.PtrToStructure(pNativeData, typeof(TimeStamp));
                pNativeData += 8;
                dfsnlink.StateTimeStamp = (TimeStamp)Marshal.PtrToStructure(pNativeData, typeof(TimeStamp));
                pNativeData += 8;
                dfsnlink.CommentTimeStamp = (TimeStamp)Marshal.PtrToStructure(pNativeData, typeof(TimeStamp));
                pNativeData += 8;
                dfsnlink.Version = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                dfsnlink.TargetListSize = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                dfsnlink.Targets = (TargetList)new TargetListMarshaler().MarshalNativeToManaged(pNativeData);
                pNativeData += (int)dfsnlink.TargetListSize;
                dfsnlink.ReservedBlobSize = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                dfsnlink.ReservedBlob = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                dfsnlink.ReferralTTL = (uint)Marshal.ReadInt32(pNativeData);
                return dfsnlink;
            }
        }

        private class TargetListMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj)
            {
            }

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
                var tl = new TargetList();
                tl.TargetCount = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                tl.TargetEntries = new TargetEntry[tl.TargetCount];
                var teMarshaler = new TargetEntryMarshaler();
                for (int i = 0; i < tl.TargetCount; ++i)
                {
                    tl.TargetEntries[i] = (TargetEntry)teMarshaler.MarshalNativeToManaged(pNativeData);
                    pNativeData += (int)tl.TargetEntries[i].TargetEntrySize;
                }

                return tl;
            }
        }
        private class TargetEntryMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj)
            {
            }

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
                var te = new TargetEntry();
                te.TargetEntrySize = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                te.TargetTimeStamp = (TimeStamp)Marshal.PtrToStructure(pNativeData, typeof(TimeStamp));
                pNativeData += 8;
                te.State = (TargetState)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                te.TargetType = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                te.ServerNameSize = (ushort)Marshal.ReadInt16(pNativeData);
                pNativeData += 2;
                te.ServerName = Marshal.PtrToStringUni(pNativeData, te.ServerNameSize >> 1);
                pNativeData += te.ServerNameSize;
                te.ShareNameSize = (ushort)Marshal.ReadInt16(pNativeData);
                pNativeData += 2;
                te.ShareName = Marshal.PtrToStringUni(pNativeData, te.ShareNameSize >> 1);

                return te;
            }
        }

        private class SiteRootMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj)
            {
            }

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
                var si = new SiteInformation();
                si.SiteTableGuid = (Guid)Marshal.PtrToStructure(pNativeData, typeof(Guid));
                pNativeData += 16;
                si.SiteEntryCount = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                si.SiteEntries = new SiteEntry[si.SiteEntryCount];

                var seMarshaler = new SiteEntryMarshaler();

                for (int i = 0; i < si.SiteEntryCount; ++i)
                {
                    si.SiteEntries[i] = (SiteEntry)seMarshaler.MarshalNativeToManaged(pNativeData);
                    pNativeData += Marshal.SizeOf(si.SiteEntries[i]);
                }

                return si;
            }
        }

        private class SiteEntryMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj)
            {
            }

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
                var se = new SiteEntry();
                se.ServerNameSize = (ushort)Marshal.ReadInt16(pNativeData);
                pNativeData += 2;
                se.ServerName = Marshal.PtrToStringUni(pNativeData, se.ServerNameSize >> 1);
                pNativeData += se.ServerNameSize;
                se.SiteNameInfoCount = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                se.SiteNames = new SiteNameInfo[se.SiteNameInfoCount];
                var sniMarshaler = new SiteNameInfoMarshaler();
                for (int i = 0; i < se.SiteNameInfoCount; ++i)
                {
                    se.SiteNames[i] = (SiteNameInfo)sniMarshaler.MarshalNativeToManaged(pNativeData);
                    pNativeData += Marshal.SizeOf(se.SiteNames[i]);
                }
                return se;
            }
        }

        private class SiteNameInfoMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj)
            {
            }

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
                var sni = new SiteNameInfo();
                sni.Flags = (uint)Marshal.ReadInt32(pNativeData);
                sni.SiteNameSize = (ushort)Marshal.ReadInt16(pNativeData, 4);
                sni.SiteName = Marshal.PtrToStringUni(pNativeData + 6, sni.SiteNameSize >> 1);
                return sni;
            }
        }
#pragma warning restore CS8605 // Unboxing a possibly null value.

        public DataTree ToDataTree()
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.Core.DataType
{
    [Guid("da318e95-9586-44f3-9a03-ae07a4d42b12")]
    public class WinProxySetting : IDataType
    {
        public static string Name { get; } = "Windows Proxy";
        public static string Description { get; } = "Proxy Setting of Windows";
        public static Dictionary<string, List<string>?>? SettingMap { get; } = null;

        private byte[] _blob = Array.Empty<byte>();
        public byte[] Blob
        {
            get { return _blob; }
            set 
            {
                _blob = value;
                var handle = GCHandle.Alloc(Blob, GCHandleType.Pinned);
                try
                {
                    var marshaler = new WinProxySettingMarshaler();
                    _proxySetting = (_sWinProxySetting)marshaler.MarshalNativeToManaged(handle.AddrOfPinnedObject());
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException("Blob provided cannot be set converted to WinProxySetting structure", ex);
                }
                finally
                {
                    handle.Free();
                }
            }
        }
        private _sWinProxySetting _proxySetting;

        public static WinProxySetting ConvertFromBytes(byte[] blob, Dictionary<string, object>? settingMap)
        {
            var newObj = new WinProxySetting();
            newObj.Blob = blob;
            return newObj;
        }

        IDataType IDataType.ConvertFromBytes(byte[] blob, Dictionary<string, object>? settingMap)
        {
            return ConvertFromBytes(blob, settingMap);
        }

        public DataTree ToDataTree()
        {
            var dt = new DataTree(Name, Description);
            dt.Head.Childs.Add(new DataTreeNode("Magic number", _proxySetting.Magic.ToString()));
            dt.Head.Childs.Add(new DataTreeNode("Version", _proxySetting.Version.ToString()));
            dt.Head.Childs.Add(new DataTreeNode("Flag", _proxySetting.Flag.ToString()));
            dt.Head.Childs.Add(new DataTreeNode("Proxy server name size", _proxySetting.ProxyServerNameSize.ToString()));
            dt.Head.Childs.Add(new DataTreeNode("Proxy server name", _proxySetting.ProxyServerName));
            dt.Head.Childs.Add(new DataTreeNode("Proxy bypass list size", _proxySetting.ProxyBypassListSize.ToString()));
            dt.Head.Childs.Add(new DataTreeNode("Proxy bypass list", _proxySetting.ProxyBypassList));
            dt.Head.Childs.Add(new DataTreeNode("Auto config URL size", _proxySetting.AutoConfigUrlSize.ToString()));
            dt.Head.Childs.Add(new DataTreeNode("Auto config URL", _proxySetting.AutoConfigUrl));
            dt.Head.Childs.Add(new DataTreeNode("aFlag", _proxySetting.aFlag.ToString()));
            dt.Head.Childs.Add(new DataTreeNode("Padding", BitConverter.ToString(_proxySetting.Padding)));
            return dt;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct _sWinProxySetting
        {
            public uint Magic;
            public uint Version;
            public ProxyFlag Flag;
            public uint ProxyServerNameSize;
            public string ProxyServerName;
            public uint ProxyBypassListSize;
            public string ProxyBypassList;
            public uint AutoConfigUrlSize;
            public string AutoConfigUrl;
            public uint aFlag;
            public byte[] Padding;
        }

        [Flags()]
        private enum ProxyFlag : uint
        {
            PROXY_UNKNOWN = 0x1,
            PROXY_MANUAL = 0x2,
            PROXY_AUTO_CONFIGURATION = 0x4,
            PROXY_AUTO_DETECT = 0x8
        }

        private class WinProxySettingMarshaler : ICustomMarshaler
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
                var proxysetting = new _sWinProxySetting();
                proxysetting.Magic = (uint)Marshal.ReadInt32(pNativeData);
                if (proxysetting.Magic != 0x46) throw new InvalidDataException("The data provided does not seem to be a WinINET proxy setting");
                pNativeData += 4;
                proxysetting.Version = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                proxysetting.Flag = (ProxyFlag)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                proxysetting.ProxyServerNameSize = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                proxysetting.ProxyServerName = Marshal.PtrToStringAnsi(pNativeData, (int)proxysetting.ProxyServerNameSize);
                pNativeData += (int)proxysetting.ProxyServerNameSize;
                proxysetting.ProxyBypassListSize = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                proxysetting.ProxyBypassList = Marshal.PtrToStringAnsi(pNativeData, (int)proxysetting.ProxyBypassListSize);
                pNativeData += (int)proxysetting.ProxyBypassListSize;
                proxysetting.AutoConfigUrlSize = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                proxysetting.AutoConfigUrl = Marshal.PtrToStringAnsi(pNativeData, (int)proxysetting.AutoConfigUrlSize);
                pNativeData += (int)proxysetting.AutoConfigUrlSize;
                proxysetting.aFlag = (uint)Marshal.ReadInt32(pNativeData);
                pNativeData += 4;
                proxysetting.Padding = new byte[28];
                Marshal.Copy(pNativeData, proxysetting.Padding, 0, 28);

                return proxysetting;
            }
        }
    }
}

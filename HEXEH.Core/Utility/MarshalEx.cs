using System.Runtime.InteropServices;

namespace HEXEH.Core.Utility
{
    internal static class MarshalEx
    {
        public static ulong ReadUInt64BE(IntPtr ptr)
        {
            var bytes = new byte[8];
            Marshal.Copy(ptr, bytes, 0, bytes.Length);
            Array.Reverse(bytes);
            return BitConverter.ToUInt64(bytes);
        }

        public static uint ReadUInt32BE(IntPtr ptr)
        {
            var bytes = new byte[4];
            Marshal.Copy(ptr, bytes, 0, bytes.Length);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes);
        }

        public static ushort ReadUInt16BE(IntPtr ptr)
        {
            var bytes = new byte[2];
            Marshal.Copy(ptr, bytes, 0, bytes.Length);
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes);
        }

    }
}

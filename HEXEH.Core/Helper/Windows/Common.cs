using System.Runtime.InteropServices;

namespace HEXEH.Core.Helper.Windows.Common
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct FILETIME
    {
        [FieldOffset(0)]
        public ulong qwDateTime;
        [FieldOffset(0)]
        public uint dwDateTimeL;
        [FieldOffset(4)]
        public uint dwDateTimeH;

        public override string ToString()
        {
            return DateTime.Parse("1601-1-1").AddTicks((long)qwDateTime).ToString();
        }
    }
}

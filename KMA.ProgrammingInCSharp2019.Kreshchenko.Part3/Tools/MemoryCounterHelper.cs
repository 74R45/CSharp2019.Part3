using System.Runtime.InteropServices;

namespace KMA.ProgrammingInCSharp2019.Kreshchenko.Part3.Tools
{
    internal static class MemoryCounterHelper
    {
        private static MemoryStatusEx _memoryStatusEx;

        public static long GetTotalPhysicalMemory()
        {
            if (_memoryStatusEx == null)
            {
                _memoryStatusEx = new MemoryStatusEx();
                GlobalMemoryStatusEx(_memoryStatusEx);
            }

            return _memoryStatusEx.ullTotalPhys;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MemoryStatusEx
        {
            private int dwLength;
            public int dwMemoryLoad;
            public long ullTotalPhys;
            public long ullAvailPhys;
            public long ullTotalPageFile;
            public long ullAvailPageFile;
            public long ullTotalVirtual;
            public long ullAvailVirtual;
            public long ullAvailExtendedVirtual;
            public MemoryStatusEx()
            {
                dwLength = Marshal.SizeOf(typeof(MemoryStatusEx));
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void GlobalMemoryStatusEx([In, Out] MemoryStatusEx lpBuffer);
    }
}

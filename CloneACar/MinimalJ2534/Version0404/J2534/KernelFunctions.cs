using System;
using System.Runtime.InteropServices;

namespace Minimal_J2534_0404
{
    internal class KernelFunctions
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = false)]
        public static extern IntPtr GetProcAddressOrdinal(IntPtr hModule, int ordinalNumber);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }
}
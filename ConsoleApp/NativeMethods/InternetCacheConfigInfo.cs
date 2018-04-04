using System;
using System.Runtime.InteropServices;

namespace NativeMethods
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct InternetCacheConfigInfo
    {
        internal uint dwStructSize;

        internal uint dwContainer;

        internal uint dwQuota;

        internal uint dwReserved4;

        [MarshalAs(UnmanagedType.Bool)]
        internal bool fPerUser;

        internal uint dwSyncMode;

        internal uint dwNumCachePaths;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string CachePath;

        internal uint dwCacheSize;

        internal uint dwNormalUsage;

        internal uint dwExemptUsage;
    }

}
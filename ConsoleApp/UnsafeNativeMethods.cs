using System.Runtime.InteropServices;
using System.Security;

namespace ConsoleApp1
{
    internal class UnsafeNativeMethods
    {
        // MS.Win32.UnsafeNativeMethods
        [SecurityCritical, SuppressUnmanagedCodeSecurity]
        [DllImport("Wininet.dll", EntryPoint = "GetUrlCacheConfigInfoW", SetLastError = true)]
        internal static extern bool GetUrlCacheConfigInfo(ref NativeMethods.InternetCacheConfigInfo pInternetCacheConfigInfo, ref uint cbCacheConfigInfo, uint fieldControl);

    }
}
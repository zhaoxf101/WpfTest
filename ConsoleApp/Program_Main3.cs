using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program_Main3
    {
        static void Main()
        {
            var uri = InternetCacheFolder();



            Console.ReadKey();
        }


        //[SecurityCritical]
        static Uri InternetCacheFolder()
        {
            // copied value 260 from orginal implementation in BitmapDownload.cs 
            const int maxPathSize = 260;
            const UInt32 fieldControl = (UInt32)maxPathSize;

            NativeMethods.InternetCacheConfigInfo icci =
                new NativeMethods.InternetCacheConfigInfo();

            Console.WriteLine("new NativeMethods.InternetCacheConfigInfo();");

            icci.CachePath = new string(new char[maxPathSize]);

            UInt32 size = (UInt32)Marshal.SizeOf(icci);
            icci.dwStructSize = size;

            bool passed = UnsafeNativeMethods.GetUrlCacheConfigInfo(
                ref icci,
                ref size,
                fieldControl);

            Console.WriteLine("bool passed = UnsafeNativeMethods.GetUrlCacheConfigInfo(");

            Console.WriteLine($"passed: {passed}");
            if (!passed)
            {
                var error = Marshal.GetLastWin32Error();
                Console.WriteLine($"error: {error}");


                int hr = Marshal.GetHRForLastWin32Error();

                Console.WriteLine($"hr: {hr}");
                if (hr != 0)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }

            return new Uri(icci.CachePath);

        }
    }
}

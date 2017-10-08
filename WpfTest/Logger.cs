using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MarketingPlatform.Client
{
    static class Logger
    {
        public static void Log(string message, [CallerMemberName] String callerMemberName = null)
        {
            Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + $"{callerMemberName}.{message}");
        }
    }
}

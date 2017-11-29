using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CustomBA
{
    static class Logger
    {
        public static void Log(string message)
        {
            Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + message);
        }
    }
}

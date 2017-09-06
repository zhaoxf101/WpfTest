using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program_Main3
    {
        static void Main()
        {
            Debug.Listeners.Add(new TextWriterTraceListener("log.log"));
            Debug.AutoFlush = true;

            Debug.WriteLine("Hello");

            
            int? i = 13;

            if (i == null)
            {
                Trace.Write("");
            }

            
        }

        [Conditional("DEBUG")]
        static void Hello(int i)
        {
            Trace.WriteLine("HHH");
        }

    }
}

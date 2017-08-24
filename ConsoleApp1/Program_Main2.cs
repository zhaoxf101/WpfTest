using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program_Main2
    {
        private static TraceSource mySource = new TraceSource("TraceSourceApp");

        static void Main2()
        {
            mySource.Switch = new SourceSwitch("sourceSwitch", "Error");
            mySource.Listeners.Remove("Default");

            TextWriterTraceListener textListener = new TextWriterTraceListener("myListener.log");
            textListener.TraceOutputOptions = TraceOptions.DateTime | TraceOptions.Callstack;
            textListener.Filter = new EventTypeFilter(SourceLevels.Error);
            mySource.Listeners.Add(textListener);

            ConsoleTraceListener console = new ConsoleTraceListener(false);
            console.Filter = new EventTypeFilter(SourceLevels.Information);
            console.Name = "console";
            mySource.Listeners.Add(console);
            Activity1();

            // Set the filter settings for the 
            // console trace listener.
            mySource.Listeners["console"].Filter = new EventTypeFilter(SourceLevels.Critical);
            Activity2();

            // Allow the trace source to send messages to 
            // listeners for all event types. 
            mySource.Switch.Level = SourceLevels.All;

            // Change the filter settings for the console trace listener.
            mySource.Listeners["console"].Filter = new EventTypeFilter(SourceLevels.Information);
            Activity3();
            mySource.Close();
        }
        static void Activity1()
        {
            mySource.TraceEvent(TraceEventType.Error, 1, "Error message.");
            mySource.TraceEvent(TraceEventType.Warning, 2, "Warning message.");
        }
        static void Activity2()
        {
            mySource.TraceEvent(TraceEventType.Critical, 3, "Critical message.");
            mySource.TraceInformation("Informational message.");
        }
        static void Activity3()
        {
            mySource.TraceEvent(TraceEventType.Error, 4, "Error message.");
            mySource.TraceInformation("Informational message.");
        }
    }
}

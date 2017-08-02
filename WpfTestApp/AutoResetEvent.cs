using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WpfTestApp
{
    public class WaitHandlerExample
    {
        public static AutoResetEvent waitHandler;
        public static ManualResetEvent manualWaitHandler;

        public static void ThreadPoolMain()
        {
            waitHandler = new AutoResetEvent(false);
            manualWaitHandler = new ManualResetEvent(false);

            // Queue the task. 
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc));
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc2));

            Console.WriteLine("Main thread does some work, then waiting....");
            manualWaitHandler.WaitOne();
            //waitHandler.Reset(); 
            manualWaitHandler.WaitOne();
            //waitHandler.Reset(); 
            Console.WriteLine("Main thread exits.");
        }

        // This thread procedure performs the task. 
        public static void ThreadProc(Object stateInfo)
        {
            Thread.Sleep(1000);
            Console.WriteLine("Hello from the thread pool.");
            //waitHandler.Set();        // 
            manualWaitHandler.Set();//过去了，但是没关，也就是说 信号还是开着的。 
                                    //manualWaitHandler.Reset(); 
        }
        public static void ThreadProc2(object stateInfo)
        {
            Thread.Sleep(100);
            Console.WriteLine("Hello from the thread Pool2");
            //waitHandler.Set(); 
            manualWaitHandler.Set();//过去了，但是没有关 
        }
    }
}

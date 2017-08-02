using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UpdateService
{
    static class Program
    {
        static Mutex _Mutex;

        static AutoResetEvent _AutoResetEvent;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {

            bool isAppRunning;
            _Mutex = new Mutex(true, "IMS_UpdateService", out isAppRunning);
            if (!isAppRunning)
            {
                Environment.Exit(0);
            }

            _AutoResetEvent = new AutoResetEvent(false);

            var updateService = new UpdateService();

            updateService.Start(args);


            _AutoResetEvent.WaitOne();
        }
    }
}

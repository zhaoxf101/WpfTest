using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateService
{
    class TaskService
    {
        Task _listenerTask;

        List<string> _taskJsonFileNameList = new List<string>();
        string _taskJsonString;

        public void Start()
        {
            ReadTask();

            _listenerTask = Task.Factory.StartNew(Listener, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {

        }


        public void Suspend()
        {

        }

        public void Resume()
        {
            Start();
        }


        protected void Listener()
        {
            try
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var entryPath = Path.Combine(baseDirectory, "MarketingPlatForm.TaskExeBLL.dll");
                var entryAssembly = Assembly.LoadFrom(entryPath);

                //var entryAssembly = Assembly.Load("MarketingPlatForm.TaskExeBLL");

                var entryMainType = entryAssembly.GetType("MarketingPlatForm.TaskExeBLL.TaskMain");
                var entryMain = entryMainType.GetMethod("Start", BindingFlags.Static | BindingFlags.Public);
                var result = entryMain.Invoke(null, new[] { _taskJsonString });

                using (var stream = new StreamWriter("log.txt", true))
                {
                    stream.WriteLine(result);
                }

                Thread.Sleep(Timeout.Infinite);
            }
            catch
            {

            }
        }

        protected void ReadTask()
        {
            try
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                var files = Directory.GetFiles(baseDirectory, "*.json");
                if (files.Length > 0)
                {
                    _taskJsonString = File.ReadAllText(files[0], Encoding.Default);

                }

            }
            catch
            {

            }
        }

    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    public class PerformanceUtil
    {
        static PerformanceCounter _Counter1 = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
        static PerformanceCounter _Counter2 = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        static PerformanceCounter _Counter3 = new PerformanceCounter("Memory", "Available MBytes");


        static ConcurrentQueue<PerformanceRecord> _Cache = new ConcurrentQueue<PerformanceRecord>();
        static Timer _Timer = new Timer(Log);

        static void Log(object state)
        {
            _Cache.Enqueue(GetPerformanceRecord());
        }

        static PerformanceRecord GetPerformanceRecord()
        {
            try
            {
                return new PerformanceRecord
                {
                    CPUUsage = (int)_Counter1.NextValue(),
                    MemoryUsage = (int)_Counter2.NextValue(),
                    MemoryAvailable = (int)_Counter3.NextValue()
                };
            }
            catch
            {
                return new PerformanceRecord();
            }
        }

        public static void StartRecord()
        {
            // 清空容器。
            while (_Cache.TryDequeue(out PerformanceRecord result))
            {

            }
            _Timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));
        }

        public static void StopRecord()
        {
            _Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void ReportRecord()
        {
            if (_Cache.Count > 0)
            {
                var data = JsonConvert.SerializeObject(_Cache);

                // 这里需要把数据发送到服务器。

            }
        }

        public static bool CheckMachine()
        {
            var list = new List<PerformanceRecord>();

            // 抛弃第一个样本。
            GetPerformanceRecord();
            Thread.Sleep(1000);

            // 采样前10秒数据
            for (int i = 0; i < 10; i++)
            {
                list.Add(GetPerformanceRecord());
                Thread.Sleep(1000);
            }

            var memoryAverage = list.Average(p => p.MemoryAvailable);
            var cpuAverage = list.Average(p => p.CPUUsage);

            // 如果可用内存低于 1000M 或者 CPU 使用率大于 50%，不做任务。
            if (memoryAverage < 1000 || cpuAverage > 50)
            {
                // 需要报告这组数据。

                return false;
            }

            return true;
        }
    }
}

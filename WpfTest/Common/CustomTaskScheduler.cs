using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketingPlatform.Client.Common
{

    public class CustomTaskScheduler : TaskScheduler, IDisposable
    {
        static CustomTaskScheduler _Current;

        public new static CustomTaskScheduler Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new CustomTaskScheduler(4);
                }

                return _Current;
            }
        }

        //调用Task的线程
        Thread[] _threads;

        //Task Collection
        BlockingCollection<Task> _tasks = new BlockingCollection<Task>();

        int _concurrencyLevel;


        //设置schedule并发
        public CustomTaskScheduler(int concurrencyLevel)
        {
            _threads = new Thread[concurrencyLevel];
            _concurrencyLevel = concurrencyLevel;

            for (int i = 0; i < concurrencyLevel; i++)
            {
                _threads[i] = new Thread(() =>
                {
                    foreach (Task task in _tasks.GetConsumingEnumerable())
                        this.TryExecuteTask(task);

                })
                {
                    IsBackground = true
                };

                _threads[i].Start();
            }
        }

        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);
        }


        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {

            if (_threads.Contains(Thread.CurrentThread)) return TryExecuteTask(task);

            return false;
        }

        public override int MaximumConcurrencyLevel
        {
            get
            {
                return _concurrencyLevel;
            }
        }


        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasks.ToArray();
        }


        public void Dispose()
        {
            this._tasks.CompleteAdding();
            foreach (Thread t in _threads)
            {
                t.Join();
            }

        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketingPlatform.Client.Common
{
    public static class Logger
    {
        const int LogMaxCountInQueue = 1000;
        const int MinimumLogMaxCountInQueue = 1;

        static int _LogMaxCountInQueue = LogMaxCountInQueue;
        static ConcurrentQueue<string> _Log = new ConcurrentQueue<string>();
        static Task _Task;

        // 0 表示 Task 当前未在写入日志；1 表示 Task 当前正在写入日志。
        static int _TaskLock = 0;

        public static string LoggerFileName { get; set; }

        public static void Log(string message, bool appendTime = true, bool appendCaller = true, [CallerMemberName] String callerMemberName = null)
        {
            var dateTime = appendTime ? DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") : "";
            var caller = appendCaller ? callerMemberName + "." : "";
            message = dateTime + caller + message;

            Debug.WriteLine(message);

            if (!string.IsNullOrEmpty(LoggerFileName))
            {
                while (_Log.Count >= _LogMaxCountInQueue)
                {
                    _Log.TryDequeue(out string result);
                }

                _Log.Enqueue(message);

                if (Interlocked.CompareExchange(ref _TaskLock, 1, 0) == 0)
                {
                    _Task = Task.Factory.StartNew(LogToFile);
                }
            }
        }

        static void LogToFile()
        {
            try
            {
                using (var stream = new StreamWriter(LoggerFileName, true))
                {
                    var log = "";
                    while (_Log.TryDequeue(out log))
                    {
                        stream.WriteLine(log);
                    }

                    stream.Flush();
                }
            }
            catch (Exception)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(LoggerFileName);
                    var extension = Path.GetExtension(LoggerFileName);
                    var path = Path.GetDirectoryName(LoggerFileName);

                    using (var stream = new StreamWriter($"{Path.Combine(path, fileName + Guid.NewGuid() + extension)}", true))
                    {
                        var log = "";
                        while (_Log.TryDequeue(out log))
                        {
                            stream.WriteLine(log);
                        }

                        stream.Flush();
                    }
                }
                catch (Exception)
                {
                }
            }
            finally
            {
                Thread.VolatileWrite(ref _TaskLock, 0);
            }
        }

        public static void Flush()
        {
            if (!string.IsNullOrEmpty(LoggerFileName))
            {
                if (Interlocked.CompareExchange(ref _TaskLock, 1, 0) == 0)
                {
                    _Task = Task.Factory.StartNew(LogToFile);
                }

                if (_Task != null)
                {
                    _Task.Wait(1000);
                }
            }
        }

        public static void LogException(Exception exception)
        {
            var msg = GetExceptionDetails(exception);
            Log(msg, true, false);
        }

        public static void LogException(Exception exception, string fileName)
        {
            try
            {
                using (var writer = new StreamWriter(fileName, true))
                {
                    var msg = GetExceptionDetails(exception);

                    writer.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + msg);
                    writer.Flush();
                }
            }
            catch (Exception)
            {
            }
        }


        public static void LogExceptionAsync(Exception exception, string fileName)
        {
            Task.Factory.StartNew(() =>
            {
                LogException(exception, fileName);

            }, TaskCreationOptions.None);
        }

        public static string GetExceptionDetails(Exception exception)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Exception: {exception?.Message}");
            builder.AppendLine();

            var level = 0;
            while (exception != null)
            {
                var padding = new string(' ', level * 2);
                builder.AppendLine(padding + "Exception: " + exception.GetType().ToString() + " Message: " + exception.Message);
                builder.AppendLine(padding + "StackTrace: ");
                builder.AppendLine(exception.StackTrace);
                builder.AppendLine();

                exception = exception.InnerException;
                level++;

                if (level > 10)
                {
                    break;
                }
            }

            return builder.ToString();
        }
    }
}

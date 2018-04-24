using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MarketingPlatform.Client.Common
{
    public static class Report
    {
        const string url = "api/common/report";

        public static void ReportInfoAsync(string message = "")
        {
            Task.Factory.StartNew(() =>
            {
                ReportInfo(message);
            });
        }

        public static void ReportInfo(string message = "")
        {
            ReportMessage(message, "I");
        }

        public static void ReportInfo(string message, int timeout)
        {
            Task.Factory.StartNew(() =>
            {
                ReportMessage(message, "I");
            }).Wait(timeout);
        }

        public static void ReportExceptionAsync(string message = "")
        {
            Task.Factory.StartNew(() =>
            {
                ReportException(message);
            });
        }

        public static void ReportException(string message = "")
        {
            ReportMessage(message, "E");
        }

        public static void ReportException(string message, int timeout)
        {
            Task.Factory.StartNew(() =>
            {
                ReportMessage(message, "E");
            }).Wait(timeout);
        }

        static void ReportMessage(string message, string tag)
        {
            var list = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrEmpty(message))
            {
                list.Add(new KeyValuePair<string, string>("", message));
            }
        }
    }
}

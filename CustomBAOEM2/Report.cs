using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CustomBA
{
    static class Report
    {
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
            var url = $"{SysParam.PrefixUrl}api/common/report";
            if (!string.IsNullOrEmpty(message))
            {
                url += $"?={HttpUtility.UrlEncode(message, Encoding.UTF8)}";
            }
            var response = HttpHelper.HttpGet<BaseModel<object>>(url, tag);
        }
    }
}

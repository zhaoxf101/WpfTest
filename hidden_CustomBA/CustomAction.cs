﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomBA
{
    static class CustomAction
    {
        readonly static string BackupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IMS_UpdateService");

        const int RetryCount = 5;
        const int WaitTimeSeconds = 2;

        internal static void Backup(string installFolder)
        {
            Trace.WriteLine($"Backup. installFolder: {installFolder}");

            var url = "http://ims-api.xyunhui.com/api/common/bootstrap";
            var autoStart = false;

            Task.Factory.StartNew(() =>
            {
                Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "Start Get Request.");

                var response = HttpHelper.HttpGet<BaseModel<KeyValuePair[]>>(url);
                if (response != null && response.IsSuccess)
                {
                    var result = response.Result;
                    autoStart = result.SingleOrDefault(p => p.Key == "ClientAutoStart")?.Value == "1";
                }
                Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "Get Request Complete.");

            }).Wait(5000);

            if (autoStart)
            {
                try
                {
                    var files = Directory.GetFiles(installFolder);
                    if (!Directory.Exists(BackupFolder))
                    {
                        Directory.CreateDirectory(BackupFolder);
                    }

                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);
                        File.Copy(file, Path.Combine(BackupFolder, fileName), true);
                    }

                    var updateServiceFileName = Path.Combine(BackupFolder, "UpdateService.exe");

                    //var pi = new ProcessStartInfo
                    //{
                    //    FileName = Path.Combine(BackupFolder, "UpdateService.exe")
                    //};

                    //Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "Start UpdateService.");
                    //Process.Start(pi);

                    bool success = Util.RegisterAutoStart(updateServiceFileName, "IMS_UpdateService", true);
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "Add UpdateService autostart. Result: " + success);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. Backup Failed. Exception:  " + ex.Message);
                }
            }
        }

        internal static void RemoveBackup()
        {
            Trace.WriteLine($"RemoveBackup. ");

            var retryCount = RetryCount;

            while (retryCount > 0)
            {
                KillRelativeProcesses();

                try
                {
                    if (Directory.Exists(BackupFolder))
                    {
                        Directory.Delete(BackupFolder, true);
                        Trace.WriteLine($"Backup Folder Removed. ");
                    }
                    break;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. RemoveBackup Failed. Exception:  " + ex.Message);
                    retryCount--;
                    Thread.Sleep(WaitTimeSeconds * 1000);
                }
            }

            Trace.WriteLine($"RemoveBackup Complete. ");
        }

        internal static void KillRelativeProcesses()
        {
            Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. Begin KillRelativeProcesses.");

            var processes = Process.GetProcessesByName("UpdateService");
            foreach (Process process in processes)
            {
                try
                {
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. Begin Kill UpdateService Process.");
                    process.Kill();
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. End Kill UpdateService Process.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. Kill UpdateService Process Failed. Exception: " + ex.Message);
                }
            }

            processes = Process.GetProcessesByName("MarketingPlatform.Client");
            foreach (Process process in processes)
            {
                Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. Begin Send shutdown to MarketingPlatform.Client Process.");

                try
                {
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. Begin Kill MarketingPlatform.Client Process.");
                    process.Kill();
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. End Kill MarketingPlatform.Client Process.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. Kill MarketingPlatform.Client Process Failed. Exception: " + ex.Message);
                }
            }

            processes = Process.GetProcessesByName("phantomjs");
            foreach (Process process in processes)
            {
                try
                {
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. Begin Kill phantomjs Process.");
                    process.Kill();
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. End Kill phantomjs Process.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. Send shutdown to phantomjs Process Failed. Exception: " + ex.Message);
                }
            }

            Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. End KillRelativeProcesses.");
        }

        internal static void CleanUp(string installFolder)
        {
            try
            {
                var result = Util.RegisterAutoStart("", "IMS_MarketingPlatform.Client", false);
                Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. RegisterAutoStart. result: " + result);

                //Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. Begin Directory.Delete.");
                //Directory.Delete(installFolder, true);
                //Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. End Directory.Delete.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. RegisterAutoStart. Exception: " + ex.Message);

                //Trace.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "CustomAction. Directory.Delete. Exception: " + ex.Message);
            }
        }

    }
}

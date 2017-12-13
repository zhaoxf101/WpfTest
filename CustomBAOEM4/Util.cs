using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace CustomBA
{
    class Util
    {
        public static bool RegisterAutoStart(string fileName, string nodeName, bool autoStart)
        {
            var isSuccess = false;

            try
            {
                using (var keyHKLM = Microsoft.Win32.Registry.CurrentUser)
                {
                    var keyRun = keyHKLM.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    if (autoStart)
                    {
                        keyRun.SetValue(nodeName, $@"""{fileName}""");//加入注册，参数一为注册节点名称(随意)  
                    }
                    else
                    {
                        keyRun.DeleteValue(nodeName, false);//删除该注册节点    
                    }

                    isSuccess = true;
                }
            }
            catch (Exception)
            {
                return isSuccess;
            }

            return isSuccess;
        }

        public static string GetSystemId()
        {
            var list = new List<string>();

            try
            {
                var mc = new ManagementClass("Win32_DiskDrive");
                var mci = mc.GetInstances();
                var model = "";
                var number = "";

                foreach (ManagementObject mo in mci)
                {
                    try
                    {
                        model = mo.Properties["Model"]?.Value?.ToString().Trim();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Win32_DiskDrive.Model. ex: {ex.Message}");
                    }

                    try
                    {
                        number = mo.Properties["SerialNumber"]?.Value?.ToString().Trim();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Win32_DiskDrive.SerialNumber. ex: {ex.Message}");
                    }

                    list.Add($"{model}_{number}");
                }

            }
            catch (Exception ex)
            {
                Logger.Log($"Win32_DiskDrive. ex: {ex.Message}");
            }

            try
            {
                var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var mci = mc.GetInstances();

                foreach (ManagementObject mo in mci)
                {
                    try
                    {
                        var ipEnabled = mo["IPEnabled"];
                        if (ipEnabled == null || !(bool)ipEnabled)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Win32_NetworkAdapterConfiguration.IPEnabled. ex: {ex.Message}");
                        continue;
                    }

                    try
                    {
                        list.Add($"{mo["MacAddress"]?.ToString().Trim()}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Win32_NetworkAdapterConfiguration.MacAddress. ex: {ex.Message}");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log($"Win32_DiskDrive. ex: {ex.Message}");
            }

            var result = string.Join("|", list);
            //if (result.Length > Const.DeviceIdMaxLength)
            //{
            //    result = result.Substring(0, Const.DeviceIdMaxLength);
            //}

            return result;
        }

        public static string GetSystemIdFromRegistry()
        {
            try
            {
                using (var keyHKLM = Microsoft.Win32.Registry.CurrentUser)
                {
                    var key = keyHKLM.CreateSubKey(@"SOFTWARE\MarketingPlatForm.Client");
                    return $"{key.GetValue("DeviceId")}";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}

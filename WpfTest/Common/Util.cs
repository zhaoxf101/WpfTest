using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MarketingPlatform.Client.Common
{
    public static class Util
    {
        const string Key = @"IMSNB]6,YF}+efcaj{+oESb9d8>Z'e9M";
        const string IV = @"IMS~f4,Ir)b$=pkf";

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
            //if (result.Length > SysParam.DeviceIdMaxLength)
            //{
            //    result = result.Substring(0, SysParam.DeviceIdMaxLength);
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

        public static bool SetSystemIdToRegistry(string deviceId)
        {
            var isSuccess = false;

            try
            {
                using (var keyHKLM = Microsoft.Win32.Registry.CurrentUser)
                {
                    var key = keyHKLM.CreateSubKey(@"SOFTWARE\MarketingPlatForm.Client");
                    key.SetValue("DeviceId", deviceId);

                    isSuccess = true;
                }
            }
            catch (Exception)
            {
                return isSuccess;
            }

            return isSuccess;
        }

        public static string EncryptString(string str)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(str);
            var hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

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

        public static bool CheckAutoStart(string nodeName, out bool autoStart)
        {
            autoStart = false;

            var isSuccess = false;

            try
            {
                using (var keyHKLM = Microsoft.Win32.Registry.CurrentUser)
                {
                    var keyRun = keyHKLM.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

                    var value = keyRun.GetValue(nodeName);
                    if (value != null)
                    {
                        autoStart = true;
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

        public static string GetFileMd5(string fileName)
        {
            try
            {
                var md5 = MD5.Create();

                using (var stream = File.OpenRead(fileName))
                {
                    var hash = md5.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sb.Append(hash[i].ToString("X2"));
                    }

                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception: {ex.Message}");
                return "";
            }
        }

        public static string GetMd5(byte[] data)
        {
            try
            {
                var md5 = MD5.Create();

                var hash = md5.ComputeHash(data);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception: {ex.Message}");
                return "";
            }
        }

        public static byte[] AESEncrypt(byte[] data)
        {
            byte[] bKey = Encoding.UTF8.GetBytes(Key);
            byte[] bIV = Encoding.UTF8.GetBytes(IV);

            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(data, 0, data.Length);
                        cStream.FlushFinalBlock();

                        aes.Clear();
                        return mStream.ToArray();
                    }
                }
            }
            catch
            {
            }
            return new byte[0];
        }

        public static byte[] AESDecrypt(byte[] data)
        {
            byte[] bKey = Encoding.UTF8.GetBytes(Key);
            byte[] bIV = Encoding.UTF8.GetBytes(IV);

            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(data, 0, data.Length);
                        cStream.FlushFinalBlock();
                        aes.Clear();
                        return mStream.ToArray();
                    }
                }
            }
            catch
            {
            }

            return new byte[0];
        }

        /// <summary>
        /// Http下载文件
        /// </summary>
        public static bool HttpDownloadFile(string url, string path)
        {
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求

                using (Stream responseStream = response.GetResponseStream())
                {
                    //创建本地文件写入流
                    using (Stream stream = new FileStream(path, FileMode.Create))
                    {
                        byte[] bArr = new byte[1024];
                        int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        while (size > 0)
                        {
                            stream.Write(bArr, 0, size);
                            size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool AreFileNameEquals(string name1, string name2)
        {
            var p = Path.GetFileNameWithoutExtension(name1);
            var q = Path.GetFileNameWithoutExtension(name2);

            return p.Equals(q, StringComparison.OrdinalIgnoreCase);
        }
    }
}

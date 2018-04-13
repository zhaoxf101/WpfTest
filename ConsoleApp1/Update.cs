using MarketingPlatform.Client.Common;
using MarketingPlatform.Client.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    public enum UpdateResult
    {
        Success,
        Failed,
        Checking,
    }

    public static class Update
    {
        static string _PrefixUrl = "http://task-api.xyunhui.com/";
        static string _ChromiumDir = "ChromeCore";

        static string _FileChromiumAll = "ChromeCore_all.zip";

        public static string PrefixUrl { get => _PrefixUrl; set => _PrefixUrl = value; }

        public static Mutex _Mutex;

        public static UpdateResult StartUpdate()
        {
            try
            {
                _Mutex = new Mutex(true, "Global\\" + "IMSTaskUpdateCheckingLock", out bool createdNew);

                if (!createdNew)
                {
                    return UpdateResult.Checking;
                }
            }
            catch (Exception)
            {
                // 在xp多用户登录的时候，会发生访问 Global\ 路径失败的错误。
                // 不管怎样，现在可以保证多用户的情况下单实例了。
                return UpdateResult.Failed;
            }

            try
            {
                var updateUrl = $"{PrefixUrl}api/updatefiles/getupdatechromecoreassemblies";
                var updateResponse = HttpClientHelper.GetResponseWithJsonAccept<BaseModel<AssemblyInfo[]>>(updateUrl);

                if (updateResponse != null && updateResponse.IsSuccess)
                {
                    var _assemblyInfoArray = updateResponse.Result;

                    var updateList = new List<AssemblyInfo>();
                    var targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _ChromiumDir);

                    if (!Directory.Exists(targetDir))
                    {
                        Directory.CreateDirectory(targetDir);
                    }

                    foreach (var item in _assemblyInfoArray)
                    {
                        if (item.Name.Equals(_FileChromiumAll, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var fileName = Path.Combine(targetDir, item.Name);

                        //Logger.Log($"Current Update File: {item.Name} Md5: {item.Md5}");

                        var md5 = "";
                        if (File.Exists(fileName))
                        {
                            md5 = Util.GetFileMd5(fileName);
                            //Logger.Log($"Current File: {fileName} Md5: {md5}");
                        }

                        if (!string.Equals(md5, item.Md5, StringComparison.OrdinalIgnoreCase))
                        {
                            updateList.Add(item);
                        }
                    }

                    if (updateList.Count > 0)
                    {
                        // 需要下载整个压缩包
                        if (updateList.Any(p => p.Name.Equals("chrome.dll", StringComparison.OrdinalIgnoreCase) || p.Name.Equals("chrome_child.dll", StringComparison.OrdinalIgnoreCase)))
                        {
                            var item = _assemblyInfoArray.Where(p => p.Name.Equals(_FileChromiumAll, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (item != null)
                            {
                                var targetFile = Path.Combine(targetDir, _FileChromiumAll);
                                var md5 = "";
                                if (File.Exists(targetFile))
                                {
                                    md5 = Util.GetFileMd5(targetFile);
                                    if (string.Equals(md5, item.Md5, StringComparison.OrdinalIgnoreCase))
                                    {
                                        Util.UnZipFile(targetFile);
                                        return UpdateResult.Success;
                                    }
                                }

                                //Logger.Log($"Begin download. Url: {item.Url}");
                                var downloaded = Util.HttpDownloadFile(item.Url, targetFile);

                                if (!downloaded)
                                {
                                    //Logger.Log($"File {item.Name} download failed. Returned.");

                                    //Logger.Log($"End Update. result: {result}");
                                    return UpdateResult.Failed;
                                }

                                md5 = Util.GetFileMd5(targetFile);
                                //Logger.Log($"targetFile: {targetFile} Md5: {md5}");
                                if (!string.Equals(md5, item.Md5, StringComparison.OrdinalIgnoreCase))
                                {
                                    //Logger.Log($"targetFile Md5 checking failed.");

                                    return UpdateResult.Failed;
                                }
                                Util.UnZipFile(targetFile);
                                return UpdateResult.Success;
                            }
                        }

                        foreach (var item in updateList)
                        {
                            var fileName = Path.Combine(targetDir, item.Name);

                            //Logger.Log($"Begin download. Url: {item.Url}");
                            var downloaded = Util.HttpDownloadFile(item.Url, fileName);

                            if (!downloaded)
                            {
                                //Logger.Log($"File {item.Name} download failed. Returned.");

                                //Logger.Log($"End Update. result: {result}");
                                return UpdateResult.Failed;
                            }
                            var fileMd5 = Util.GetFileMd5(fileName);
                            //Logger.Log($"targetFile: {targetFile} Md5: {md5}");
                            if (!string.Equals(fileMd5, item.Md5, StringComparison.OrdinalIgnoreCase))
                            {
                                //Logger.Log($"targetFile Md5 checking failed.");

                                return UpdateResult.Failed;
                            }
                        }

                    }

                    return UpdateResult.Success;
                }
            }
            catch (Exception ex)
            {
                return UpdateResult.Failed;
            }
            finally
            {
                _Mutex.Dispose();
            }
            return UpdateResult.Failed;
        }
    }
}

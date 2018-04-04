using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ConsoleApp1
{
    public class BaseModel<T>
    {
        public bool IsSuccess
        {
            get
            {
                return Code == 0;
            }
        }

        public int Code { get; set; }

        public string Msg { get; set; }

        public T Result { get; set; }
    }

    public class KeyValuePair
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }


    class Program_Main1
    {
        string[] ParseCommandLine()
        {
            var list = new List<string>();

            var commandLine = Environment.CommandLine;
            var tokenStartedIndex = -1;

            var programStarted = false;
            var programEnded = false;

            var quotationStarted = false;

            var builder = new StringBuilder();
            for (int i = 0; i < commandLine.Length; i++)
            {
                var c = commandLine[i];
                if (c == '\"')
                {
                    if (quotationStarted)
                    {
                        if (programEnded)
                        {
                            list.Add(commandLine.Substring(tokenStartedIndex, i - tokenStartedIndex));
                            tokenStartedIndex = -1;
                        }

                        quotationStarted = false;
                    }
                    else
                    {
                        quotationStarted = true;
                    }

                    // Token 未开始
                    if (tokenStartedIndex == -1)
                    {
                        if (programEnded)
                        {
                            tokenStartedIndex = i;
                        }
                        else
                        {
                            programStarted = true;
                        }
                    }
                }
                else if (c == ' ' || c == '\t')
                {
                    if (quotationStarted)
                    {
                        continue;
                    }
                    else
                    {
                        if (tokenStartedIndex != -1)
                        {
                            list.Add(commandLine.Substring(tokenStartedIndex, i - tokenStartedIndex));
                            tokenStartedIndex = -1;
                        }
                    }
                }
                else
                {
                    if (!programStarted)
                    {
                        programStarted = true;
                    }
                }
            }

            if (tokenStartedIndex != -1)
            {
                list.Add(commandLine.Substring(tokenStartedIndex));
            }

            return list.ToArray();
        }

        static void Main1(string[] args)
        {
            //foreach (var item in args)
            //{
            //    Debug.WriteLine(item);
            //}

            //Debug.WriteLine("");

            //Debug.WriteLine(Environment.CommandLine);


            //int WS_POPUP = unchecked((int)0x80000000);

            //Console.WriteLine(WS_POPUP);

            //HttpDownloadFileHelper.HttpDownloadFile("http://ot4e7ngbr.bkt.clouddn.com/UpdateMarketingPlatForm.Client.exe", "Hello");

            //Debug.WriteLine("sdf");

            //var handle = new WindowInteropHelper(this).Handle;

            ////改变窗体的样式  
            //int style = User32.GetWindowLong(handle, User32.GWL_STYLE);
            //User32.SetWindowLong(handle, User32.GWL_STYLE, style | User32.WS_POPUP | User32.WS_THICKFRAME | User32.WS_MINIMIZEBOX);

            ////var extendedStyle = User32.GetWindowLong(handle, User32.GWL_EXSTYLE);
            ////User32.SetWindowLong(handle, User32.GWL_EXSTYLE, extendedStyle & (~User32.WS_EX_APPWINDOW) & (~User32.WS_EX_WINDOWEDGE) | User32.WS_EX_OVERLAPPEDWINDOW | User32.WS_EX_LAYERED);

            ////更新窗口的非客户区，以反映变化  
            //User32.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, User32.SWP_NOMOVE |
            //      User32.SWP_NOSIZE | User32.SWP_NOZORDER | User32.SWP_FRAMECHANGED);





            //var url = " http://data.zz.baidu.com/urls?site=www.xyunhui.com&token=hWO1fNTUUwrlZxGv";
            //if (url.StartsWith("https"))
            //{
            //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            //}

            //HttpClient httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri(url, uriKind: UriKind.Absolute);

            //var form = new MultipartFormDataContent();
            ////StreamContent fileContent = new StreamContent(File.OpenRead(path));
            ////fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            ////fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            ////fileContent.Headers.ContentDisposition.FileName = Path.GetFileName(path);
            //form.Add(new StringContent("www.xyunhui.com/doc.html", Encoding.UTF8, "multipart/form-data"), "site");

            //HttpResponseMessage response = null;
            //try
            //{
            //    response = httpClient.PostAsync(url, form).Result;
            //    Task<string> t = response.Content.ReadAsStringAsync();
            //    string s = t.Result;


            //}
            //catch
            //{

            //}

            //if (response.IsSuccessStatusCode)
            //{


            //}

            //var url = "http://ims-api.xyunhui.com/api/common/bootstrap";


            //if (autoStart)
            //{

            //}

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "Start Get Request.");

                var url = "http://localhost:16760/api/common/bootstrap";
                var response = HttpGet<BaseModel<KeyValuePair[]>>(url);
                var autoStart = false;
                if (response != null && response.IsSuccess)
                {
                    var result = response.Result;
                    autoStart = result.SingleOrDefault(p => p.Key == "ClientAutoStart")?.Value == "1";
                }
                Console.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "Get Request Complete.");

            }).Wait(5000);

            Console.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ") + "End Get Request.");

            Console.ReadKey();
        }


        public static T HttpGet<T>(string url)
        {
            var result = default(T);
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "Get";

                request.Headers.Add("XinYunHui-DeviceId", "Uninstall");
                request.Headers.Add("XinYunHui-DeviceType", Environment.OSVersion.ToString());
                request.Headers.Add("XinYunHui-Version", "Uninstall");

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求

                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var json = reader.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var jsonData = js.Deserialize<T>(json);
                        return jsonData;
                    }
                }
            }
            catch (Exception)
            {
            }

            return result;
        }


        static void M()
        {
            //HttpClient client = new HttpClient();
            ////准备POST的数据
            //MultipartFormDataContent httpcontent = new MultipartFormDataContent();
            //HttpContent accessContent = new StringContent(access_token);
            //accessContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            //httpcontent.Add(accessContent, "access_token");
            ////传输二进制图片
            //Stream stream = new MemoryStream(pic);
            //HttpContent piccontent = new StreamContent(stream);
            //piccontent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/form-data");
            //httpcontent.Add(statusContent, "status");
            ////发起POST连接
            //HttpResponseMessage response = await client.PostAsync("接口地址", httpcontent);
            ////返回的信息
            //responseBody = await response.Content.ReadAsStringAsync();
        }
    }
}

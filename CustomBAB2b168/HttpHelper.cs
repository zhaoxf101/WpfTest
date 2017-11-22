using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace CustomBA
{
    class HttpHelper
    {
        public static T HttpGet<T>(string url)
        {
            var result = default(T);
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "Get";

                request.Headers.Add("XinYunHui-DeviceId", Util.GetSystemId());
                request.Headers.Add("XinYunHui-DeviceType", $"{Environment.OSVersion}");
                request.Headers.Add("XinYunHui-Version", $"I{Assembly.GetExecutingAssembly().GetName().Version}");
                request.Headers.Add("XinYunHui-Tag", "U");

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
    }
}

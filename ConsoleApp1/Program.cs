using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {


        static void Main(string[] args)
        {
            var url = " http://data.zz.baidu.com/urls?site=www.xyunhui.com&token=hWO1fNTUUwrlZxGv";
            if (url.StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url, uriKind: UriKind.Absolute);

            var form = new MultipartFormDataContent();
            //StreamContent fileContent = new StreamContent(File.OpenRead(path));
            //fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            //fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            //fileContent.Headers.ContentDisposition.FileName = Path.GetFileName(path);
            form.Add(new StringContent("www.xyunhui.com/doc.html", Encoding.UTF8, "multipart/form-data"), "site");

            HttpResponseMessage response = null;
            try
            {
                response = httpClient.PostAsync(url, form).Result;
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;


            }
            catch
            {
                
            }

            if (response.IsSuccessStatusCode)
            {
               

            }

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

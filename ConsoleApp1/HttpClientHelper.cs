using MarketingPlatform.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace MarketingPlatform.Client.Common
{
    public class HttpClientHelper
    {
        static string _ClientVersion;
        static string _DeviceType;
        static string _DeviceId;
        static string _Tag;
        static string _AgentId;
        
        static string _LastToken;

        public static string LastToken
        {
            get { return _LastToken; }
            set { _LastToken = value; }
        }

        public static string DeviceId
        {
            get { return _DeviceId; }
            set { _DeviceId = value; }
        }

        public static string DeviceType
        {
            get { return _DeviceType; }
            set { _DeviceType = value; }
        }

        public static string ClientVersion
        {
            get { return _ClientVersion; }
            set { _ClientVersion = value; }
        }

        public static string AgentId
        {
            get { return _AgentId; }
            set { _AgentId = value; }
        }

        private static string _PrefixUrl;

        public static string PrefixUrl
        {
            get { return _PrefixUrl; }
            set { _PrefixUrl = value; }
        }

        private static HttpClient GetHttpClient(string url, string acceptHeader)
        {
            var checkUrl = _PrefixUrl;

            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                if (uri.IsAbsoluteUri)
                {
                    checkUrl = url;
                }
            }

            if (checkUrl.StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }

            HttpClient httpClient = new HttpClient();
            if (uri == null || !uri.IsAbsoluteUri)
            {
                httpClient.BaseAddress = new Uri(checkUrl, uriKind: UriKind.Absolute);
            }

            httpClient.DefaultRequestHeaders.Add("XinYunHui-DeviceId", _DeviceId);
            httpClient.DefaultRequestHeaders.Add("XinYunHui-DeviceType", _DeviceType);
            httpClient.DefaultRequestHeaders.Add("xinyunhui-Version", _ClientVersion);

            if (!string.IsNullOrEmpty(_Tag))
            {
                httpClient.DefaultRequestHeaders.Add("XinYunHui-Tag", _Tag);
            }

            if (!string.IsNullOrEmpty(_AgentId))
            {
                httpClient.DefaultRequestHeaders.Add("XinYunHui-AgentId", _AgentId);
            }

            if (!string.IsNullOrEmpty(_LastToken))
            {
                httpClient.DefaultRequestHeaders.Add("XinYunHui-Token", _LastToken);
                //Logger.Log($"url: {url} Token: {_LastToken}");
            }

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

            return httpClient;
        }

        private static HttpClient GetHttpClientWithJsonAccept(string url)
        {
            return GetHttpClient(url, "application/json");
        }

        private static T GetObjectFromHttpResponseMessage<T>(string url, HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("XinYunHui-Token", out IEnumerable<string> values))
            {
                _LastToken = values.First();
            }

            T result = default(T);
            try
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;
                //Logger.Log($"url: {url} t.Result: {t.Result}");

                result = JsonConvert.DeserializeObject<T>(s);
            }
            catch (Exception ex)
            {
                //Logger.Log($"JsonConvert.DeserializeObject<T> Exception: {ex.Message}");
                //Logger.LogException(ex);
            }

            return result;
        }

        public static T GetResponseWithJsonAccept<T>(string url, IEnumerable<KeyValuePair<string, string>> data = null)
                  where T : class, new()
        {
            var httpClient = GetHttpClientWithJsonAccept(url);

            var arguments = data?.Select(p => HttpUtility.UrlEncode(p.Key, Encoding.UTF8) + "=" + HttpUtility.UrlEncode(p.Value, Encoding.UTF8));
            if (arguments != null)
            {
                url += "?" + string.Join("&", arguments);
            }

            var result = default(T);
            HttpResponseMessage response = null;
            try
            {
                //Logger.Log($"Start Get Request. url: {url}");
                response = httpClient.GetAsync(url).Result;
                //Logger.Log($"End Get Request. url: {url}");
            }
            catch (Exception ex)
            {
                //Logger.Log($"httpClient.GetAsync(url).Result Exception: {ex.Message}");
                //Logger.LogException(ex);
                return result;
            }

            if (response.IsSuccessStatusCode)
            {
                result = GetObjectFromHttpResponseMessage<T>(url, response);
            }
            else
            {
                try
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    //Logger.Log($"url: {url} StatusCode: {response.StatusCode} t.Result: {t.Result}");
                }
                catch (Exception ex)
                {
                    //Logger.Log($"response.Content.ReadAsStringAsync Exception.");
                    //Logger.LogException(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// 发起post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static T PostResponseWithJsonAccept<T>(string url, IEnumerable<KeyValuePair<string, string>> data, string tag = "")
            where T : class, new()
        {
            _Tag = tag;
            HttpClient httpClient = GetHttpClientWithJsonAccept(url);
            _Tag = "";

            var content = "";
            var contentOriginal = "";

            if (data != null)
            {
                content = string.Join("&", data.Select(p => HttpUtility.UrlEncode(p.Key, Encoding.UTF8) + "=" + HttpUtility.UrlEncode(p.Value, Encoding.UTF8)));
                contentOriginal = string.Join("&", data.Select(p => p.Key + "=" + p.Value));
            }
            var httpContent = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");

            var result = default(T);
            HttpResponseMessage response = null;
            try
            {
                //Logger.Log("Start Post Request. url: " + url + " data: " + contentOriginal);
                response = httpClient.PostAsync(url, httpContent).Result;
                //Logger.Log("End Post Request. url: " + url);
            }
            catch
            {
                return result;
            }

            if (response.IsSuccessStatusCode)
            {
                result = GetObjectFromHttpResponseMessage<T>(url, response);
            }
            else
            {
                try
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    //Logger.Log($"url: {url} StatusCode: {response.StatusCode} t.Result: {t.Result}");
                }
                catch (Exception ex)
                {
                    //Logger.Log($"response.Content.ReadAsStringAsync Exception.");
                    //Logger.LogException(ex);
                }
            }

            return result;
        }

        public static T PostJsonWithJsonAccept<T>(string url, string data)
            where T : class, new()
        {
            var httpClient = GetHttpClientWithJsonAccept(url);
            var httpContent = new StringContent(data, Encoding.UTF8, "application/json");

            var result = default(T);
            HttpResponseMessage response = null;

            try
            {
                //Logger.Log("Start Post Request. url: " + url + " data: " + data);
                response = httpClient.PostAsync(url, httpContent).Result;
                //Logger.Log("End Post Request. url: " + url);
            }
            catch
            {
                return result;
            }

            if (response.IsSuccessStatusCode)
            {
                result = GetObjectFromHttpResponseMessage<T>(url, response);
            }
            else
            {
                try
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    //Logger.Log($"url: {url} StatusCode: {response.StatusCode} t.Result: {t.Result}");
                }
                catch (Exception ex)
                {
                    //Logger.Log($"response.Content.ReadAsStringAsync Exception.");
                    //Logger.LogException(ex);
                }
            }

            return result;
        }

        public static string GetResponse(string url)
        {
            var httpClient = new HttpClient();

            HttpResponseMessage response = null;
            try
            {
                //Logger.Log($"Start Get Request. url: {url}");
                response = httpClient.GetAsync(url).Result;
                //Logger.Log($"End Get Request. response: {response.ToString()}");
            }
            catch (Exception ex)
            {
                //Logger.Log($"httpClient.GetAsync(url).Result Exception: {ex.Message}");
                return "";
            }

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                try
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    //Logger.Log($"url: {url} StatusCode: {response.StatusCode} t.Result: {t.Result}");
                }
                catch (Exception ex)
                {
                    //Logger.Log($"response.Content.ReadAsStringAsync Exception.");
                    //Logger.LogException(ex);
                }
            }

            return "";
        }

    }
}

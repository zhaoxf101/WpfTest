using Newtonsoft.Json;
using Qiniu.Common;
using Qiniu.Http;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WpfTest.Controls.RichTextBox.Controls;

namespace WpfRichText
{
    class UploadImageManagerQiniu : IUploadImageManager
    {
        public void UploadImage(byte[] data, string key, out string url)
        {
            url = "";
            var bucket = "xyh-website";
            var domain = "img.xyunhui.com";

            if (string.IsNullOrWhiteSpace(key))
            {
                key = $"Image{DateTime.Now.ToString("yyMMddHHmmssfff")}{Guid.NewGuid().ToString().Substring(0, 3)}.jpg";
            }

            string ak = "daek9-W6ALpt1k9DR3c7wUC5tnGzNxtRmoZhOEhW";
            string sk = "QLJgX_5fbtmieCXjlHWnUi7vUqJ9s7S6dL1OsTYQ";
            Mac mac = new Mac(ak, sk);
            Auth auth = new Auth(mac);
            PutPolicy putPolicy = new PutPolicy();
            Config.SetZone(ZoneID.CN_South, true);
            putPolicy.Scope = $"{bucket}:{key}";

            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            var jstr = JsonConvert.SerializeObject(putPolicy);

            var token = Auth.CreateUploadToken(mac, jstr);
            var um = new UploadManager();

            HttpResult result = null;
            result = um.UploadData(data, key, token);
            if (result.Code != 200)
            {
                throw new UploadImageException("图片上传失败！");
            }

            url = $"http://{domain}/{HttpUtility.UrlEncode(key)}";
        }
    }
}

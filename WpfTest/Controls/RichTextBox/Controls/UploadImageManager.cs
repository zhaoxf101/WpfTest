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

namespace WpfRichText
{
    public interface IUploadImageManager
    {
        void UploadImage(byte[] data, string key, out string url);
    }
}

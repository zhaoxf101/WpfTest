using MarketingPlatform.Client.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MarketingPlatform.Client
{
    static class Util
    {
        public static readonly Random Random = new Random();

        public static string GetImageBase64String(Stream stream)
        {
            var result = "";

            stream.Position = 0;
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            result = Convert.ToBase64String(data);

            return result;
        }

        public static string GetUrlWithHttpPrefix(string url)
        {
            if (url == null)
            {
                url = "";
            }

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }
            return url;
        }

        public static string GetUrlWithHttpOrHttpsRemoved(string url)
        {
            if (url == null)
            {
                url = "";
            }

            if (url.StartsWith("http://"))
            {
                url = url.Substring("http://".Length);
            }
            else if (url.StartsWith("https://"))
            {
                url = url.Substring("https://".Length);
            }
            return url;
        }

        public static Encoding GetEncoding(string fileName)
        {
            return GetEncoding(fileName, Encoding.Default);
        }

        public static Encoding GetEncoding(FileStream stream)
        {
            return GetEncoding(stream, Encoding.Default);
        }

        public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
        {
            var targetEncoding = defaultEncoding;
            try
            {
                using (var fs = File.OpenRead(fileName))
                {
                    targetEncoding = GetEncoding(fs, defaultEncoding);
                };
            }
            catch (Exception)
            {

            }
            return targetEncoding;
        }

        public static Encoding GetEncoding(FileStream stream, Encoding defaultEncoding)
        {
            Encoding targetEncoding = defaultEncoding;
            if (stream != null && stream.Length >= 2)
            {
                //保存文件流的前4个字节   
                byte byte1 = 0;
                byte byte2 = 0;
                byte byte3 = 0;
                byte byte4 = 0;
                //保存当前Seek位置   
                long origPos = stream.Seek(0, SeekOrigin.Begin);
                stream.Seek(0, SeekOrigin.Begin);

                int nByte = stream.ReadByte();
                byte1 = Convert.ToByte(nByte);
                byte2 = Convert.ToByte(stream.ReadByte());
                if (stream.Length >= 3)
                {
                    byte3 = Convert.ToByte(stream.ReadByte());
                }
                if (stream.Length >= 4)
                {
                    byte4 = Convert.ToByte(stream.ReadByte());
                }
                //根据文件流的前4个字节判断Encoding   
                //Unicode {0xFF, 0xFE};   
                //BE-Unicode {0xFE, 0xFF};   
                //UTF8 = {0xEF, 0xBB, 0xBF};   
                if (byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe   
                {
                    targetEncoding = Encoding.BigEndianUnicode;
                }
                if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode   
                {
                    targetEncoding = Encoding.Unicode;
                }
                if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)//UTF8   
                {
                    targetEncoding = Encoding.UTF8;
                }
                //恢复Seek位置         
                stream.Seek(origPos, SeekOrigin.Begin);
            }
            return targetEncoding;
        }

        // 新增加一个方法，解决了不带BOM的 UTF8 编码问题   

        /// <summary>   
        /// 通过给定的文件流，判断文件的编码类型   
        /// </summary>   
        /// <param name="fs">文件流</param>   
        /// <returns>文件的编码类型</returns>   
        public static Encoding GetEncoding(Stream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM   
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            byte[] ss = r.ReadBytes(4);
            if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            else
            {
                if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                {
                    reVal = Encoding.UTF8;
                }
                else
                {
                    int.TryParse(fs.Length.ToString(), out int i);
                    ss = r.ReadBytes(i);

                    if (IsUTF8Bytes(ss))
                        reVal = Encoding.UTF8;
                }
            }
            r.Close();
            return reVal;

        }

        /// <summary>   
        /// 判断是否是不带 BOM 的 UTF8 格式   
        /// </summary>   
        /// <param name="data"></param>   
        /// <returns></returns>   
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;  //计算当前正分析的字符应还有的字节数   
            byte curByte; //当前分析的字节.   
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前   
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　   
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1   
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式!");
            }
            return true;
        }

        public static void StartBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception)
            {
                try
                {
                    Process.Start(@"IExplore.exe", url);
                }
                catch (Exception ex)
                {
                    var msg = Logger.GetExceptionDetails(ex);
                    Logger.Log(msg);
                    Report.ReportExceptionAsync(msg);
                }
            }
        }

        public static string DecryptString(string source)
        {
            return Encoding.UTF8.GetString(Common.Util.AESDecrypt(Convert.FromBase64String(source)));
        }

        public static MemoryStream AddWatermark(Stream imageSource, string watermark, Color color, Font font)
        {
            using (var bitmap = new Bitmap(imageSource))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    var brush = new SolidBrush(color);
                    var r = graphics.MeasureString(watermark, font);

                    var width = bitmap.Width;
                    var height = bitmap.Height;

                    graphics.DrawString(watermark, font, brush, (width - r.Width) / 2, height - r.Height - 10);
                }

                var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Jpeg);
                return stream;
            }
        }

    }
}


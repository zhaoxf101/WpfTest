using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfTest
{
    class ImageUtil
    {
        public static Stream Test()
        {
            var text = "测试" + Environment.NewLine + "当前时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            var textColor = Color.FromArgb(124, 12, 236, 124);
            var textFont = new Font("宋体", 19);

            var path = @"D:\test\test.jpg";
            
            var stream = Application.GetResourceStream(new Uri("pack://application:,,,/WpfTest2;component/images/watermark_background.png", UriKind.RelativeOrAbsolute)).Stream;

            var output = AddWatermark(stream, text, textColor, textFont);

            var fileStream = File.Create(path);

            output.WriteTo(fileStream);

            fileStream.Close();
            

            return null;
        }


        public static MemoryStream AddWatermark(Stream imageSource, string watermark, Color color, Font font)
        {
            var bitmap = new Bitmap(imageSource);

            var graphics = Graphics.FromImage(bitmap);
            var brush = new SolidBrush(color);


            var r = graphics.MeasureString(watermark, font);

            var width = bitmap.Width;
            var height = bitmap.Height;

            graphics.DrawString(watermark, font, brush, (width - r.Width) / 2, height - r.Height - 10);

            graphics.Dispose();

            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);
            bitmap.Dispose();

            return stream;
        }


        /// <returns></returns>
        private bool AddWatermarkAndSave(string path, string fileName, string text, Image img,
                    int paddingTop, int paddingLeft, Color textColor, Font textFont)
        {
            text = text + ";" + "当前时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            textFont = new Font("宋体", 19);

            Bitmap bm = new Bitmap(img);
            var g = Graphics.FromImage(bm);
            var b = new SolidBrush(textColor);

            //string[] str = text.Split(';');
            //for (int i = 0; i < str.Length; i++)
            //    g.DrawString(str[i], textFont, b, paddingLeft, paddingTop + 33 * i);


            g.DrawString(text, textFont, b, paddingLeft, paddingTop);

            g.Dispose();

            bm.Save(path + fileName, ImageFormat.Jpeg);
            bm.Dispose();
            return true;
        }
    }
}

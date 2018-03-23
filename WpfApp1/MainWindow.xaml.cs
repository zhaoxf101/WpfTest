using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const string Key = @"IMSNB]6,YF}+efcaj{+oESb9d8>Z'e9M";
        const string IV = @"IMS~f4,Ir)b$=pkf";


        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private BitmapSource BitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr ptr = bitmap.GetHbitmap();
            BitmapSource result =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //release resource  
            DeleteObject(ptr);

            return result;
        }

        public MainWindow()
        {
            InitializeComponent();



            //ImageTest.Source = BitmapToBitmapSource(WpfApp1.Properties.Resources.QQ图片20180116104852);

        }

        private void Txt2_GotFocus(object sender, RoutedEventArgs e)
        {
            Txt2.SelectAll();
        }

        private void Txt1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Txt2.Text = "";
            if (!string.IsNullOrEmpty(Txt1.Text))
            {
                var textArray = Txt1.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in textArray)
                {
                    if (!string.IsNullOrEmpty(Txt2.Text))
                    {
                        Txt2.Text += Environment.NewLine;
                    }
                    Txt2.Text += EncryptString(item);
                }
            }
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


        public static string DecryptString(string source)
        {
            return Encoding.UTF8.GetString(AESDecrypt(Convert.FromBase64String(source)));
        }

        public static string EncryptString(string source)
        {
            return Convert.ToBase64String(AESEncrypt(Encoding.UTF8.GetBytes(source)));
        }

        private void Txt2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Txt2.Focus();
            e.Handled = true;
        }

        private void ButtonDeviceId_Click(object sender, RoutedEventArgs e)
        {
            var deviceId = Txt1.Text;
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                if (!deviceId.StartsWith("base64"))
                {
                    MessageBox.Show("无效的设备ID！");
                    return;
                }
                var result = Encoding.UTF8.GetString(Convert.FromBase64String(deviceId.Substring("base64".Length)));
                Clipboard.SetText(result);
            }
        }

        private void ButtonDeviceIdMd5_Click(object sender, RoutedEventArgs e)
        {
            var deviceId = Txt1.Text;
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                var result = "base64" + Convert.ToBase64String(Encoding.UTF8.GetBytes(deviceId));
                Clipboard.SetText(result);
            }
        }
    }
}

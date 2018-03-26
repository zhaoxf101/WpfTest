using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeviceFilter
{
    public class FilterRecord
    {
        public string MachineCode { get; set; }

        public string UserName { get; set; }

        public string Company { get; set; }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnMd5MachineCode_Click(object sender, RoutedEventArgs e)
        {
            var lines = TxtMachineCode.Text?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim());
            var builder = new StringBuilder();
            foreach (var item in lines)
            {
                var result = item;
                if (!item.StartsWith("base64"))
                {
                    result = "base64" + Convert.ToBase64String(Encoding.UTF8.GetBytes(item));
                }

                builder.AppendLine(result);
            }

            TxtMachineCode.Text = builder.ToString();
        }

        private void BtnResolveMachineCode_Click(object sender, RoutedEventArgs e)
        {
            var lines = TxtMachineCode.Text?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim());
            var builder = new StringBuilder();
            foreach (var item in lines)
            {
                var result = item;
                if (item.StartsWith("base64"))
                {
                    result = Encoding.UTF8.GetString(Convert.FromBase64String(item.Substring("base64".Length)));
                }
                
                builder.AppendLine(result);
            }

            TxtMachineCode.Text = builder.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var list = new List<FilterRecord>();
            var lines = TxtMachineCode.Text?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()).ToList();
            var length = 0;

            foreach (var item in lines)
            {
                var code = item;
                if (!code.StartsWith("base64"))
                {
                    code = "base64" + Convert.ToBase64String(Encoding.UTF8.GetBytes(code));
                }
                list.Add(new FilterRecord
                {
                    MachineCode = code
                });
            }

            lines = TxtUserName.Text?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()).ToList();
            length = Math.Min(lines.Count, list.Count);
            for (int i = 0; i < length; i++)
            {
                list[i].UserName = lines[i];
            }

            lines = TxtCompany.Text?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()).ToList();
            length = Math.Min(lines.Count, list.Count);
            for (int i = 0; i < length; i++)
            {
                list[i].Company = lines[i];
            }

            var builder = new StringBuilder();
            foreach (var item in list)
            {
                builder.AppendLine(Post(item)).AppendLine();
            }

            MessageBox.Show(builder.ToString());
        }

        string Post(FilterRecord data)
        {
            string json = string.Empty;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://ims-api.xyunhui.com/api/common/adddevicetofilter");
            Encoding encoding = Encoding.UTF8;

            req.Headers.Add("XinYunHui-DeviceId", "zhanghs");
            req.Headers.Add("XinYunHui-DeviceType", "win7");
            req.Headers.Add("XinYunHui-Version", "2.0.0.0");

            var param = $"DeviceId={data.MachineCode}&UserName={data.UserName}&Company={data.Company}";
            byte[] bs = Encoding.UTF8.GetBytes(param);
            req.Method = "Post";

            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            req.Timeout = 10 * 60 * 1000;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
            }
            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    json = reader.ReadToEnd();
                }
            }

            return json;
        }
    }
}

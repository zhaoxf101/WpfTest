using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfTest
{
    /// <summary>
    /// TextBoxTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TextBoxTestWindow : Window
    {
        public TextBoxTestWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var t = TxtContent.HtmlText;

            //TxtContent.HtmlText = @"<P><IMG SRC=""file:///D:/MyFiles/History/云合景从项目/切图/网站建设/产品维护（添加）.png"" TITLE="""" ALT="""" /></P>";

        }
    }
}

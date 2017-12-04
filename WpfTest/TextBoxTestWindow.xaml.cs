using System;
using System.Collections.Generic;
using System.IO.Packaging;
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
            // pack://payload:,,wpf1,/Xaml/Document.xaml" UriSource="./Image1.bmp


            // <FlowDocument PagePadding="5,0,5,0" AllowDrop="True" NumberSubstitution.CultureSource="User" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"><BlockUIContainer TextAlignment="Justify"><Image Width="400" Height="134"><Image.Source><BitmapImage BaseUri="pack://payload:,,wpf1,/Xaml/Document.xaml" UriSource="./Image1.bmp" CacheOption="OnLoad" /></Image.Source></Image></BlockUIContainer></FlowDocument>

            //TxtContent.HtmlText = @"<P><IMG SRC=""file:///D:/MyFiles/History/云合景从项目/切图/网站建设/产品维护（添加）.png"" TITLE="""" ALT="""" /></P>";

        }
    }
}

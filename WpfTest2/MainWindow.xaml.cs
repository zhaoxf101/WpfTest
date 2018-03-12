using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WpfTest2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            List<BitmapImage> ls_adv_img = new List<BitmapImage>()
            {
                
            };


            //"/Main;component/images/menu/模板网站.png";
            // {pack://application:,,,/Main;component/images/menu/模板网站.png}
            //rollImg.AddImage("pack://application:,,,/WpfTest2;component/images/5.png");
            //rollImg.AddImage("pack://application:,,,/WpfTest2;component/images/6.png");

        }

        List<dynamic> Test()
        {
            return new List<dynamic>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

        }
    }
}

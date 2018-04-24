using Newtonsoft.Json;
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
using WpfTest;

namespace WpfTest2
{

    public class RedisRefreshRankingModel
    {
        public DateTime LastCompletedTime { get; set; }

        public int RefreshCount { get; set; }
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();

            DataContext = _viewModel = new ViewModel();

            for (int i = 0; i < 10; i++)
            {
                _viewModel.Regions.Add(new TitleItem
                {
                    Name = "hello" + i,
                    IsChecked = i % 2 == 0 ? (bool?)true : i % 3 == 0 ? null : (bool?)false
                });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var str1 = JsonConvert.SerializeObject("Hell0");

            var str = "ba";

            var value = JsonConvert.DeserializeObject<RedisRefreshRankingModel>(str);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is TitleItem dataItem)
            {
                _viewModel.Regions.Remove(dataItem);

            }
        }
    }
}

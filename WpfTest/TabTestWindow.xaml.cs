using MarketingPlatform.Client;
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
    /// TabTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TabTestWindow : Window
    {
        public TabTestWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Tab.SelectedIndex = -1;

            Logger.Log($"{Tab.SelectedTabIndex}");
        }

        private void Tab_TabItemSelected(object sender, TabItemSelectedEventArgs args)
        {
            Logger.Log($"{args.TabItem}");
        }

    }
}

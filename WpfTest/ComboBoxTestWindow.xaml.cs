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
    public class PurchaseServiceType
    {
        public int Limit { get; set; }

        public string Option { get; set; }

        public decimal Price { get; set; }
    }


    /// <summary>
    /// ComboBoxTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ComboBoxTestWindow : Window
    {
        public ComboBoxTestWindow()
        {
            InitializeComponent();

            var list = new List<PurchaseServiceType>
            {
                new PurchaseServiceType
                {
                    Limit = 12,
                    Option = "ads1236545655fad",
                    Price = 34
                },
                new PurchaseServiceType
                {
                    Limit = 12,
                    Option = "adsfad",
                    Price = 34
                }

            };

            ComboBoxServiceLimit.ItemsSource = list;
        }

        private void ComboBoxServiceLimit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Grade { get; set; }

        public string Professional { get; set; }

        public int Score { get; set; }

        public bool HasJob { get; set; }

    }
    /// <summary>
    /// ListViewTest.xaml 的交互逻辑
    /// </summary>
    public partial class ListViewTest : Window
    {
        public ListViewTest()
        {
            InitializeComponent();
        }

        private void CheckBocHasJob_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void textboxName_GotFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}

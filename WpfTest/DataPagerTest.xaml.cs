using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class DataPagerViewModel : ViewModelBase
    {
        private int _pageIndex = 1;

        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; OnPropertyChanged("PageIndex"); }
        }

        private int _pageSize = 10;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; OnPropertyChanged("PageSize"); }
        }

        private int _totalCount = 51;

        public int TotalCount
        {
            get { return _totalCount; }
            set { _totalCount = value; OnPropertyChanged("TotalCount"); }
        }

        private int _selectedIndex = -10;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; OnPropertyChanged("SelectedIndex"); }
        }

        private int _count;

        public int Count
        {
            get { return _count; }
            set { _count = value; OnPropertyChanged("Count"); }
        }


    }

    /// <summary>
    /// DataPagerTest.xaml 的交互逻辑
    /// </summary>
    public partial class DataPagerTest : Window
    {
        DataPagerViewModel _viewModel;

        public DataPagerTest()
        {
            InitializeComponent();

            DataContext = _viewModel = new DataPagerViewModel();
        }

        private void DataPager_PageChanged(object sender, PageChangedEventArgs e)
        {
            Debug.WriteLine("PageChanged. PageIndex: {0} PageSize: {1} PageCount: {2} TotalCount: {3}", e.PageIndex, e.PageSize, e.PageCount, e.TotalCount);
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {

            _viewModel.PageIndex = 0;

            _viewModel.PageSize++;
            //if (Test.SeparatorBrush == Brushes.Red)
            //{
            //    Test.SeparatorBrush = Brushes.Black;
            //}
            //else
            //{
            //    Test.SeparatorBrush = Brushes.Red;
            //}
        }
    }
}

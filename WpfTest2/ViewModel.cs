using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTest2
{
    public class TitleItem : ViewModelBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        private bool? _isChecked;

        public bool? IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; OnPropertyChanged("IsChecked"); }
        }

    }

    class ViewModel
    {
        ObservableCollection<TitleItem> _regions = new ObservableCollection<TitleItem>();
        
        public ObservableCollection<TitleItem> Regions { get => _regions; }
    }
}

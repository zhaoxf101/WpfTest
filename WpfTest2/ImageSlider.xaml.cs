using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfTest2
{
    public partial class ImageSlider : UserControl
    {
        static Brush _IndicatorBorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2c5387"));
        static Brush _IndicatorBackgroundBrushSelected = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2272c4"));

        ObservableCollection<string> _imageUrlList = new ObservableCollection<string>();

        int _index;
        int _imageIndex;

        DispatcherTimer _timer = new DispatcherTimer();

        AnimationHelper _animationHelper = new AnimationHelper();

        public ImageSlider()
        {
            InitializeComponent();

            _timer.Tick += _timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(6);

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _timer.Start();
            }

            _imageUrlList.CollectionChanged += _imageUrlList_CollectionChanged;
        }

        private void _imageUrlList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Reset();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            Update(-1);
        }

        void Update(int next)
        {
            _animationHelper.RemoveAll();

            ImageWrapper.Children[_imageIndex].Opacity = 1;
            _animationHelper.BeginAnimation(ImageWrapper.Children[_imageIndex], Image.OpacityProperty, 0, 0, 0.3);

            var newImageIndex = 0;
            var newIndex = 0;
            if (next > -1)
            {
                newImageIndex = (_imageIndex + 1) % ImageWrapper.Children.Count;
                newIndex = next;               
            }
            else
            {
                // 设定下一张图片
                newImageIndex = (_imageIndex + 1) % ImageWrapper.Children.Count;
                newIndex = (_index + 1) % _imageUrlList.Count;
            }

            var image = (Image)ImageWrapper.Children[newImageIndex];
            var imageUrl = _imageUrlList[newIndex];

            var bitmapImage = new BitmapImage(new Uri(imageUrl, UriKind.RelativeOrAbsolute));
            image.Width = bitmapImage.PixelWidth;
            image.Height = bitmapImage.PixelHeight;

            image.Source = bitmapImage;

            image.Opacity = 0;
            _animationHelper.BeginAnimation(ImageWrapper.Children[newImageIndex], Image.OpacityProperty, 1, 0.3, 0);

            var border = (Border)IndicatorWrapper.Children[_index];
            border.Background = Brushes.Transparent;

            border = (Border)IndicatorWrapper.Children[newIndex];
            border.Background = _IndicatorBackgroundBrushSelected;

            _index = newIndex;
            _imageIndex = newImageIndex;
        }

        public void AddImage(string url)
        {
            _imageUrlList.Add(url);
            Reset();
        }

        public void Clear()
        {

        }

        public ObservableCollection<string> Images { get => _imageUrlList; }

        void Reset()
        {
            var count = _imageUrlList.Count;

            var subCount = IndicatorWrapper.Children.Count - count;
            for (int i = 0; i < subCount; i++)
            {
                IndicatorWrapper.Children.RemoveAt(IndicatorWrapper.Children.Count - 1);
            }

            var addCount = count - IndicatorWrapper.Children.Count;
            for (int i = 0; i < addCount; i++)
            {
                var border = new Border
                {
                    Width = 40,
                    Height = 10,
                    BorderThickness = new Thickness(1),
                    BorderBrush = _IndicatorBorderBrush,
                    CornerRadius = new CornerRadius(4),
                    Background = Brushes.Transparent,
                    Margin = new Thickness(5, 0, 5, 0),
                    Cursor = Cursors.Hand
                };
                border.MouseEnter += Border_MouseEnter;
                border.MouseLeave += Border_MouseLeave;

                IndicatorWrapper.Children.Add(border);
            }
            _index = 0;
            _imageIndex = 0;

            if (count > 0)
            {
                var indicator = (Border)IndicatorWrapper.Children[_index];
                var image = (Image)ImageWrapper.Children[_imageIndex];
                var imageUrl = _imageUrlList[_index];

                var bitmapImage = new BitmapImage(new Uri(imageUrl, UriKind.RelativeOrAbsolute));
                image.Width = bitmapImage.PixelWidth;
                image.Height = bitmapImage.PixelHeight;

                image.Source = bitmapImage;

                indicator.Background = _IndicatorBackgroundBrushSelected;
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            _timer.Start();
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = (Border)sender;
            var index = IndicatorWrapper.Children.IndexOf(border);

            Update(index);
            _timer.Stop();
        }

    }
}

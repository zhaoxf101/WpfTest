using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfTest2
{
    public class TilePanel : Panel
    {
        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(int), typeof(TilePanel), new PropertyMetadata(3, OnColumnCountPropertyChanged));

        private static void OnColumnCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (TilePanel)d;
            //panel.InvalidateMeasure();
            panel.InvalidateVisual();
        }



        public GridLength RowHeight
        {
            get { return (GridLength)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(GridLength), typeof(TilePanel), new PropertyMetadata(GridLength.Auto, OnRowHeightPropertyChanged));

        private static void OnRowHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (TilePanel)d;
            //panel.InvalidateMeasure();
            panel.InvalidateVisual();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count < 1)
                return new Size(0, 0);

            var index = 0;
            var maxWidth = 0.0;
            var maxHeight = 0.0;
            foreach (UIElement item in Children)
            {
                if (item.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                maxWidth = Math.Max(maxWidth, item.DesiredSize.Width);
                maxHeight = Math.Max(maxHeight, item.DesiredSize.Height);
                index++;
            }

            if (index == 0)
            {
                return new Size(maxWidth * ColumnCount, 0);
            }

            if (RowHeight.IsAbsolute)
            {
                return new Size(maxWidth * ColumnCount, RowHeight.Value * ((index - 1) / ColumnCount + 1));
            }
            else
            {
                return new Size(maxWidth * ColumnCount, maxHeight * ((index - 1) / ColumnCount + 1));
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count < 1)
                return finalSize;

            var widthUnit = finalSize.Width / ColumnCount;
            var heightUnit = 0d;

            if (RowHeight.IsAbsolute)
            {
                heightUnit = RowHeight.Value;
            }
            else
            {
                var count = Children.Cast<UIElement>().Count(p => p.Visibility != Visibility.Collapsed);
                if (count == 0)
                {
                    return finalSize;
                }

                heightUnit = finalSize.Height / ((count - 1) / ColumnCount + 1);
            }

            var index = 0;
            foreach (UIElement item in Children)
            {
                if (item.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                var row = index / ColumnCount;
                var column = index % ColumnCount;

                var origin = new Point(column * widthUnit + (widthUnit - item.DesiredSize.Width) / 2,
                    row * heightUnit + (heightUnit - item.DesiredSize.Height) / 2);

                item.Arrange(new Rect(origin, item.DesiredSize));

                index++;
            }

            return finalSize;
        }
    }
}

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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTest
{
    public class DataGridRowHeader : Control
    {
        public int Count2
        {
            get
            {
                return (int)GetValue(Count2Property);
            }
        }

        public static readonly DependencyProperty Count2Property;

        private static readonly DependencyPropertyKey Count2PropertyKey =
            DependencyProperty.RegisterReadOnly("Count2", typeof(int), typeof(DataGridRowHeader), new FrameworkPropertyMetadata(0, null, new CoerceValueCallback(OnCoerceCount2)));

        private static object OnCoerceCount2(DependencyObject d, object baseValue)
        {
            var header = ((DataGridRowHeader)d);

            return header.Count2 + 1;
        }


        public static readonly DependencyProperty SeparatorBrushProperty;

        public Brush SeparatorBrush
        {
            get
            {
                return (Brush)base.GetValue(DataGridRowHeader.SeparatorBrushProperty);
            }
            set
            {
                base.SetValue(DataGridRowHeader.SeparatorBrushProperty, value);
            }
        }



        public Style TestStyle
        {
            get { return (Style)GetValue(TestStyleProperty); }
            set { SetValue(TestStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TestStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TestStyleProperty =
            DependencyProperty.Register("TestStyle", typeof(Style), typeof(DataGridRowHeader), new PropertyMetadata(null));



        static DataGridRowHeader()
        {
            DataGridRowHeader.SeparatorBrushProperty = DependencyProperty.Register("SeparatorBrush", typeof(Brush), typeof(DataGridRowHeader), new FrameworkPropertyMetadata(null, OnSeparatorBrushChanged));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridRowHeader), new FrameworkPropertyMetadata(typeof(DataGridRowHeader)));

            Count2Property = Count2PropertyKey.DependencyProperty;

        }

        private static void OnSeparatorBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var header = (DataGridRowHeader)d;
            d.CoerceValue(DataGridRowHeader.Count2Property);
        }

        public override void EndInit()
        {
            base.EndInit();
        }
    }
}

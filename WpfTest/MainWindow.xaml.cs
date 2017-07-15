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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var value = 90.0;

            var angle = Math.PI / 180 * value;
            var x = Radius * (1 + Math.Sin(angle));
            var y = Radius * (1 - Math.Cos(angle));
            var isLargeArc = value > 180 ? true : false;

            CreateArcSegment(x, y, isLargeArc);

        }

        private void CreateArcSegment(double x, double y, bool isLargeArc)
        {
            PathFigure pthFigure = new PathFigure
            {
                StartPoint = new Point(50, 0),
                IsClosed = false
            };

            ArcSegment arcSeg = new ArcSegment
            {
                Point = new Point(x, y),
                Size = new Size(50, 50),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = isLargeArc
            };
            /*RotationAngle = 30*/
            ;

            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            myPathSegmentCollection.Add(arcSeg);

            pthFigure.Segments = myPathSegmentCollection;

            PathFigureCollection pthFigureCollection = new PathFigureCollection();
            pthFigureCollection.Add(pthFigure);

            PathGeometry pthGeometry = new PathGeometry();
            pthGeometry.Figures = pthFigureCollection;


            //Path arcPath = new Path();
            //arcPath.Stroke = new SolidColorBrush(Colors.Black);
            //arcPath.StrokeThickness = 1;
            //arcPath.Data = pthGeometry;
            //arcPath.Fill = new SolidColorBrush(Colors.Yellow);

            PathKeyword.Data = pthGeometry;

            //LayoutRoot.Children.Add(arcPath);
        }

        const double Radius = 50;
        const float Precision = 0.000001f;

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var value = e.NewValue;
            if (Math.Abs(value - 360) <= Precision)
            {
                value = 359.9;
            }
            var angle = Math.PI / 180 * value;
            var x = Radius * (1 + Math.Sin(angle));
            var y = Radius * (1 - Math.Cos(angle));
            var isLargeArc = value > 180 ? true : false;
            CreateArcSegment(x, y, isLargeArc);
            Debug.WriteLine("angle: {2} x: {0} y: {1}", x, y, value);
        }
    }
}

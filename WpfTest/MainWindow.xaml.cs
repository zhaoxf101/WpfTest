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
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var button = new Button();

            CreateArcSegment();
        }

        private void CreateArcSegment()
        {
            PathFigure pthFigure = new PathFigure
            {
                StartPoint = new Point(50, 0),
                IsClosed = false
            };

            ArcSegment arcSeg = new ArcSegment
            {
                Point = new Point(100, 50),
                Size = new Size(50, 50),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = false
            };
            /*RotationAngle = 30*/;

            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            myPathSegmentCollection.Add(arcSeg);

            pthFigure.Segments = myPathSegmentCollection;

            PathFigureCollection pthFigureCollection = new PathFigureCollection();
            pthFigureCollection.Add(pthFigure);

            PathGeometry pthGeometry = new PathGeometry();
            pthGeometry.Figures = pthFigureCollection;


            Path arcPath = new Path();
            arcPath.Stroke = new SolidColorBrush(Colors.Black);
            arcPath.StrokeThickness = 1;
            arcPath.Data = pthGeometry;
            arcPath.Fill = new SolidColorBrush(Colors.Yellow);

            PathKeyword.Data = pthGeometry;

            //LayoutRoot.Children.Add(arcPath);
        }
    }
}

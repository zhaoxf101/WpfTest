using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfTest
{
    internal static class DrawingConstants
    {
        public static readonly int Rows = 10;
        public static readonly int Columms = 10;
        public static readonly double PenThickness = 1.0;
        public static readonly double HalfOfPenThickness = PenThickness / 2;
    }

    class GuidelineSetDrawingElement : FrameworkElement
    {
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            //Pen pen = new Pen(Brushes.Red, 1);
            //pen.DashStyle = new DashStyle(new List<double> { 5, 2 }, 0);
            //drawingContext.DrawLine(pen, new System.Windows.Point(60, 75), new System.Windows.Point(1600, 75));

            double xOffset = Math.Floor(this.RenderSize.Width / DrawingConstants.Columms);
            double yOffset = Math.Floor(this.RenderSize.Height / DrawingConstants.Rows);
            double xLineWidth = Math.Floor(this.RenderSize.Width);
            double yLineHeight = Math.Floor(this.RenderSize.Height);

            DrawingContext dct = drawingContext;
            Pen blackPen = new Pen(Brushes.Black, DrawingConstants.PenThickness);
            blackPen.DashStyle = new DashStyle(new List<double> { 5, 2 }, 0);
            blackPen.Freeze();

            //Draw the horizontal lines  
            Point x = new Point(0, 0);
            Point y = new Point(xLineWidth, 0);
            for (int i = 0; i <= DrawingConstants.Rows; i++)
            {
                dct.PushGuidelineSet(new GuidelineSet(null, new double[] { y.Y - DrawingConstants.HalfOfPenThickness, y.Y + DrawingConstants.HalfOfPenThickness }));
                dct.DrawLine(blackPen, x, y);
                dct.Pop();
                x.Offset(0, yOffset);
                y.Offset(0, yOffset);
            }

            //Draw the vertical lines  
            x = new Point(0, 0);
            y = new Point(0, yLineHeight);
            for (int i = 0; i <= DrawingConstants.Columms; i++)
            {
                dct.PushGuidelineSet(new GuidelineSet(new double[] { x.X + DrawingConstants.HalfOfPenThickness, x.X - DrawingConstants.HalfOfPenThickness }, null));
                dct.DrawLine(blackPen, x, y);
                dct.Pop();
                x.Offset(xOffset, 0);
                y.Offset(xOffset, 0);
            }
        }
    }
}

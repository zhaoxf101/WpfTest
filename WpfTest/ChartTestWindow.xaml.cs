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
using Visifire.Charts;

namespace WpfTest
{
    /// <summary>
    /// ChartTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChartTestWindow : Window
    {

        private List<string> strListx = new List<string>() { "苹果", "樱桃", "菠萝", "香蕉", "榴莲", "葡萄", "桃子", "猕猴桃" };
        private List<string> strListy = new List<string>() { "13", "75", "60", "38", "97", "22", "39", "80" };

        private List<DateTime> LsTime = new List<DateTime>()
            {
               new DateTime(2012,1,1),
               new DateTime(2012,2,1),
               new DateTime(2012,3,1),
               new DateTime(2012,4,1),
               new DateTime(2012,5,1),
               new DateTime(2012,6,1),
               new DateTime(2012,7,1),
               new DateTime(2012,8,1),
               new DateTime(2012,9,1),
               new DateTime(2012,10,1),
               new DateTime(2012,11,1),
               new DateTime(2012,12,1),
            };
        private List<string> cherry = new List<string>() { "33", "75", "60", "98", "67", "88", "39", "45", "13", "22", "45", "80" };
        private List<string> pineapple = new List<string>() { "13", "34", "38", "12", "45", "76", "36", "80", "97", "22", "76", "39" };

        public ChartTestWindow()
        {
            InitializeComponent();

            var xaxis = new Axis();
            var xal = new AxisLabels
            {
                Enabled = true
            };
            xaxis.AxisLabels = xal;
            Axis yaxis = new Axis();
            var yal = new AxisLabels
            {
                Enabled = true
            };
            yaxis.AxisLabels = yal;
            Chart.AxesX.Add(xaxis);
            Chart.AxesY.Add(yaxis);

            Chart.Series.Clear();
            Chart.Titles.Clear();

            var dataSeries = new DataSeries
            {
                RenderAs = RenderAs.Column
            };
            dataSeries.DataPoints.Add(new DataPoint { AxisXLabel = "baid", YValue = 30 });
            dataSeries.DataPoints.Add(new DataPoint { AxisXLabel = "baid2", YValue = 32 });
            dataSeries.DataPoints.Add(new DataPoint { AxisXLabel = "baid3", YValue = 33 });
            dataSeries.DataPoints.Add(new DataPoint { AxisXLabel = "baid4", YValue = 300 });

            Chart.Series.Add(dataSeries);


            //设置图标的宽度和高度
            //Chart.Width = 580;
            //Chart.Height = 380;
            //Chart.Margin = new Thickness(100, 5, 10, 5);
            ////是否启用打印和保持图片
            //Chart.ToolBarEnabled = false;


            //Chart.Theme = "Theme2";

            ////设置图标的属性
            //Chart.ScrollingEnabled = false;//是否启用或禁用滚动
            //Chart.View3D = false;//3D效果显示

            ////创建一个标题的对象
            //Title title = new Title();

            ////设置标题的名称
            //title.Text = Name;
            //title.Padding = new Thickness(0, 10, 5, 0);

            ////向图标添加标题
            //Chart.Titles.Add(title);

            //Axis yAxis = new Axis();
            ////设置图标中Y轴的最小值永远为0           
            //yAxis.AxisMinimum = 0;
            ////设置图表中Y轴的后缀          
            //yAxis.Suffix = "斤";
            //Chart.AxesY.Add(yAxis);

            //// 创建一个新的数据线。               
            //DataSeries dataSeries = new DataSeries();

            //// 设置数据线的格式
            //dataSeries.RenderAs = RenderAs.StackedColumn;//柱状Stacked

            //var valuex = strListx;
            //var valuey = strListy;

            //// 设置数据点              
            //DataPoint dataPoint;
            //for (int i = 0; i < valuex.Count; i++)
            //{
            //    // 创建一个数据点的实例。                   
            //    dataPoint = new DataPoint();
            //    // 设置X轴点                    
            //    dataPoint.AxisXLabel = valuex[i];
            //    //设置Y轴点                   
            //    dataPoint.YValue = double.Parse(valuey[i]);
            //    //添加一个点击事件        
            //    //dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
            //    //添加数据点                   
            //    dataSeries.DataPoints.Add(dataPoint);
            //}

            //// 添加数据线到数据序列。                
            //Chart.Series.Add(dataSeries);

            //dataSeries = new DataSeries();

            //// 设置数据线的格式
            //dataSeries.RenderAs = RenderAs.StackedColumn;//柱状Stacked


            //for (int i = 0; i < valuex.Count; i++)
            //{
            //    // 创建一个数据点的实例。                   
            //    dataPoint = new DataPoint();
            //    // 设置X轴点                    
            //    dataPoint.AxisXLabel = valuex[i];
            //    //设置Y轴点                   
            //    dataPoint.YValue = double.Parse(valuey[i]);
            //    //添加一个点击事件        
            //    //dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
            //    //添加数据点                   
            //    dataSeries.DataPoints.Add(dataPoint);
            //}

            //// 添加数据线到数据序列。                
            //Chart.Series.Add(dataSeries);
        }

        private void dataPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}

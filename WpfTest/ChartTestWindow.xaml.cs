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
    class PageData
    {
        public int PageIndex { get; set; }

        public int Baidu { get; set; }

        public int Qihu { get; set; }

        public int Sougou { get; set; }
    }
    /// <summary>
    /// ChartTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChartTestWindow : Window
    {
        public ChartTestWindow()
        {
            InitializeComponent();

            Chart.Series.Clear();
            Chart.Titles.Clear();

            var dataSeries = new DataSeries[] {
                new DataSeries
                {
                    RenderAs = RenderAs.Column,
                    LabelEnabled = false,
                    LegendText = "百度"
                },

                new DataSeries
                {
                    RenderAs = RenderAs.Column,
                    LabelEnabled = false,
                    LegendText = "360"
                },
                new DataSeries
                {
                    RenderAs = RenderAs.Column,
                    LabelEnabled = false,
                    LegendText = "搜狗"
                }
            };

            var list = new List<PageData>
            {
                new PageData
                {
                    PageIndex = 1,
                    Baidu = 5,
                    Qihu = 3,
                    Sougou = 1
                },
                new PageData
                {
                    PageIndex = 2,
                    Baidu = 9,
                    Qihu = 3,
                    Sougou = 1
                },
                new PageData
                {
                    PageIndex = 3,
                     Baidu = 15,
                    Qihu = 33,
                    Sougou = 1
                }
            };


            for (int i = 0; i < list.Count; i++)
            {
                var datapoint = new DataPoint();
                datapoint.AxisXLabel = $"第{i + 1}页";
                datapoint.XValue = list[i].PageIndex;
                datapoint.YValue = list[i].Baidu;
                dataSeries[0].DataPoints.Add(datapoint);

                datapoint = new DataPoint();
                datapoint.AxisXLabel = $"第{i + 1}页";
                datapoint.XValue = list[i].PageIndex;
                datapoint.YValue = list[i].Qihu;
                dataSeries[1].DataPoints.Add(datapoint);

                datapoint = new DataPoint();
                datapoint.AxisXLabel = $"第{i + 1}页";
                datapoint.XValue = list[i].PageIndex;
                datapoint.YValue = list[i].Sougou;
                dataSeries[2].DataPoints.Add(datapoint);
            }

            foreach (var item in dataSeries)
            {
                Chart.Series.Add(item);
            }

            var t = Chart.Legends;

        
            Chart.Legends.Clear();

            Chart.Legends.Add(new Legend
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                DockInsidePlotArea = false,
            });

            Chart.AnimationEnabled = false;
            Chart.Theme = "Theme2";
        }

        private void dataPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}

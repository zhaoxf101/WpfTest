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

    public class DataPager : Control
    {
        static DataPager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataPager), new FrameworkPropertyMetadata(typeof(DataPager)));
        }

        public delegate void PageChangingRoutedEventHandler(object sender, PageChangingEventArgs e);
        public delegate void PageChangedRoutedEventHandler(object sender, PageChangedEventArgs e);

        public event PageChangingRoutedEventHandler PageChanging;
        public event PageChangedRoutedEventHandler PageChanged;

        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof(int), typeof(DataPager), new UIPropertyMetadata(1, OnPageIndexChanged));

        private static void OnPageIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pageIndex = (int)e.NewValue;
            var dataPager = (DataPager)d;

            dataPager.OnPageChanging(pageIndex, dataPager.PageSize, dataPager.TotalCount);
        }

        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(DataPager), new UIPropertyMetadata(10, OnPageSizeChanged));

        private static void OnPageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pageSize = (int)e.NewValue;
            var dataPager = (DataPager)d;

            dataPager.OnPageChanging(dataPager.PageIndex, pageSize, dataPager.TotalCount);
        }

        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set { SetValue(PageCountProperty, value); }
        }

        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(DataPager), new UIPropertyMetadata(1));

        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set
            {
                SetValue(TotalCountProperty, value);
            }
        }

        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register("TotalCount", typeof(int), typeof(DataPager), new UIPropertyMetadata(0, OnTotalCountChanged));

        private static void OnTotalCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var totalCount = (int)e.NewValue;
            var dataPager = (DataPager)d;

            dataPager.OnPageChanging(dataPager.PageIndex, dataPager.PageCount, totalCount);
        }

        internal void OnPageChanging(int pageIndex, int pageSize, int totalCount)
        {
            var pageIndexChanged = false;
            var pageSizeChanged = false;
            var totalCountChanged = false;

            if (pageIndex != PageIndex)
            {
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }
                else if (pageIndex > this.PageCount)
                {
                    pageIndex = this.PageCount;
                }

                if (pageIndex != PageIndex)
                {
                    pageIndexChanged = true;
                }
            }
            else if (pageSize != PageSize)
            {
                pageSize = pageSize > 0 ? pageSize : 10;
                if (pageSize != PageSize)
                {
                    pageSizeChanged = true;

                    var pageCount = (TotalCount + pageSize - 1) / pageSize;
                    if (PageIndex > pageCount)
                    {
                        pageIndex = pageCount;
                        pageIndexChanged = true;
                    }
                }
            }
            else if (totalCount != TotalCount)
            {
                totalCount = totalCount >= 0 ? totalCount : 0;

                if (totalCount != TotalCount)
                {
                    totalCountChanged = true;

                    var pageCount = (totalCount + PageSize - 1) / PageSize;
                    if (PageIndex > pageCount)
                    {
                        pageIndex = pageCount;
                        pageIndexChanged = true;
                    }
                }
            }

            if (pageIndexChanged || pageSizeChanged || totalCountChanged)
            {
                var eventArgs = new PageChangingEventArgs
                {
                    OldPageIndex = PageIndex,
                    NewPageIndex = pageIndex,

                    OldPageCount = PageCount,
                    NewPageCount = (totalCount + pageSize - 1) / pageSize,

                    OldPageSize = PageSize,
                    NewPageSize = pageSize,

                    OldTotalCount = TotalCount,
                    NewTotalCount = totalCount
                };

                PageChanging?.Invoke(this, eventArgs);

                if (!eventArgs.IsCancel)
                {
                    SetCurrentValue(PageIndexProperty, pageIndex);
                    SetCurrentValue(PageSizeProperty, pageSize);
                    SetCurrentValue(TotalCountProperty, totalCount);
                    SetCurrentValue(PageCountProperty, (TotalCount + PageSize - 1) / PageSize);

                    PageChanged?.Invoke(this, new PageChangedEventArgs
                    {
                        PageIndex = pageIndex,
                        PageCount = PageCount,
                        PageSize = pageSize
                    });
                }
            }
        }
    }

    public class PageChangedEventArgs
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int PageCount { get; set; }

        public int TotalCount { get; set; }
    }

    public class PageChangingEventArgs
    {

        public int OldPageIndex { get; set; }

        public int NewPageIndex { get; set; }

        public int OldPageSize { get; set; }

        public int NewPageSize { get; set; }

        public int OldPageCount { get; set; }

        public int NewPageCount { get; set; }

        public int OldTotalCount { get; set; }

        public int NewTotalCount { get; set; }

        public bool IsCancel { get; internal set; }
    }
}

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
        const int PageButtonsCount = 2;
        const int DefaultPageSize = 10;

        static DataPager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataPager), new FrameworkPropertyMetadata(typeof(DataPager)));

            PageCountProperty = PageCountPropertyKey.DependencyProperty;
        }

        public delegate void PageChangedRoutedEventHandler(object sender, PageChangedEventArgs e);

        public event PageChangedRoutedEventHandler PageChanged;

        StackPanel _pageButtonWrapper;
        internal Button[] _firstLastButtons;
        internal Button[] _pageButtons;
        internal TextBlock _txtPageIndex;

        public Style FirstLastButtonStyle
        {
            get { return (Style)GetValue(FirstLastButtonStyleProperty); }
            set { SetValue(FirstLastButtonStyleProperty, value); }
        }
        public static readonly DependencyProperty FirstLastButtonStyleProperty = DependencyProperty.Register("FirstLastButtonStyle", typeof(Style), typeof(DataPager));

        public Style PageButtonStyle
        {
            get { return (Style)GetValue(PageButtonStyleProperty); }
            set { SetValue(PageButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty PageButtonStyleProperty = DependencyProperty.Register("PageButtonStyle", typeof(Style), typeof(DataPager));


        public Style PageLabelStyle
        {
            get { return (Style)GetValue(PageLabelStyleProperty); }
            set { SetValue(PageLabelStyleProperty, value); }
        }
        public static readonly DependencyProperty PageLabelStyleProperty = DependencyProperty.Register("PageLabelStyle", typeof(Style), typeof(DataPager));

        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register("PageIndex", typeof(int), typeof(DataPager), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPageIndexChanged, CoercePageIndex), ValidatePageIndex);

        private static object CoercePageIndex(DependencyObject d, object baseValue)
        {
            var dataPager = (DataPager)d;
            var pageIndex = (int)baseValue;
            if (pageIndex > dataPager.PageCount)
            {
                pageIndex = dataPager.PageCount;
            }

            return pageIndex;
        }

        private static bool ValidatePageIndex(object value)
        {
            var index = (int)value;
            return index >= 1;
        }

        private static void OnPageIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pageIndex = (int)e.NewValue;
            var dataPager = (DataPager)d;

            dataPager.OnPageChanged((int)e.OldValue, dataPager.PageSize, dataPager.TotalCount);
        }

        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }
        public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register("PageSize", typeof(int), typeof(DataPager), new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPageSizeChanged), ValidatePageSize);

        private static bool ValidatePageSize(object value)
        {
            return ((int)value) >= 1;
        }

        private static void OnPageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pageSize = (int)e.NewValue;
            var dataPager = (DataPager)d;

            dataPager.CoerceValue(PageCountProperty);
            dataPager.CoerceValue(PageIndexProperty);

            dataPager.OnPageChanged(dataPager.PageIndex, (int)e.OldValue, dataPager.TotalCount);
        }

        public int PageCount
        {
            get { return (int)GetValue(PageCountPropertyKey.DependencyProperty); }
        }

        public static readonly DependencyProperty PageCountProperty;
        private static readonly DependencyPropertyKey PageCountPropertyKey = DependencyProperty.RegisterReadOnly("PageCount", typeof(int), typeof(DataPager), new UIPropertyMetadata(1, null, CoercePageCount));

        private static object CoercePageCount(DependencyObject d, object baseValue)
        {
            var pager = (DataPager)d;

            var pageCount = (pager.TotalCount + pager.PageSize - 1) / pager.PageSize;
            if (pageCount == 0)
            {
                pageCount = 1;
            }
            return pageCount;
        }

        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set
            {
                SetValue(TotalCountProperty, value);
            }
        }

        public static readonly DependencyProperty TotalCountProperty = DependencyProperty.Register("TotalCount", typeof(int), typeof(DataPager), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTotalCountChanged), ValidateTotalCount);

        private static bool ValidateTotalCount(object value)
        {
            return (int)value >= 0;
        }

        private static void OnTotalCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var totalCount = (int)e.NewValue;
            var dataPager = (DataPager)d;

            dataPager.CoerceValue(PageCountProperty);

            if (dataPager.PageIndex > dataPager.PageCount)
            {
                dataPager.CoerceValue(DataPager.PageIndexProperty);
            }

            dataPager.OnPageChanged(dataPager.PageIndex, dataPager.PageCount, (int)e.OldValue);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _pageButtonWrapper = GetTemplateChild("PART_PageButtonWrapper") as StackPanel;

            if (_pageButtonWrapper != null)
            {
                if (_pageButtons == null)
                {
                    _pageButtons = new Button[PageButtonsCount];

                    for (int i = 0; i < _pageButtons.Length; i++)
                    {
                        var button = new Button
                        {
                            Content = i + i,
                            Style = PageButtonStyle
                        };
                        button.Click += PageButton_Click;
                        _pageButtons[i] = button;
                    }
                }

                if (_firstLastButtons == null)
                {
                    _firstLastButtons = new Button[2];
                    for (int i = 0; i < _firstLastButtons.Length; i++)
                    {
                        var button = new Button
                        {
                            Content = i == 0 ? "首页" : "尾页",
                            Style = FirstLastButtonStyle
                        };
                        button.Click += FirstLastButton_Click;
                        _firstLastButtons[i] = button;
                    }
                }

                if (_txtPageIndex == null)
                {
                    _txtPageIndex = new TextBlock
                    {
                        Text = "1",
                        Style = PageLabelStyle
                    };
                }

                ArrangePageButtons();
            }
        }

        private void FirstLastButton_Click(object sender, RoutedEventArgs e)
        {
            var index = Array.IndexOf(_firstLastButtons, sender);

            if (index == 0)
            {
                PageIndex = 1;
            }
            else
            {
                PageIndex = PageCount;
            }
        }

        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var pageIndex = Convert.ToInt32(button.Content);
            PageIndex = pageIndex;
        }

        internal void OnPageChanged(int pageIndex, int pageSize, int totalCount)
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
                pageSize = pageSize > 0 ? pageSize : DefaultPageSize;
                if (pageSize != PageSize)
                {
                    pageSizeChanged = true;

                    CoerceValue(PageCountProperty);

                    var pageCount = (TotalCount + pageSize - 1) / pageSize;
                    if (pageCount == 0)
                    {
                        pageCount = 1;
                    }

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
                    if (pageCount == 0)
                    {
                        pageCount = 1;
                    }
                    if (PageIndex > pageCount)
                    {
                        pageIndex = pageCount;
                        pageIndexChanged = true;
                    }
                }
            }

            if (pageIndexChanged || pageSizeChanged || totalCountChanged)
            {
                var pageCount = (totalCount + pageSize - 1) / pageSize;
                if (pageCount == 0)
                {
                    pageCount = 1;
                }

                PageChanged?.Invoke(this, new PageChangedEventArgs
                {
                    PageIndex = PageIndex,
                    PageCount = PageCount,
                    PageSize = PageSize,
                    TotalCount = TotalCount
                });

                ArrangePageButtons();
            }
        }

        void ArrangePageButtons()
        {
            if (_pageButtonWrapper != null)
            {
                _pageButtonWrapper.Children.Clear();

                _pageButtonWrapper.Children.Add(_firstLastButtons[0]);

                var middleIndex = PageButtonsCount / 2;
                var startPageIndex = PageIndex - middleIndex;
                var totalButtonsCount = PageCount > PageButtonsCount ? PageButtonsCount : PageCount - 1;

                if (PageIndex <= middleIndex)
                {
                    startPageIndex = 1;
                }

                if (PageCount <= PageButtonsCount + 1)
                {
                    startPageIndex = 1;
                }
                else
                {
                    var value = PageCount - PageIndex;
                    if (value < middleIndex)
                    {
                        startPageIndex -= middleIndex - value;
                    }
                }

                for (int i = startPageIndex; i < PageIndex; i++)
                {
                    var button = _pageButtons[i - startPageIndex];
                    button.Content = i;
                    _pageButtonWrapper.Children.Add(button);
                }

                _txtPageIndex.Text = PageIndex.ToString();
                _pageButtonWrapper.Children.Add(_txtPageIndex);

                var rightButtonsCount = totalButtonsCount - PageIndex + startPageIndex;
                var rightButtonStartIndex = PageIndex - startPageIndex;
                for (int i = 0; i < rightButtonsCount; i++)
                {
                    var button = _pageButtons[i + rightButtonStartIndex];
                    button.Content = PageIndex + i + 1;
                    _pageButtonWrapper.Children.Add(button);
                }
                _pageButtonWrapper.Children.Add(_firstLastButtons[1]);
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

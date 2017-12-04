using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfRichText
{
    /// <summary>
    /// Interaction logic for BindableRichTextbox.xaml
    /// </summary>
    public partial class RichTextEditor : UserControl
    {

        public static readonly DependencyProperty IsToolBarVisibleProperty =
          DependencyProperty.Register("IsToolBarVisible", typeof(bool), typeof(RichTextEditor),
          new PropertyMetadata(true));


        public static readonly DependencyProperty IsContextMenuEnabledProperty =
          DependencyProperty.Register("IsContextMenuEnabled", typeof(bool), typeof(RichTextEditor),
          new PropertyMetadata(false));


        public static readonly DependencyProperty IsReadOnlyProperty =
          DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(RichTextEditor),
          new PropertyMetadata(false));

        private TextRange textRange = null;

        const string ImageTypeFilterDefault = "图片文件(*png,*.jpg,*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
        const int ImageMaxSizeDefaultMB = 2;


        private int _imageMaxSizeMB;

        public int ImageMaxSizeMB
        {
            get { return _imageMaxSizeMB; }
            set
            {
                if (value > 0)
                {
                    _imageMaxSizeMB = value;
                }
                else
                {
                    _imageMaxSizeMB = ImageMaxSizeDefaultMB;
                }
            }
        }

        private string _imageTypeFilter;

        public string ImageTypeFilter
        {
            get { return _imageTypeFilter; }
            set
            {
                if (!string.IsNullOrEmpty(_imageTypeFilter))
                {
                    _imageTypeFilter = value;
                }
                else
                {
                    _imageTypeFilter = ImageTypeFilterDefault;
                }
            }
        }


        private Action<string> _showMessageCallback;

        public Action<string> ShowMessageCallback
        {
            get { return _showMessageCallback; }
            set { _showMessageCallback = value; }
        }

        UploadImageManager _uploadImageManager;

        static string[] _PreDefinedFonts = new[] { "宋体", "微软雅黑", "楷体", "黑体", "隶书",
                "Microsoft YaHei UI", "courier new", "arial", "arial black", "comic sans ms",
                "impact", "times new roman" };

        static double[] _PreDefinedFontSizes = new double[] { 10, 11, 12, 14, 16, 18, 20, 24 };

        List<string> _availableFonts = new List<string>();

        bool _isInitialized = false;

        public RichTextEditor()
        {
            InitializeComponent();

            ImageMaxSizeMB = ImageMaxSizeDefaultMB;
            ImageTypeFilter = ImageTypeFilterDefault;

            _uploadImageManager = new UploadImageManager();

            var fontList = Fonts.SystemFontFamilies.Select(p => p.FamilyNames).ToList();
            foreach (var item in _PreDefinedFonts)
            {
                if (fontList.Any(p => p.Values.Contains(item, StringComparer.CurrentCultureIgnoreCase)))
                {
                    _availableFonts.Add(item);
                }
            }

            CmbFontFamilies.ItemsSource = _availableFonts;
            CmbFontSizes.ItemsSource = _PreDefinedFontSizes;

            Loaded += RichTextEditor_Loaded;
        }

        private void RichTextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                SetToolBarElementsEnabled(IsFocused);


                _isInitialized = true;
            }

            var index = _availableFonts.FindIndex(p => p == "微软雅黑");
            if (index != -1)
            {
                CmbFontFamilies.SelectedIndex = index;
            }
            else
            {
                index = _availableFonts.FindIndex(p => p == "宋体");
                if (index != -1)
                {
                    CmbFontFamilies.SelectedIndex = index;
                }
                else
                {
                    CmbFontFamilies.SelectedIndex = 0;
                }
            }

            CmbFontSizes.SelectedIndex = 0;
        }


        public string Text
        {
            get
            {
                TextRange tr = new TextRange(MainRichTextBox.Document.ContentStart, MainRichTextBox.Document.ContentEnd);

                using (MemoryStream ms = new MemoryStream())
                {
                    tr.Save(ms, DataFormats.Text);
                    string text = Encoding.UTF8.GetString(ms.ToArray());
                    return text;
                }
            }
            set
            {
                MainRichTextBox.Document.Blocks.Clear();
                Paragraph paragraph = new Paragraph();
                Run run = new Run() { Text = value };
                paragraph.Inlines.Add(run);
                MainRichTextBox.Document.Blocks.Add(paragraph);
            }
        }

        public string HtmlText
        {
            get
            {
                string xamlText = XamlWriter.Save(MainRichTextBox.Document);
                //xamlText = @"<FlowDocument PagePadding=""5,0,5,0"" AllowDrop=""True"" NumberSubstitution.CultureSource=""User"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""><Paragraph><Image Source=""file:///D:/MyFiles/History/云合景从项目/切图/网站建设/产品维护（添加）.png"" Stretch=""None"" IsEnabled=""True"" /></Paragraph></FlowDocument>";

                var html = HtmlFromXamlConverter.ConvertXamlToHtmlWithoutHtmlAndBody(xamlText, true);
                return html;
            }
            set
            {
                var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(value, true);
                var sr = new StringReader(xaml);
                var xr = System.Xml.XmlReader.Create(sr);
                MainRichTextBox.Document = (FlowDocument)XamlReader.Load(xr);
            }
        }



        public bool IsToolBarVisible
        {
            get { return (GetValue(IsToolBarVisibleProperty) as bool? == true); }
            set
            {
                SetValue(IsToolBarVisibleProperty, value);
                //this.mainToolBar.Visibility = (value == true) ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public bool IsContextMenuEnabled
        {
            get
            {
                return (GetValue(IsContextMenuEnabledProperty) as bool? == true);
            }
            set
            {
                SetValue(IsContextMenuEnabledProperty, value);
            }
        }

        public bool IsReadOnly
        {
            get { return (GetValue(IsReadOnlyProperty) as bool? == true); }
            set
            {
                SetValue(IsReadOnlyProperty, value);
                SetValue(IsToolBarVisibleProperty, !value);
                SetValue(IsContextMenuEnabledProperty, !value);
            }
        }

        private void FontColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            MainRichTextBox.Selection.ApplyPropertyValue(ForegroundProperty, e.NewValue.ToString(CultureInfo.InvariantCulture));
        }


        private void BtnInsertLink_Click(object sender, RoutedEventArgs e)
        {
            this.textRange = new TextRange(this.MainRichTextBox.Selection.Start, this.MainRichTextBox.Selection.End);
            this.uriInputPopup.IsOpen = true;
        }

        private void UriCancelClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.uriInputPopup.IsOpen = false;
            this.uriInput.Text = string.Empty;
        }

        private void UriSubmitClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.uriInputPopup.IsOpen = false;
            this.MainRichTextBox.Selection.Select(this.textRange.Start, this.textRange.End);
            if (!string.IsNullOrEmpty(this.uriInput.Text))
            {
                this.textRange = new TextRange(this.MainRichTextBox.Selection.Start, this.MainRichTextBox.Selection.End);
                Hyperlink hlink = new Hyperlink(this.textRange.Start, this.textRange.End);
                hlink.NavigateUri = new Uri(this.uriInput.Text, UriKind.RelativeOrAbsolute);
                this.uriInput.Text = string.Empty;
            }
            else
                this.MainRichTextBox.Selection.ClearAllProperties();
        }

        private void uriInput_KeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    this.UriSubmitClick(sender, e);
                    break;
                case Key.Escape:
                    this.UriCancelClick(sender, e);
                    break;
                default:
                    break;
            }
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!this.IsContextMenuEnabled == true)
                e.Handled = true;
        }

        private void BtnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = ImageTypeFilter;

            if (dialog.ShowDialog() == true)
            {
                var filePath = dialog.FileName;

                var fileInfo = new System.IO.FileInfo(filePath);
                if (fileInfo.Length > ImageMaxSizeMB * 1024 * 1024)
                {
                    var msg = $"文件大小在{ImageMaxSizeMB}M以内！";
                    if (_showMessageCallback != null)
                    {
                        _showMessageCallback(msg);
                    }
                    else
                    {
                        MessageBox.Show(msg);
                    }
                    return;
                }

                if (_uploadImageManager != null)
                {
                    var url = "";
                    var key = $"Image{DateTime.Now.ToString("yyMMddHHmmssfff")}{Guid.NewGuid().ToString().Substring(0, 3)}{System.IO.Path.GetExtension(filePath)}";
                    if (_uploadImageManager.UploadImage(filePath, key, out url))
                    {
                        filePath = url;
                    }
                    else
                    {
                        var msg = "图片上传失败！";
                        if (_showMessageCallback != null)
                        {
                            _showMessageCallback(msg);
                        }
                        else
                        {
                            MessageBox.Show(msg);
                        }
                        return;
                    }
                }

                Image img = new Image();
                BitmapImage bImg = new BitmapImage(new Uri(filePath));
                img.IsEnabled = true;
                img.Source = bImg;

                img.Stretch = Stretch.None;  //图片缩放模式

                new InlineUIContainer(img, MainRichTextBox.Selection.Start); //插入图片到选定位置
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var data = Clipboard.GetDataObject();

            //if (Clipboard.ContainsImage())
            //{
            //    e.Handled = true;
            //    //Clipboard.Clear();
            //    return;
            //}
            ////Get Unicode Text
            //string paste = Clipboard.GetText();
            //Clipboard.Clear();
            //Clipboard.SetText(paste);
            MainRichTextBox.Paste();
            //e.Handled = true;
        }

        void SetToolBarElementsEnabled(bool enabled)
        {
            CmbFontFamilies.IsEnabled = enabled;
            CmbFontSizes.IsEnabled = enabled;
            BtnInsertLink.IsEnabled = enabled;
            BtnUploadImage.IsEnabled = enabled;
        }

        private void MainRichTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SetToolBarElementsEnabled(true);
        }

        private void MainRichTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetToolBarElementsEnabled(false);
        }

        private void CmbFontSizes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = CmbFontSizes.SelectedValue;
            if (newValue != null)
            {
                MainRichTextBox.Selection.ApplyPropertyValue(FontSizeProperty, newValue);
            }
        }

        private void CmbFontFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = CmbFontFamilies.SelectedValue;
            if (newValue != null)
            {
                MainRichTextBox.Selection.ApplyPropertyValue(FontFamilyProperty, newValue);
            }
        }

        private void MainRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextRange selectionRange = new TextRange(MainRichTextBox.Selection.Start, MainRichTextBox.Selection.End);

            var fontFamilyValue = selectionRange.GetPropertyValue(FontFamilyProperty);
            if (fontFamilyValue != DependencyProperty.UnsetValue)
            {
                var fontFamily = (FontFamily)fontFamilyValue;
                CmbFontFamilies.SelectedValue = _availableFonts.SingleOrDefault(p => fontFamily.FamilyNames.Values.Contains(p, StringComparer.OrdinalIgnoreCase));
            }
            else
            {
                CmbFontFamilies.SelectedValue = null;
            }

            CmbFontSizes.SelectedValue = selectionRange.GetPropertyValue(FontSizeProperty);

            if (selectionRange.GetPropertyValue(FontWeightProperty).ToString() == "Bold")
            {
                BtnBold.IsChecked = true;
            }
            else
            {
                BtnBold.IsChecked = false;
            }

            if (selectionRange.GetPropertyValue(FontStyleProperty).ToString() == "Italic")
            {
                BtnItalic.IsChecked = true;
            }
            else
            {
                BtnItalic.IsChecked = false;
            }

            if (selectionRange.GetPropertyValue(Inline.TextDecorationsProperty) == TextDecorations.Underline)
            {
                BtnUnderline.IsChecked = true;
            }
            else
            {
                BtnUnderline.IsChecked = false;
            }

            if (selectionRange.GetPropertyValue(FlowDocument.TextAlignmentProperty).ToString() == "Left")
            {
                BtnAlignLeft.IsChecked = true;
            }

            if (selectionRange.GetPropertyValue(FlowDocument.TextAlignmentProperty).ToString() == "Center")
            {
                BtnAlignCenter.IsChecked = true;
            }

            if (selectionRange.GetPropertyValue(FlowDocument.TextAlignmentProperty).ToString() == "Right")
            {
                BtnAlignRight.IsChecked = true;
            }

        }
    }
}

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
using WpfTest.Controls.RichTextBox.Controls;

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
        const int ImageMaxSizeDefaultBytes = 2 * 1024 * 1024;

        private int _imageMaxSizeBytes;

        public int ImageMaxSizeBytes
        {
            get
            {
                return _imageMaxSizeBytes;
            }
            set
            {
                if (value > 0)
                {
                    _imageMaxSizeBytes = value;
                }
                else
                {
                    _imageMaxSizeBytes = ImageMaxSizeDefaultBytes;
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

        IUploadImageManager _uploadImageManager;

        static string[] _PreDefinedFonts = new[] { "宋体", "微软雅黑", "楷体", "黑体", "隶书",
                "Microsoft YaHei UI", "courier new", "arial", "arial black", "comic sans ms",
                "impact", "times new roman" };

        static double[] _PreDefinedFontSizes = new double[] { 10, 11, 12, 14, 16, 18, 20, 24 };

        List<string> _availableFonts = new List<string>();

        string _defaultFontFamily = "宋体";

        public string DefaultFontFamily
        {
            get { return _defaultFontFamily; }
            set { _defaultFontFamily = value; }
        }

        string _defaultFontSize = "14";

        public string DefaultFontSize
        {
            get { return _defaultFontSize; }
            set { _defaultFontSize = value; }
        }

        bool _isInitialized = false;

        public RichTextEditor()
        {
            InitializeComponent();

            ImageMaxSizeBytes = ImageMaxSizeDefaultBytes;
            ImageTypeFilter = ImageTypeFilterDefault;

            _uploadImageManager = new UploadImageManagerQiniu();

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

                var index = _availableFonts.FindIndex(p => p == _defaultFontFamily);
                if (index != -1)
                {
                    CmbFontFamilies.SelectedIndex = index;
                }
                else
                {
                    CmbFontFamilies.SelectedIndex = 0;
                    _defaultFontFamily = _availableFonts[0];
                }

                index = Array.FindIndex(_PreDefinedFontSizes, p => p.ToString() == _defaultFontSize);
                if (index != -1)
                {
                    CmbFontSizes.SelectedIndex = index;
                }
                else
                {
                    CmbFontSizes.SelectedIndex = 0;
                    _defaultFontSize = _PreDefinedFontSizes[0].ToString();
                }

                var style = new Style(typeof(Paragraph));
                style.Setters.Add(new Setter(MarginProperty, new Thickness(0, 5, 0, 5)));
                style.Setters.Add(new Setter(FontFamilyProperty, new FontFamily(_defaultFontFamily)));
                style.Setters.Add(new Setter(FontSizeProperty, double.Parse(_defaultFontSize)));

                MainRichTextBox.Resources.Add(typeof(Paragraph), style);

                _isInitialized = true;
            }
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
                ProcessWpfImages();

                string xamlText = XamlWriter.Save(MainRichTextBox.Document);

                // <FlowDocument PagePadding="5,0,5,0" AllowDrop="True" NumberSubstitution.CultureSource="User" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"><BlockUIContainer TextAlignment="Justify"><Image Width="400" Height="134"><Image.Source><BitmapImage BaseUri="pack://payload:,,wpf1,/Xaml/Document.xaml" UriSource="./Image1.bmp" CacheOption="OnLoad" /></Image.Source></Image></BlockUIContainer></FlowDocument>
                //xamlText = @"<FlowDocument PagePadding=""5,0,5,0"" AllowDrop=""True"" NumberSubstitution.CultureSource=""User"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""><Paragraph><Image Source=""file:///D:/MyFiles/History/云合景从项目/切图/网站建设/产品维护（添加）.png"" Stretch=""None"" IsEnabled=""True"" /></Paragraph></FlowDocument>";

                Debug.WriteLine("xaml:");
                Debug.WriteLine(xamlText);
                Debug.WriteLine("");

                var html = HtmlFromXamlConverter.ConvertXamlToHtmlWithoutHtmlAndBody(xamlText, true, _defaultFontFamily, _defaultFontSize);
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

        public void ProcessWpfImages()
        {
            ProcessWpfImages(MainRichTextBox.Document.Blocks);
        }

        void ProcessWpfImages(BlockCollection blocks)
        {
            foreach (Block b in blocks)
            {
                if (b is Paragraph p)
                {
                    foreach (Inline inline in p.Inlines)
                    {
                        if (inline is InlineUIContainer uiContainer)
                        {
                            if (uiContainer.Child is Image targetImage)
                            {
                                ProcessWpfImage(targetImage);
                            }
                        }
                        else if (inline is Span span)
                        {
                            foreach (Inline inline2 in span.Inlines)
                            {
                                if (inline2 is InlineUIContainer uiContainer2)
                                {
                                    if (uiContainer2.Child is Image targetImage)
                                    {
                                        ProcessWpfImage(targetImage);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (b is BlockUIContainer blockUIContainer)
                {
                    if (blockUIContainer.Child is Image targetImage)
                    {
                        ProcessWpfImage(targetImage);
                    }
                }
                else if (b is Table t)
                {
                    foreach (var rg in t.RowGroups)
                    {
                        foreach (var row in rg.Rows)
                        {
                            foreach (var cell in row.Cells)
                            {
                                ProcessWpfImages(cell.Blocks);
                            }
                        }
                    }
                }
            }
        }

        void ProcessWpfImage(Image image)
        {
            if (image.Source is BitmapImage bitmap && !(bitmap.UriSource?.ToString().StartsWith("http") == true))
            {
                var data = GetImageByteArray(bitmap);
                if (_uploadImageManager != null)
                {
                    _uploadImageManager.UploadImage(data, "", out string url);
                    image.Source = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
                }
            }
        }

        private byte[] GetImageByteArray(BitmapImage src)
        {

            //PngBitmapEncoder png = new PngBitmapEncoder();
            //png.Frames.Add(BitmapFrame.Create(src));
            //using (var fileStream = File.Create("d:/test.png"))
            //{
            //    png.Save(fileStream);
            //}

            //JpegBitmapEncoder jpg = new JpegBitmapEncoder();
            //jpg.Frames.Add(BitmapFrame.Create(src));
            //using (var fs = File.Create("d:/test2.jpg"))
            //{
            //    jpg.Save(fs);
            //}


            MemoryStream stream = new MemoryStream();
            //BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)src));
            encoder.Save(stream);
            stream.Flush();
            return stream.ToArray();
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

        private void UriInput_KeyPressed(object sender, KeyEventArgs e)
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
            var dialog = new OpenFileDialog
            {
                Filter = ImageTypeFilter
            };

            if (dialog.ShowDialog() == true)
            {
                var filePath = dialog.FileName;

                byte[] data = null;
                using (var fileStream = File.OpenRead(filePath))
                {
                    data = new byte[fileStream.Length];
                    fileStream.Read(data, 0, data.Length);
                }

                if (data.Length > ImageMaxSizeBytes)
                {
                    var msg = $"文件大小在{GetImageMaxSizeDescription()}以内！";
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

                var imgUrl = "";

                if (_uploadImageManager != null)
                {
                    var key = $"Image{DateTime.Now.ToString("yyMMddHHmmssfff")}{Guid.NewGuid().ToString().Substring(0, 3)}{System.IO.Path.GetExtension(filePath)}";
                    try
                    {
                        _uploadImageManager.UploadImage(data, key, out imgUrl);
                    }
                    catch (Exception)
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

                if (string.IsNullOrEmpty(imgUrl))
                {
                    var stream = new MemoryStream(data);

                    Image img = new Image();
                    BitmapImage bImg = new BitmapImage();
                    bImg.BeginInit();
                    bImg.StreamSource = stream;
                    bImg.EndInit();

                    img.Width = bImg.PixelWidth;
                    img.Height = bImg.PixelHeight;

                    img.IsEnabled = true;
                    img.Source = bImg;
                    
                    new InlineUIContainer(img, MainRichTextBox.Selection.Start); //插入图片到选定位置
                }
                else
                {
                    Image img = new Image();
                    BitmapImage bImg = new BitmapImage(new Uri(imgUrl));

                    bImg.DownloadCompleted += (sender1, e1) =>
                    {
                        img.Width = bImg.PixelWidth;
                        img.Height = bImg.PixelHeight;
                    };
                    img.IsEnabled = true;
                    img.Source = bImg;

                    new InlineUIContainer(img, MainRichTextBox.Selection.Start); //插入图片到选定位置
                }

                //targetBitmap.CopyPixels(pixelData, width * 4, 0);
                //BitmapSource bmpSource = BitmapSource.Create(width, height, 147, 147, PixelFormats.Bgra32, null, pixelData, width * 4);

            }
        }

        string GetImageMaxSizeDescription()
        {
            var value = _imageMaxSizeBytes / (1024 * 1024);
            if (value > 0)
            {
                return $"{value}M";
            }
            else if ((value = _imageMaxSizeBytes / 1024) > 0)
            {
                return $"{value}K";
            }
            else
            {
                return $"{value}B";
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

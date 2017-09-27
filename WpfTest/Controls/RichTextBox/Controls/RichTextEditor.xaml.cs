using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <summary></summary>
        public static readonly DependencyProperty IsToolBarVisibleProperty =
          DependencyProperty.Register("IsToolBarVisible", typeof(bool), typeof(RichTextEditor),
          new PropertyMetadata(true));

        /// <summary></summary>
        public static readonly DependencyProperty IsContextMenuEnabledProperty =
          DependencyProperty.Register("IsContextMenuEnabled", typeof(bool), typeof(RichTextEditor),
          new PropertyMetadata(false));

        /// <summary></summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
          DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(RichTextEditor),
          new PropertyMetadata(false));

        /// <summary></summary>
        public static readonly DependencyProperty AvailableFontsProperty =
          DependencyProperty.Register("AvailableFonts", typeof(Collection<String>), typeof(RichTextEditor),
          new PropertyMetadata(new Collection<String>(
              new List<String>()
        //{
        //    "Arial",
        //    "Courier New",
        //    "Tahoma",
        //    "Times New Roman"
        //}
        )));


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


        List<string> _fontList;

        /// <summary></summary>
        public RichTextEditor()
        {
            InitializeComponent();

            ImageMaxSizeMB = ImageMaxSizeDefaultMB;
            ImageTypeFilter = ImageTypeFilterDefault;

            _uploadImageManager = new UploadImageManager();

            InstalledFontCollection MyFont = new InstalledFontCollection();

            _fontList = MyFont.Families.Select(p => p.Name).ToList();
            foreach (var item in _fontList)
            {
                AvailableFonts.Add(item);
            }

            Loaded += RichTextEditor_Loaded;
        }

        private void RichTextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            var index = _fontList.FindIndex(p => p == "微软雅黑");
            if (index != -1)
            {
                CmbFonts.SelectedIndex = index;
            }
            else
            {
                index = _fontList.FindIndex(p => p == "宋体");
                if (index != -1)
                {
                    CmbFonts.SelectedIndex = index;
                }
                else
                {
                    CmbFonts.SelectedIndex = 0;
                }
            }
        }

        /// <summary></summary>
        public string Text
        {
            get
            {
                TextRange tr = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);

                using (MemoryStream ms = new MemoryStream())
                {
                    tr.Save(ms, DataFormats.Text);
                    string text = Encoding.UTF8.GetString(ms.ToArray());
                    return text;
                }
            }
            set
            {
                mainRTB.Document.Blocks.Clear();
                Paragraph paragraph = new Paragraph();
                Run run = new Run() { Text = value };
                paragraph.Inlines.Add(run);
                mainRTB.Document.Blocks.Add(paragraph);
            }
        }

        public string HtmlText
        {
            get
            {
                string xamlText = XamlWriter.Save(mainRTB.Document);
                //xamlText = @"<FlowDocument PagePadding=""5,0,5,0"" AllowDrop=""True"" NumberSubstitution.CultureSource=""User"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""><Paragraph><Image Source=""file:///D:/MyFiles/History/云合景从项目/切图/网站建设/产品维护（添加）.png"" Stretch=""None"" IsEnabled=""True"" /></Paragraph></FlowDocument>";
                
                var html = HtmlFromXamlConverter.ConvertXamlToHtmlWithoutHtmlAndBody(xamlText, true);
                return html;
            }
            set
            {
                var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(value, true);
                var sr = new StringReader(xaml);
                var xr = System.Xml.XmlReader.Create(sr);
                mainRTB.Document = (FlowDocument)XamlReader.Load(xr);              
            }
        }


        /// <summary></summary>
        public bool IsToolBarVisible
        {
            get { return (GetValue(IsToolBarVisibleProperty) as bool? == true); }
            set
            {
                SetValue(IsToolBarVisibleProperty, value);
                //this.mainToolBar.Visibility = (value == true) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary></summary>
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

        /// <summary></summary>
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

        /// <summary></summary>
        public Collection<String> AvailableFonts
        {
            get { return GetValue(AvailableFontsProperty) as Collection<String>; }
            set
            {
                SetValue(AvailableFontsProperty, value);
            }
        }

        private void FontColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            this.mainRTB.Selection.ApplyPropertyValue(ForegroundProperty, e.NewValue.ToString(CultureInfo.InvariantCulture));
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.mainRTB != null && this.mainRTB.Selection != null)
                this.mainRTB.Selection.ApplyPropertyValue(FontFamilyProperty, e.AddedItems[0]);
        }

        private void insertLink_Click(object sender, RoutedEventArgs e)
        {
            this.textRange = new TextRange(this.mainRTB.Selection.Start, this.mainRTB.Selection.End);
            this.uriInputPopup.IsOpen = true;
        }

        private void uriCancelClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.uriInputPopup.IsOpen = false;
            this.uriInput.Text = string.Empty;
        }

        private void uriSubmitClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.uriInputPopup.IsOpen = false;
            this.mainRTB.Selection.Select(this.textRange.Start, this.textRange.End);
            if (!string.IsNullOrEmpty(this.uriInput.Text))
            {
                this.textRange = new TextRange(this.mainRTB.Selection.Start, this.mainRTB.Selection.End);
                Hyperlink hlink = new Hyperlink(this.textRange.Start, this.textRange.End);
                hlink.NavigateUri = new Uri(this.uriInput.Text, UriKind.RelativeOrAbsolute);
                this.uriInput.Text = string.Empty;
            }
            else
                this.mainRTB.Selection.ClearAllProperties();
        }

        private void uriInput_KeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    this.uriSubmitClick(sender, e);
                    break;
                case Key.Escape:
                    this.uriCancelClick(sender, e);
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

                new InlineUIContainer(img, mainRTB.Selection.Start); //插入图片到选定位置
            }
        }
    }
}

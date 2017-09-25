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

        List<string> _fontList;

        /// <summary></summary>
        public RichTextEditor()
        {
            InitializeComponent();

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
                TextRange tr = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);

                using (MemoryStream ms = new MemoryStream())
                {
                    tr.Save(ms, DataFormats.Xaml);
                    string xamlText = Encoding.UTF8.GetString(ms.ToArray());
                    var html = HtmlFromXamlConverter.ConvertXamlToHtmlWithoutHtmlAndBody(xamlText, false);
                    return html;
                }
            }
            set
            {
                var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(value, false);
                mainRTB.Document.Blocks.Clear();

                if (!string.IsNullOrEmpty(xaml))
                {
                    using (MemoryStream xamlMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xaml)))
                    {
                        ParserContext parser = new ParserContext();
                        parser.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                        parser.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                        //FlowDocument doc = new FlowDocument();
                        Section section = XamlReader.Load(xamlMemoryStream, parser) as Section;

                        mainRTB.Document.Blocks.Add(section);
                    }
                }
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

    }
}

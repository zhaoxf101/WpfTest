using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfTest
{
    /// <summary>
    /// TextBoxTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TextBoxTestWindow : Window
    {
        public TextBoxTestWindow()
        {
            InitializeComponent();
        }
        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var changes = e.Changes;
            var builder = new StringBuilder();
            var text = textBox.Text;
            var caretIndex = 0;

            foreach (var change in changes)
            {
                var added = text.Substring(change.Offset, change.AddedLength);
                foreach (var c in added)
                {
                    if (char.IsNumber(c))
                    {
                        builder.Append(c);
                    }
                }

                if (added.Length != builder.Length)
                {
                    text = text.Substring(0, change.Offset) + builder.ToString() + text.Substring(change.Offset + change.AddedLength);
                    caretIndex = change.Offset + builder.Length;
                }
                builder.Clear();
            }

            if (textBox.Text != text)
            {
                textBox.Text = text;
                textBox.CaretIndex = caretIndex; 
            }
        }

        private void TxtName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            
        }

        private void HyperlinkOperation_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

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
using System.Windows.Threading;

namespace CustomBA
{
    /// <summary>
    /// DialogMessage.xaml 的交互逻辑
    /// </summary>
    public partial class DialogMessage : Window
    {
        DispatcherTimer _messageWindowCloseTimer;
        const int _MessageWindowCloseCounter = 1;
        bool _autoClose;

        private DialogMessage()
        {
            InitializeComponent();
        }

        public DialogMessage(MessageBoxButton button, MessageBoxImage icon, string message1, string message2, bool autoClose) : this()
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    BtnCancel.Visibility = Visibility.Collapsed;
                    break;
            }

            switch (icon)
            {
                case MessageBoxImage.Question:
                case MessageBoxImage.Information:
                case MessageBoxImage.Warning:
                    ImageIcon.Source = new BitmapImage(new Uri("/CustomBA;component/images/icon2.png", UriKind.RelativeOrAbsolute));
                    BtnCancel.Style = (Style)Resources["InfoCancelButtonStyle"];
                    BtnOK.Style = (Style)Resources["InfoOKButtonStyle"];
                    break;
                case MessageBoxImage.Error:
                    ImageIcon.Source = new BitmapImage(new Uri("/CustomBA;component/images/icon.png", UriKind.RelativeOrAbsolute));
                    BtnCancel.Style = (Style)Resources["ErrorButtonStyle"];
                    break;
            }

            TxtLine1.Text = message1;
            TxtLine2.Text = message2;

            _autoClose = autoClose;
            if (autoClose)
            {
                if (_messageWindowCloseTimer == null)
                {
                    _messageWindowCloseTimer = new DispatcherTimer();
                    _messageWindowCloseTimer.Interval = TimeSpan.FromSeconds(_MessageWindowCloseCounter);
                    _messageWindowCloseTimer.Tick += (s, re) =>
                    {
                        DialogResult = true;
                        _messageWindowCloseTimer.Stop();
                    };
                }
                _messageWindowCloseTimer.Start();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    DragMove();
            //}
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            if (_autoClose)
            {
                _messageWindowCloseTimer.Stop();
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            if (_autoClose)
            {
                _messageWindowCloseTimer.Stop();
            }
        }
    }
}

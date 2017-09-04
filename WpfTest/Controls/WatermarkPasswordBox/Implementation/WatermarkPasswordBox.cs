/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System.Security;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Xceed.Wpf.Toolkit
{
    public class WatermarkPasswordBox : Control
    {
        PasswordBox _passwordBox;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WatermarkPasswordBox), new PropertyMetadata(""));

        public string Password
        {
            [SecuritySafeCritical]
            get
            {
                if (_passwordBox != null)
                {
                    return _passwordBox.Password;
                }

                return "";
            }
            set
            {
                if (_passwordBox != null)
                {
                    _passwordBox.Password = value;
                }
            }
        }



        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkPasswordBox), new PropertyMetadata(""));


        public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(WatermarkPasswordBox), new UIPropertyMetadata(null));
        public DataTemplate WatermarkTemplate
        {
            get
            {
                return (DataTemplate)GetValue(WatermarkTemplateProperty);
            }
            set
            {
                SetValue(WatermarkTemplateProperty, value);
            }
        }


        public static readonly DependencyProperty PasswordCharProperty = DependencyProperty.Register("PasswordChar", typeof(char), typeof(WatermarkPasswordBox)
          , new UIPropertyMetadata('\u25CF', OnPasswordCharChanged)); //default is black bullet

        public char PasswordChar
        {
            get
            {
                return (char)GetValue(PasswordCharProperty);
            }

            set
            {
                SetValue(PasswordCharProperty, value);
            }
        }

        public static readonly DependencyProperty KeepWatermarkOnGotFocusProperty = DependencyProperty.Register("KeepWatermarkOnGotFocus", typeof(bool), typeof(WatermarkPasswordBox), new UIPropertyMetadata(false));
        public bool KeepWatermarkOnGotFocus
        {
            get
            {
                return (bool)GetValue(KeepWatermarkOnGotFocusProperty);
            }
            set
            {
                SetValue(KeepWatermarkOnGotFocusProperty, value);
            }
        }

        private static void OnPasswordCharChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var watermarkPasswordBox = o as WatermarkPasswordBox;
            if (watermarkPasswordBox != null)
            {
                watermarkPasswordBox.OnPasswordCharChanged((char)e.OldValue, (char)e.NewValue);
            }
        }

        protected virtual void OnPasswordCharChanged(char oldValue, char newValue)
        {
            if (_passwordBox != null)
            {
                _passwordBox.PasswordChar = newValue;
            }
        }




        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(WatermarkPasswordBox), new PropertyMetadata(0, OnMaxLengthChanged));

        private static void OnMaxLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var watermarkPasswordBox = o as WatermarkPasswordBox;
            if (watermarkPasswordBox != null)
            {
                watermarkPasswordBox.OnMaxLengthChanged((int)e.OldValue, (int)e.NewValue);
            }
        }

        protected virtual void OnMaxLengthChanged(int oldValue, int newValue)
        {
            if (_passwordBox != null)
            {
                _passwordBox.MaxLength = newValue;
            }
        }


        public WatermarkPasswordBox()
        {
            this.Password = string.Empty;
        }


        public static readonly RoutedEvent PasswordChangedEvent = EventManager.RegisterRoutedEvent("PasswordChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler)
          , typeof(WatermarkPasswordBox));
        public event RoutedEventHandler PasswordChanged
        {
            add
            {
                AddHandler(PasswordChangedEvent, value);
            }
            remove
            {
                RemoveHandler(PasswordChangedEvent, value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _passwordBox = GetTemplateChild("PART_ContentHost") as PasswordBox;
            if (_passwordBox != null)
            {
                _passwordBox.PasswordChanged += _passwordBox_PasswordChanged;
            }

        }

        private void _passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Text = "".PadLeft(_passwordBox.Password.Length, PasswordChar);
            RaiseEvent(new RoutedEventArgs(PasswordChangedEvent, this));
        }
    }
}

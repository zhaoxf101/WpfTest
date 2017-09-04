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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTest
{
   
    public class DateTimePicker : Control
    {
        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));

            //EventManager.RegisterClassHandler(typeof(DatePicker), UIElement.GotFocusEvent, new RoutedEventHandler(OnGotFocus));
            //KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(DatePicker), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            //KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(DatePicker), new FrameworkPropertyMetadata(false));
            //IsEnabledProperty.OverrideMetadata(typeof(DatePicker), new UIPropertyMetadata(new PropertyChangedCallback(OnIsEnabledChanged)));

            //ControlsTraceLogger.AddControl(TelemetryControls.DatePicker);
        }
    }
}

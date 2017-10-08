using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace WpfTest
{

    public class TabGroupItem : HeaderedItemsControl
    {
        static TabGroupItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabGroupItem), new FrameworkPropertyMetadata(typeof(TabGroupItem)));
            IsTabStopProperty.OverrideMetadata(typeof(TabGroupItem), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        }

        /// <summary>
        /// ExpandDirection specifies to which direction the content will expand
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the ExpandDirection property.
        /// Default Value: ExpandDirection.Down
        /// </summary>
        public static readonly DependencyProperty ExpandDirectionProperty =
                DependencyProperty.Register(
                        "ExpandDirection",
                        typeof(ExpandDirection),
                        typeof(TabGroupItem),
                        new FrameworkPropertyMetadata(
                                ExpandDirection.Down /* default value */,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                new PropertyChangedCallback(OnVisualStatePropertyChanged)),
                        new ValidateValueCallback(IsValidExpandDirection));

        private static void OnVisualStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        void UpdateVisualState()
        {

        }

        private static bool IsValidExpandDirection(object o)
        {
            ExpandDirection value = (ExpandDirection)o;

            return (value == ExpandDirection.Down ||
                    value == ExpandDirection.Left ||
                    value == ExpandDirection.Right ||
                    value == ExpandDirection.Up);
        }

        /// <summary>
        ///     The DependencyProperty for the IsExpanded property.
        ///     Default Value: false
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
                DependencyProperty.Register(
                        "IsExpanded",
                        typeof(bool),
                        typeof(TabGroupItem),
                        new FrameworkPropertyMetadata(
                                BooleanBoxes.FalseBox /* default value */,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                                new PropertyChangedCallback(OnIsExpandedChanged)));

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabGroupItem groupItem = (TabGroupItem)d;
            bool newValue = (bool)e.NewValue;

            if (newValue)
            {
                groupItem.OnExpanded();
            }
            else
            {
                groupItem.OnCollapsed();
            }

            groupItem.UpdateVisualState();
        }

        /// <summary>
        /// IsExpanded indicates whether the TabGroupItem is currently expanded.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Expanded event.
        /// </summary>
        public static readonly RoutedEvent ExpandedEvent =
            EventManager.RegisterRoutedEvent("Expanded",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TabGroupItem)
            );

        /// <summary>
        /// Expanded event. It is fired when IsExpanded changed from false to true.
        /// </summary>
        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }

        /// <summary>
        /// Collapsed event.
        /// </summary>
        public static readonly RoutedEvent CollapsedEvent =
            EventManager.RegisterRoutedEvent("Collapsed",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TabGroupItem)
            );

        /// <summary>
        /// Collapsed event. It is fired when IsExpanded changed from true to false.
        /// </summary>
        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }


        internal void ChangeVisualState(bool useTransitions)
        {
            // Handle the Common states
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (IsMouseOver)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            // Handle the Focused states
            if (IsKeyboardFocused)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }

            if (IsExpanded)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateExpanded);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateCollapsed);
            }

            switch (ExpandDirection)
            {
                case ExpandDirection.Down:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateExpandDown);
                    break;

                case ExpandDirection.Up:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateExpandUp);
                    break;

                case ExpandDirection.Left:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateExpandLeft);
                    break;

                default:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateExpandRight);
                    break;
            }
        }

        /// <summary>
        /// A virtual function that is called when the IsExpanded property is changed to true. 
        /// Default behavior is to raise an ExpandedEvent.
        /// </summary>
        protected virtual void OnExpanded()
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = TabGroupItem.ExpandedEvent;
            args.Source = this;
            RaiseEvent(args);

            var expandSiteWrapper = GetTemplateChild("ExpandSiteWrapper") as StackPanel;
            var expandSite = GetTemplateChild("ExpandSite") as StackPanel;

            if (expandSiteWrapper != null && expandSite != null)
            {
                DoubleAnimation animation = new DoubleAnimation(0, expandSite.ActualHeight, new Duration(TimeSpan.FromMilliseconds(250)));
                expandSiteWrapper.BeginAnimation(HeightProperty, animation);
            }

            var tabControl = LogicalTreeHelper.GetParent(this) as TabControl;
            if (tabControl != null)
            {
                var groupItems = TreeHelper.FindLogicalDirectChildren<TabGroupItem>(tabControl);

                foreach (var item in groupItems)
                {
                    if (!object.ReferenceEquals(item, this))
                    {
                        if (item.IsExpanded)
                        {
                            item.IsExpanded = false;
                        }
                    }
                }
            }


        }

        /// <summary>
        /// A virtual function that is called when the IsExpanded property is changed to false. 
        /// Default behavior is to raise a CollapsedEvent.
        /// </summary>
        protected virtual void OnCollapsed()
        {
            RaiseEvent(new RoutedEventArgs(TabGroupItem.CollapsedEvent, this));

            var expandSiteWrapper = GetTemplateChild("ExpandSiteWrapper") as StackPanel;
            var expandSite = GetTemplateChild("ExpandSite") as StackPanel;

            if (expandSiteWrapper != null && expandSite != null)
            {
                DoubleAnimation animation = new DoubleAnimation(expandSite.ActualHeight, 0, new Duration(TimeSpan.FromMilliseconds(250)));
                expandSiteWrapper.BeginAnimation(HeightProperty, animation);
            }
        }
    }
}

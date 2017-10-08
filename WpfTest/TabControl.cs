using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using MS.Utility;

using System;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.Core.Utilities;
using System.Collections.Generic;
using MarketingPlatform.Client;

namespace WpfTest
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(TabItem))]
    [TemplatePart(Name = "PART_SelectedContentHost", Type = typeof(ContentPresenter))]
    public class TabControl : Selector
    {
        static TabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));
            IsTabStopProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
        }

        /// <summary>
        ///     Default TabControl constructor
        /// </summary>
        /// <remarks>
        ///     Automatic determination of current Dispatcher. Use alternative constructor
        ///     that accepts a Dispatcher for best performance.
        /// </remarks>
        public TabControl() : base()
        {
        }

        private static readonly DependencyPropertyKey SelectedContentPropertyKey = DependencyProperty.RegisterReadOnly("SelectedContent", typeof(object), typeof(TabControl), new FrameworkPropertyMetadata((object)null));

        /// <summary>
        ///     The DependencyProperty for the SelectedContent property.
        ///     Flags:              None
        ///     Default Value:      null
        /// </summary>
        public static readonly DependencyProperty SelectedContentProperty = SelectedContentPropertyKey.DependencyProperty;

        /// <summary>
        ///     SelectedContent is the Content of current SelectedItem.
        /// This property is updated whenever the selection is changed.
        /// It always keeps a reference to active TabItem.Content
        /// Used for aliasing in default TabControl Style
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedContent
        {
            get
            {
                return GetValue(SelectedContentProperty);
            }
            internal set
            {
                SetValue(SelectedContentPropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey SelectedContentTemplatePropertyKey = DependencyProperty.RegisterReadOnly("SelectedContentTemplate", typeof(DataTemplate), typeof(TabControl), new FrameworkPropertyMetadata((DataTemplate)null));

        /// <summary>
        ///     The DependencyProperty for the SelectedContentTemplate property.
        ///     Flags:              None
        ///     Default Value:      null
        /// </summary>
        public static readonly DependencyProperty SelectedContentTemplateProperty = SelectedContentTemplatePropertyKey.DependencyProperty;

        /// <summary>
        ///     SelectedContentTemplate is the ContentTemplate of current SelectedItem.
        /// This property is updated whenever the selection is changed.
        /// It always keeps a reference to active TabItem.ContentTemplate
        /// It is used for aliasing in default TabControl Style
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplate SelectedContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(SelectedContentTemplateProperty);
            }
            internal set
            {
                SetValue(SelectedContentTemplatePropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey SelectedContentTemplateSelectorPropertyKey = DependencyProperty.RegisterReadOnly("SelectedContentTemplateSelector", typeof(DataTemplateSelector), typeof(TabControl), new FrameworkPropertyMetadata((DataTemplateSelector)null));

        /// <summary>
        ///     The DependencyProperty for the SelectedContentTemplateSelector property.
        ///     Flags:              None
        ///     Default Value:      null
        /// </summary>
        public static readonly DependencyProperty SelectedContentTemplateSelectorProperty = SelectedContentTemplateSelectorPropertyKey.DependencyProperty;

        /// <summary>
        ///     SelectedContentTemplateSelector allows the app writer to provide custom style selection logic.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplateSelector SelectedContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(SelectedContentTemplateSelectorProperty);
            }
            internal set
            {
                SetValue(SelectedContentTemplateSelectorPropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey SelectedContentStringFormatPropertyKey =
                DependencyProperty.RegisterReadOnly("SelectedContentStringFormat",
                        typeof(String),
                        typeof(TabControl),
                        new FrameworkPropertyMetadata((String)null));

        /// <summary>
        ///     The DependencyProperty for the SelectedContentStringFormat property.
        ///     Flags:              None
        ///     Default Value:      null
        /// </summary>
        public static readonly DependencyProperty SelectedContentStringFormatProperty =
                SelectedContentStringFormatPropertyKey.DependencyProperty;


        /// <summary>
        ///     ContentStringFormat is the format used to display the content of
        ///     the control as a string.  This arises only when no template is
        ///     available.
        /// </summary>
        public String SelectedContentStringFormat
        {
            get { return (String)GetValue(SelectedContentStringFormatProperty); }
            internal set { SetValue(SelectedContentStringFormatPropertyKey, value); }
        }


        /// <summary>
        ///     The DependencyProperty for the ContentTemplate property.
        ///     Flags:              None
        ///     Default Value:      null
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(TabControl), new FrameworkPropertyMetadata((DataTemplate)null));

        /// <summary>
        /// ContentTemplate is the ContentTemplate to apply to TabItems
        /// that do not have the ContentTemplate or ContentTemplateSelector properties
        /// defined
        /// </summary>
        /// <value></value>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }
            set
            {
                SetValue(ContentTemplateProperty, value);
            }
        }

        /// <summary>
        ///     The DependencyProperty for the ContentTemplateSelector property.
        ///     Flags:              None
        ///     Default Value:      null
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(TabControl), new FrameworkPropertyMetadata((DataTemplateSelector)null));

        /// <summary>
        ///     ContentTemplateSelector allows the app writer to provide custom style selection logic.
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty);
            }
            set
            {
                SetValue(ContentTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        ///     The DependencyProperty for the ContentStringFormat property.
        ///     Flags:              None
        ///     Default Value:      null
        /// </summary>
        public static readonly DependencyProperty ContentStringFormatProperty =
                DependencyProperty.Register(
                        "ContentStringFormat",
                        typeof(String),
                        typeof(TabControl),
                        new FrameworkPropertyMetadata((String)null));


        /// <summary>
        ///     ContentStringFormat is the format used to display the content of
        ///     the control as a string.  This arises only when no template is
        ///     available.
        /// </summary>
        public String ContentStringFormat
        {
            get { return (String)GetValue(ContentStringFormatProperty); }
            set { SetValue(ContentStringFormatProperty, value); }
        }

        public int SelectedTabIndex
        {
            get
            {
                var index = 0;
                var tabItems = TreeHelper.FindLogicalChildren<TabItem>(this, true);
                foreach (var tabItem in tabItems)
                {
                    if (tabItem.IsSelected)
                    {
                        break;
                    }
                    index++;
                }

                if (index == tabItems.Count)
                {
                    return -1;
                }
                else
                {
                    return index;
                }
            }
            set
            {
                var tabItems = TreeHelper.FindLogicalChildren<TabItem>(this, true);

                for (int i = 0; i < tabItems.Count; i++)
                {
                    if (i == value)
                    {
                        tabItems[i].IsSelected = true;
                    }
                    else
                    {
                        tabItems[i].IsSelected = false;
                    }
                }

                OnSelectionChanged();
            }
        }

        public TabItem SelectedTabItem
        {
            get
            {
                var tabItems = TreeHelper.FindLogicalChildren<TabItem>(this, true);
                foreach (var tabItem in tabItems)
                {
                    if (tabItem.IsSelected)
                    {
                        return tabItem;
                    }
                }

                return null;
            }
            set
            {
                var tabItems = TreeHelper.FindLogicalChildren<TabItem>(this, true);
                foreach (var tabItem in tabItems)
                {
                    if (object.ReferenceEquals(tabItem, value))
                    {
                        tabItem.IsSelected = true;
                        OnSelectionChanged();
                    }
                }
            }
        }

        public List<TabItem> TabItems
        {
            get
            {
                return TreeHelper.FindLogicalChildren<TabItem>(this, true);
            }
        }

        internal void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateNormal, useTransitions);
            }
        }

        /// <summary>
        ///     This virtual method in called when IsInitialized is set to true and it raises an Initialized event
        /// </summary>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            ItemContainerGenerator.StatusChanged += new EventHandler(OnGeneratorStatusChanged);
        }
        /// <summary>
        /// Called when the Template's tree has been generated. When Template gets expanded we ensure that SelectedContent is in sync
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateSelectedContent();
        }

        /// <summary>
        /// A virtual function that is called when the selection is changed. Default behavior
        /// is to raise a SelectionChangedEvent
        /// </summary>
        /// <param name="e">The inputs for this event. Can be raised (default behavior) or processed
        ///   in some other way.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            OnSelectionChanged();
        }

        internal void OnSelectionChanged()
        {
            if (IsKeyboardFocusWithin)
            {
                // If keyboard focus is within the control, make sure it is going to the correct place
                TabItem item = SelectedTabItem;
                if (item != null)
                {
                    item.SetFocus();
                }
            }
            UpdateSelectedContent();
        }

        /// <summary>
        /// Updates the current selection when Items has changed
        /// </summary>
        /// <param name="e">Information about what has changed</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Remove && SelectedTabIndex == -1)
            {
                // If we remove the selected item we should select the previous item
                int startIndex = e.OldStartingIndex + 1;
                if (startIndex > TabItems.Count)
                    startIndex = 0;
                TabItem nextTabItem = FindNextTabItem(startIndex, -1);
                if (nextTabItem != null)
                {
                    nextTabItem.SetCurrentValue(TabItem.IsSelectedProperty, BooleanBoxes.TrueBox);
                }
            }
        }

        /// <summary>
        /// This is the method that responds to the KeyDown event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            TabItem nextTabItem = null;

            // Handle [Ctrl][Shift]Tab, Home and End cases
            // We have special handling here because if focus is inside the TabItem content we cannot
            // cycle through TabItem because the content is not part of the TabItem visual tree

            int direction = 0;
            int startIndex = -1;
            switch (e.Key)
            {
                case Key.Tab:
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        startIndex = SelectedTabIndex;
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                            direction = -1;
                        else
                            direction = 1;
                    }
                    break;
                case Key.Home:
                    direction = 1;
                    startIndex = -1;
                    break;
                case Key.End:
                    direction = -1;
                    startIndex = TabItems.Count;
                    break;
            }


            nextTabItem = FindNextTabItem(startIndex, direction);

            Logger.Log($"startIndex: {startIndex} direction: {direction} nextTabItem: {nextTabItem}");

            if (nextTabItem != null && nextTabItem != SelectedTabItem)
            {
                e.Handled = nextTabItem.SetFocus();
            }

            if (!e.Handled)
            {
                base.OnKeyDown(e);
            }
        }

        private TabItem FindNextTabItem(int startIndex, int direction)
        {
            TabItem nextTabItem = null;
            if (direction != 0)
            {
                int index = startIndex;
                for (int i = 0; i < TabItems.Count; i++)
                {
                    index += direction;
                    if (index >= TabItems.Count)
                        index = 0;
                    else if (index < 0)
                        index = TabItems.Count - 1;

                    TabItem tabItem = TabItems[index];
                    if (tabItem != null && tabItem.IsEnabled && tabItem.Visibility == Visibility.Visible)
                    {
                        nextTabItem = tabItem;
                        break;
                    }
                }
            }
            return nextTabItem;
        }

        /// <summary>
        /// Return true if the item is (or is eligible to be) its own ItemUI
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            //return (item is TabItem);
            return true;
        }

        /// <summary> Create or identify the element used to display the given item. </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabItem();
        }

        internal ContentPresenter SelectedContentPresenter
        {
            get
            {
                return GetTemplateChild(SelectedContentHostTemplateName) as ContentPresenter;
            }
        }

        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                //if (HasItems && _selectedItems.Count == 0)
                //{
                //    SetCurrentValueInternal(SelectedIndexProperty, 0);
                //}

                UpdateSelectedContent();
            }
        }

        // When selection is changed we need to copy the active TabItem content in SelectedContent property
        // SelectedContent is aliased in the TabControl style
        private void UpdateSelectedContent()
        {
            if (SelectedTabIndex < 0)
            {
                SelectedContent = null;
                SelectedContentTemplate = null;
                SelectedContentTemplateSelector = null;
                SelectedContentStringFormat = null;
                return;
            }

            TabItem tabItem = SelectedTabItem;
            if (tabItem != null)
            {
                SelectedContent = tabItem.Content;
                ContentPresenter scp = SelectedContentPresenter;
                if (scp != null)
                {
                    scp.HorizontalAlignment = tabItem.HorizontalContentAlignment;
                    scp.VerticalAlignment = tabItem.VerticalContentAlignment;
                }

                // Use tabItem's template or selector if specified, otherwise use TabControl's
                if (tabItem.ContentTemplate != null || tabItem.ContentTemplateSelector != null || tabItem.ContentStringFormat != null)
                {
                    SelectedContentTemplate = tabItem.ContentTemplate;
                    SelectedContentTemplateSelector = tabItem.ContentTemplateSelector;
                    SelectedContentStringFormat = tabItem.ContentStringFormat;
                }
                else
                {
                    SelectedContentTemplate = ContentTemplate;
                    SelectedContentTemplateSelector = ContentTemplateSelector;
                    SelectedContentStringFormat = ContentStringFormat;
                }
            }
        }


        // Part name used in the style. The class TemplatePartAttribute should use the same name
        private const string SelectedContentHostTemplateName = "PART_SelectedContentHost";

    }
}


//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Diagnostics;
using MS.Internal;
using MS.Internal.KnownBoxes;
using MS.Utility;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System.Windows.Data;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using MarketingPlatform.Client;
using Xceed.Wpf.Toolkit.Core.Utilities;

// Disable CS3001: Warning as Error: not CLS-compliant
#pragma warning disable 3001

namespace WpfTest
{
    /// <summary>
    ///     A child item of TabControl.
    /// </summary>
    //[DefaultEvent("IsSelectedChanged")]
    public class TabItem : HeaderedContentControl
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------

        #region Constructors

        /// <summary>
        ///     Default DependencyObject constructor
        /// </summary>
        /// <remarks>
        ///     Automatic determination of current Dispatcher. Use alternative constructor
        ///     that accepts a Dispatcher for best performance.
        /// </remarks>
        public TabItem() : base()
        {
        }

        static TabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItem), new FrameworkPropertyMetadata(typeof(TabItem)));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(TabItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TabItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Properties
        //
        //-------------------------------------------------------------------

        /// <summary>
        ///     Indicates whether this TabItem is selected.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
                Selector.IsSelectedProperty.AddOwner(typeof(TabItem),
                        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.Journal,
                                new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        ///     Indicates whether this TabItem is selected.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, BooleanBoxes.Box(value)); }
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem tabItem = d as TabItem;

            bool isSelected = (bool)e.NewValue;

            TabControl parentTabControl = tabItem.TabControlParent;
            //if (parentTabControl != null)
            //{
            //    parentTabControl.RaiseIsSelectedChangedAutomationEvent(tabItem, isSelected);
            //}

            if (isSelected)
            {
                tabItem.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, tabItem));
            }
            else
            {
                tabItem.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, tabItem));
            }


            // KeyboardNavigation use bounding box reduced with DirectionalNavigationMargin when calculating the next element in directional navigation
            // Because TabItem use negative margins some TabItems overlap which would changes the directional navigation if we don't reduce the bounding box
            //if (isSelected)
            //{
            //    Binding binding = new Binding("Margin");
            //    binding.Source = tabItem;
            //    BindingOperations.SetBinding(tabItem, KeyboardNavigation.DirectionalNavigationMarginProperty, binding);
            //}
            //else
            //{
            //    BindingOperations.ClearBinding(tabItem, KeyboardNavigation.DirectionalNavigationMarginProperty);
            //}

            tabItem.UpdateVisualState();
        }

        internal void UpdateVisualState()
        {
            UpdateVisualState(true);
        }

        internal void UpdateVisualState(bool useTransitions)
        {
            if (!VisualStateChangeSuspended)
            {
                ChangeVisualState(useTransitions);
            }
        }

        internal bool VisualStateChangeSuspended
        {
            get { return ReadControlFlag(ControlBoolFlags.VisualStateChangeSuspended); }
            set { WriteControlFlag(ControlBoolFlags.VisualStateChangeSuspended, value); }
        }

        internal bool ReadControlFlag(ControlBoolFlags reqFlag)
        {
            return (_controlBoolField & reqFlag) != 0;
        }

        internal void WriteControlFlag(ControlBoolFlags reqFlag, bool set)
        {
            if (set)
            {
                _controlBoolField |= reqFlag;
            }
            else
            {
                _controlBoolField &= (~reqFlag);
            }
        }


        void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStateManager.GoToState(this, VisualStates.StateDisabled, useTransitions);
            }
            else if (IsMouseOver)
            {
                VisualStateManager.GoToState(this, VisualStates.StateMouseOver, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateNormal, useTransitions);
            }

            // Update the SelectionStates group
            if (IsSelected)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateSelected, VisualStates.StateUnselected);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateUnselected, useTransitions);
            }

            if (IsKeyboardFocused)
            {
                VisualStateManager.GoToState(this, VisualStates.StateFocused, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateUnfocused, useTransitions);
            }
        }

        /// <summary>
        ///     Event indicating that the IsSelected property is now true.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            HandleIsSelectedChanged(true, e);
        }

        /// <summary>
        ///     Event indicating that the IsSelected property is now false.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            HandleIsSelectedChanged(false, e);
        }

        private void HandleIsSelectedChanged(bool newValue, RoutedEventArgs e)
        {
            var tab = TabControlParent;
            if (tab != null)
            {
                var tabItems = tab.TabItems;
                var tabGroupItems = tab.TabGroupItems;
                var _this = this;

                // 处理 TabGroupItem 选中状态。
                var currentGroupItem = tabGroupItems.Where(p =>
                {
                    foreach (var item in p.Items)
                    {
                        if (object.ReferenceEquals(_this, item))
                        {
                            return true;
                        }
                    }
                    return false;
                }).FirstOrDefault();

                if (currentGroupItem != null)
                {
                    if (newValue)
                    {
                        currentGroupItem.IsSelected = true;
                    }
                    IsInnerSelectorVisible = newValue;
                    IsOuterSelectorVisible = false;
                }
                else
                {
                    IsInnerSelectorVisible = false;
                    IsOuterSelectorVisible = newValue;

                    if (newValue)
                    {
                        IsSeparatorVisible = false;
                    }
                }

                if (newValue)
                {
                    foreach (var item in tabGroupItems)
                    {
                        if (!object.ReferenceEquals(currentGroupItem, item))
                        {
                            item.IsSelected = false;

                            if (item.IsExpanded)
                            {
                                item.IsSeparatorVisible = false;
                            }
                        }
                    }

                    foreach (var item in tab.TabItems)
                    {
                        if (item != null && !object.ReferenceEquals(item, this))
                        {
                            if (item.IsSelected)
                            {
                                item.IsSelected = false;
                                break;
                            }
                        }
                    }

                    //
                    // 重置所有的分隔线。
                    //

                    // 处理 TabGroupItem。
                    foreach (var groupItem2 in tabGroupItems)
                    {
                        if (!groupItem2.IsExpanded && !groupItem2.IsSelected)
                        {
                            groupItem2.IsSeparatorVisible = true;
                        }

                        // 最后一个 TabItem 不要分隔线。
                        TabItem lastTabItem = null;
                        for (int i = 0; i < groupItem2.Items.Count; i++)
                        {
                            if (groupItem2.Items[i] is TabItem tabItem)
                            {
                                tabItem.IsSeparatorVisible = true;
                                lastTabItem = tabItem;
                            }
                        }
                        if (lastTabItem != null)
                        {
                            lastTabItem.IsSeparatorVisible = false;
                        }
                    }

                    // 处理一级 TabItem。
                    foreach (var item in tab.Items)
                    {
                        if (item is TabItem tabItem)
                        {
                            if (!tabItem.IsSelected)
                            {
                                tabItem.IsSeparatorVisible = true;
                            }
                        }
                    }


                    // 处理一级元素
                    for (int i = 0; i < tab.Items.Count - 1; i++)
                    {
                        var current = tab.Items[i];
                        var next = tab.Items[i + 1];

                        if (current is TabItem tabItem)
                        {
                            if (tabItem.IsSelected)
                            {
                                break;
                            }
                            else
                            {
                                if (next is TabItem tabItem2 && tabItem2.IsSelected)
                                {
                                    tabItem.IsSeparatorVisible = false;
                                    break;
                                }
                                else if (next is TabGroupItem groupItem2 && groupItem2.IsSelected)
                                {
                                    tabItem.IsSeparatorVisible = false;
                                    break;
                                }
                            }
                        }
                        else if (current is TabGroupItem groupItem)
                        {
                            if (groupItem.IsSelected)
                            {
                                break;
                            }
                            else
                            {
                                if (next is TabItem tabItem2 && tabItem2.IsSelected)
                                {
                                    groupItem.IsSeparatorVisible = false;
                                    break;
                                }
                                else if (next is TabGroupItem groupItem2 && groupItem2.IsSelected)
                                {
                                    groupItem.IsSeparatorVisible = false;
                                    break;
                                }
                            }
                        }
                    }




                 
                }

                tab.OnSelectionChanged();
            }

            //RaiseEvent(e);
        }

        bool _isSeparatorVisible = true;
        public bool IsSeparatorVisible
        {
            get
            {
                return _isSeparatorVisible;
            }
            set
            {
                if (GetTemplateChild("SeparatedBd") is Border separatedBd)
                {
                    if (value)
                    {
                        separatedBd.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        separatedBd.Visibility = Visibility.Collapsed;
                    }
                }

                _isSeparatorVisible = value;
            }
        }


        bool _isInnerSelectorVisible;
        public bool IsInnerSelectorVisible
        {
            get
            {
                return _isInnerSelectorVisible;
            }
            set
            {
                if (GetTemplateChild("SelectedBd") is Border selectedBd)
                {
                    if (value)
                    {
                        selectedBd.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        selectedBd.Visibility = Visibility.Collapsed;
                    }
                }

                _isInnerSelectorVisible = value;
            }
        }

        bool _isOuterSelectorVisible;
        public bool IsOuterSelectorVisible
        {
            get
            {
                return _isOuterSelectorVisible;
            }
            set
            {
                if (GetTemplateChild("OuterBd") is Border selectedBd)
                {
                    if (value)
                    {
                        selectedBd.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        selectedBd.Visibility = Visibility.Collapsed;
                    }
                }

                _isOuterSelectorVisible = value;
            }
        }

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// This is the method that responds to the MouseLeftButtonDownEvent event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // We should process only the direct events in case TabItem is the selected one
            // otherwise we are getting this event when we click on TabItem content because it is in the logical subtree
            if (e.Source == this || !IsSelected)
            {
                if (SetFocus())
                    e.Handled = true;
            }
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Focus event handler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnPreviewGotKeyboardFocus(e);
            if (!e.Handled && e.NewFocus == this)
            {
                if (!IsSelected && TabControlParent != null)
                {
                    SetCurrentValue(IsSelectedProperty, BooleanBoxes.TrueBox);
                    // If focus moved in result of selection - handle the event to prevent setting focus back on the new item
                    if (e.OldFocus != Keyboard.FocusedElement)
                    {
                        e.Handled = true;
                    }
                    else if (GetBoolField(BoolField.SetFocusOnContent))
                    {
                        TabControl parentTabControl = TabControlParent;
                        if (parentTabControl != null)
                        {
                            // Save the parent and check for null to make sure that SetCurrentValue didn't have a change handler
                            // that removed the TabItem from the tree.
                            ContentPresenter selectedContentPresenter = parentTabControl.SelectedContentPresenter;
                            if (selectedContentPresenter != null)
                            {
                                parentTabControl.UpdateLayout(); // Wait for layout
                                bool success = selectedContentPresenter.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                                // If we successfully move focus inside the content then don't set focus to the header
                                if (success)
                                    e.Handled = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The Access key for this control was invoked.
        /// </summary>
        protected override void OnAccessKey(AccessKeyEventArgs e)
        {
            SetFocus();
        }

        /// <summary>
        ///     This method is invoked when the Content property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the Content property.</param>
        /// <param name="newContent">The new value of the Content property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            // If this is the selected TabItem then we should update TabControl.SelectedContent
            if (IsSelected)
            {
                TabControl tabControl = TabControlParent;
                if (tabControl != null)
                {
                    //if (newContent == BindingExpressionBase.DisconnectedItem)
                    //{
                    //    // don't let {DisconnectedItem} bleed into the UI
                    //    newContent = null;
                    //}

                    tabControl.SelectedContent = newContent;
                }
            }
        }

        /// <summary>
        ///     This method is invoked when the ContentTemplate property changes.
        /// </summary>
        /// <param name="oldContentTemplate">The old value of the ContentTemplate property.</param>
        /// <param name="newContentTemplate">The new value of the ContentTemplate property.</param>
        protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
        {
            base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);

            // If this is the selected TabItem then we should update TabControl.SelectedContentTemplate
            if (IsSelected)
            {
                TabControl tabControl = TabControlParent;
                if (tabControl != null)
                {
                    tabControl.SelectedContentTemplate = newContentTemplate;
                }
            }
        }

        /// <summary>
        ///     This method is invoked when the ContentTemplateSelector property changes.
        /// </summary>
        /// <param name="oldContentTemplateSelector">The old value of the ContentTemplateSelector property.</param>
        /// <param name="newContentTemplateSelector">The new value of the ContentTemplateSelector property.</param>
        protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
        {
            base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);

            // If this is the selected TabItem then we should update TabControl.SelectedContentTemplateSelector
            if (IsSelected)
            {
                TabControl tabControl = TabControlParent;
                if (tabControl != null)
                {
                    tabControl.SelectedContentTemplateSelector = newContentTemplateSelector;
                }
            }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Methods
        //
        //-------------------------------------------------------------------

        #region Private Methods

        private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (!e.Handled && e.Scope == null)
            {
                TabItem tabItem = sender as TabItem;

                if (e.Target == null)
                {
                    e.Target = tabItem;
                }
                else if (!tabItem.IsSelected) // If TabItem is not active it is a scope for its content elements
                {
                    e.Scope = tabItem;
                    e.Handled = true;
                }
            }
        }

        internal bool SetFocus()
        {
            bool returnValue = false;

            var groupItem = TreeHelper.FindLogicalParentWithStopType<TabGroupItem, TabControl>(this);
            if (groupItem != null && !groupItem.IsExpanded)
            {
                groupItem.IsExpanded = true;
                groupItem.UpdateLayout();
            }

            if (!GetBoolField(BoolField.SettingFocus))
            {
                TabItem currentFocus = Keyboard.FocusedElement as TabItem;

                // If current focus was another TabItem in the same TabControl - dont set focus on content
                bool setFocusOnContent = ((currentFocus == this) || (currentFocus == null) || (currentFocus.TabControlParent != this.TabControlParent));
                SetBoolField(BoolField.SettingFocus, true);
                SetBoolField(BoolField.SetFocusOnContent, setFocusOnContent);
                try
                {
                    returnValue = Focus() || setFocusOnContent;
                }
                finally
                {
                    SetBoolField(BoolField.SettingFocus, false);
                    SetBoolField(BoolField.SetFocusOnContent, false);
                }
            }

            return returnValue;
        }

        private TabControl TabControlParent
        {
            get
            {
                //return ItemsControl.ItemsControlFromItemContainer(this) as TabControl;
                return TreeHelper.FindParent<TabControl>(this);
            }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Fields
        //
        //-------------------------------------------------------------------

        #region Private Fields

        private bool GetBoolField(BoolField field)
        {
            return (_tabItemBoolFieldStore & field) != 0;
        }

        private void SetBoolField(BoolField field, bool value)
        {
            if (value)
            {
                _tabItemBoolFieldStore |= field;
            }
            else
            {
                _tabItemBoolFieldStore &= (~field);
            }
        }

        [Flags]
        private enum BoolField
        {
            SetFocusOnContent = 0x10, // This flag determine if we want to set focus on active TabItem content
            SettingFocus = 0x20, // This flag indicates that the TabItem is in the process of setting focus

            // By default ListBoxItem is selectable
            DefaultValue = 0,
        }

        BoolField _tabItemBoolFieldStore = BoolField.DefaultValue;


        internal enum ControlBoolFlags : ushort
        {
            ContentIsNotLogical = 0x0001,            // used in contentcontrol.cs
            IsSpaceKeyDown = 0x0002,            // used in ButtonBase.cs
            HeaderIsNotLogical = 0x0004,            // used in HeaderedContentControl.cs, HeaderedItemsControl.cs
            CommandDisabled = 0x0008,            // used in ButtonBase.cs, MenuItem.cs
            ContentIsItem = 0x0010,            // used in contentcontrol.cs
            HeaderIsItem = 0x0020,            // used in HeaderedContentControl.cs, HeaderedItemsControl.cs
            ScrollHostValid = 0x0040,            // used in ItemsControl.cs
            ContainsSelection = 0x0080,            // used in TreeViewItem.cs
            VisualStateChangeSuspended = 0x0100,            // used in Control.cs
        }

        internal ControlBoolFlags _controlBoolField;   // Cache valid bits


        #endregion Private Fields
    }


}

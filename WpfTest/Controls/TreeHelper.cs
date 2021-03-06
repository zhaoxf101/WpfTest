﻿/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;

namespace Xceed.Wpf.Toolkit.Core.Utilities
{
    internal static class TreeHelper
    {
        /// <summary>
        /// Tries its best to return the specified element's parent. It will 
        /// try to find, in this order, the VisualParent, LogicalParent, LogicalTemplatedParent.
        /// It only works for Visual, FrameworkElement or FrameworkContentElement.
        /// </summary>
        /// <param name="element">The element to which to return the parent. It will only 
        /// work if element is a Visual, a FrameworkElement or a FrameworkContentElement.</param>
        /// <remarks>If the logical parent is not found (Parent), we check the TemplatedParent
        /// (see FrameworkElement.Parent documentation). But, we never actually witnessed
        /// this situation.</remarks>
        public static DependencyObject GetParent(DependencyObject element)
        {
            return TreeHelper.GetParent(element, true);
        }

        private static DependencyObject GetParent(DependencyObject element, bool recurseIntoPopup)
        {
            if (recurseIntoPopup)
            {
                // Case 126732 : To correctly detect parent of a popup we must do that exception case
                Popup popup = element as Popup;

                if ((popup != null) && (popup.PlacementTarget != null))
                    return popup.PlacementTarget;
            }

            Visual visual = element as Visual;
            DependencyObject parent = (visual == null) ? null : VisualTreeHelper.GetParent(visual);

            if (parent == null)
            {
                // No Visual parent. Check in the logical tree.
                FrameworkElement fe = element as FrameworkElement;

                if (fe != null)
                {
                    parent = fe.Parent;

                    if (parent == null)
                    {
                        parent = fe.TemplatedParent;
                    }
                }
                else
                {
                    FrameworkContentElement fce = element as FrameworkContentElement;

                    if (fce != null)
                    {
                        parent = fce.Parent;

                        if (parent == null)
                        {
                            parent = fce.TemplatedParent;
                        }
                    }
                }
            }

            return parent;
        }

        /// <summary>
        /// This will search for a parent of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="startingObject">The node where the search begins. This element is not checked.</param>
        /// <returns>Returns the found element. Null if nothing is found.</returns>
        public static T FindParent<T>(DependencyObject startingObject) where T : DependencyObject
        {
            return TreeHelper.FindParent<T>(startingObject, false, null);
        }

        /// <summary>
        /// This will search for a parent of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="startingObject">The node where the search begins.</param>
        /// <param name="checkStartingObject">Should the specified startingObject be checked first.</param>
        /// <returns>Returns the found element. Null if nothing is found.</returns>
        public static T FindParent<T>(DependencyObject startingObject, bool checkStartingObject) where T : DependencyObject
        {
            return TreeHelper.FindParent<T>(startingObject, checkStartingObject, null);
        }

        /// <summary>
        /// This will search for a parent of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="startingObject">The node where the search begins.</param>
        /// <param name="checkStartingObject">Should the specified startingObject be checked first.</param>
        /// <param name="additionalCheck">Provide a callback to check additional properties 
        /// of the found elements. Can be left Null if no additional criteria are needed.</param>
        /// <returns>Returns the found element. Null if nothing is found.</returns>
        /// <example>Button button = TreeHelper.FindParent&lt;Button&gt;( this, foundChild => foundChild.Focusable );</example>
        public static T FindParent<T>(DependencyObject startingObject, bool checkStartingObject, Func<T, bool> additionalCheck) where T : DependencyObject
        {
            T foundElement;
            DependencyObject parent = (checkStartingObject ? startingObject : TreeHelper.GetParent(startingObject, true));

            while (parent != null)
            {
                foundElement = parent as T;

                if (foundElement != null)
                {
                    if (additionalCheck == null)
                    {
                        return foundElement;
                    }
                    else
                    {
                        if (additionalCheck(foundElement))
                            return foundElement;
                    }
                }

                parent = TreeHelper.GetParent(parent, true);
            }

            return null;
        }

        public static T FindLogicalParentWithStopType<T, T2>(DependencyObject startingObject) where T : DependencyObject where T2 : DependencyObject
        {
            return TreeHelper.FindLogicalParentWithStopType<T, T2>(startingObject, false, null);
        }

        public static T FindLogicalParentWithStopType<T, T2>(DependencyObject startingObject, bool checkStartingObject) where T : DependencyObject where T2 : DependencyObject
        {
            return TreeHelper.FindParent<T>(startingObject, checkStartingObject, null);
        }

        public static T FindLogicalParentWithStopType<T, T2>(DependencyObject startingObject, bool checkStartingObject, Func<T, bool> additionalCheck) where T : DependencyObject where T2 : DependencyObject
        {
            T foundElement;
            DependencyObject parent = (checkStartingObject ? startingObject : LogicalTreeHelper.GetParent(startingObject));

            while (parent != null)
            {
                if (parent is T2)
                {
                    break;
                }

                foundElement = parent as T;

                if (foundElement != null)
                {
                    if (additionalCheck == null)
                    {
                        return foundElement;
                    }
                    else
                    {
                        if (additionalCheck(foundElement))
                            return foundElement;
                    }
                }

                parent = LogicalTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// This will search for a child of the specified type. The search is performed 
        /// hierarchically, breadth first (as opposed to depth first).
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="parent">The root of the tree to search for. This element itself is not checked.</param>
        /// <returns>Returns the found element. Null if nothing is found.</returns>
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            return TreeHelper.FindChild<T>(parent, null);
        }

        /// <summary>
        /// This will search for a child of the specified type. The search is performed 
        /// hierarchically, breadth first (as opposed to depth first).
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="parent">The root of the tree to search for. This element itself is not checked.</param>
        /// <param name="additionalCheck">Provide a callback to check additional properties 
        /// of the found elements. Can be left Null if no additional criteria are needed.</param>
        /// <returns>Returns the found element. Null if nothing is found.</returns>
        /// <example>Button button = TreeHelper.FindChild&lt;Button&gt;( this, foundChild => foundChild.Focusable );</example>
        public static T FindChild<T>(DependencyObject parent, Func<T, bool> additionalCheck) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            T child;

            for (int index = 0; index < childrenCount; index++)
            {
                child = VisualTreeHelper.GetChild(parent, index) as T;

                if (child != null)
                {
                    if (additionalCheck == null)
                    {
                        return child;
                    }
                    else
                    {
                        if (additionalCheck(child))
                            return child;
                    }
                }
            }

            for (int index = 0; index < childrenCount; index++)
            {
                child = TreeHelper.FindChild<T>(VisualTreeHelper.GetChild(parent, index), additionalCheck);

                if (child != null)
                    return child;
            }

            return null;
        }

        public static List<T> FindChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            return TreeHelper.FindChildren<T>(parent, null);
        }

        public static List<T> FindChildren<T>(DependencyObject parent, Func<T, bool> additionalCheck) where T : DependencyObject
        {
            List<T> list = new List<T>();
            FindChildrenRecursive(parent, additionalCheck, list);
            return list;
        }

        public static List<T> FindLogicalDirectChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            List<T> list = new List<T>();
            T child;
            foreach (var ch in LogicalTreeHelper.GetChildren(parent))
            {
                child = ch as T;

                if (child != null)
                {
                    list.Add(child);
                }
            }

            return list;
        }

        public static List<T> FindLogicalChildren<T>(DependencyObject parent, bool stopWhenFound) where T : DependencyObject
        {
            return TreeHelper.FindLogicalChildren<T>(parent, stopWhenFound, null);
        }

        public static List<T> FindLogicalChildren<T>(DependencyObject parent, bool stopWhenFound, Func<T, bool> additionalCheck) where T : DependencyObject
        {
            List<T> list = new List<T>();
            FindLogicalChildrenRecursiveDepthFirst(parent, stopWhenFound, additionalCheck, list);
            return list;
        }

        static void FindChildrenRecursive<T>(DependencyObject parent, Func<T, bool> additionalCheck, List<T> list) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            T child;
            for (int index = 0; index < childrenCount; index++)
            {
                child = VisualTreeHelper.GetChild(parent, index) as T;

                if (child != null)
                {
                    if (additionalCheck == null)
                    {
                        list.Add(child);
                    }
                    else
                    {
                        if (additionalCheck(child))
                        {
                            list.Add(child);
                        }
                    }
                }

            }

            for (int index = 0; index < childrenCount; index++)
            {
                var ch = VisualTreeHelper.GetChild(parent, index);

                if (ch != null)
                {
                    FindChildrenRecursive(ch, additionalCheck, list);
                }
            }
        }

        static void FindLogicalChildrenRecursiveDepthFirst<T>(DependencyObject parent, bool stopWhenFound, Func<T, bool> additionalCheck, List<T> list) where T : DependencyObject
        {
            T child;
            var found = false;
            foreach (var ch in LogicalTreeHelper.GetChildren(parent))
            {
                found = false;
                child = ch as T;

                if (child != null && (additionalCheck == null || additionalCheck(child)))
                {
                    list.Add(child);
                    found = true;
                }

                if (ch as DependencyObject != null && (!found || !stopWhenFound))
                {
                    FindLogicalChildrenRecursiveDepthFirst((DependencyObject)ch, stopWhenFound, additionalCheck, list);
                }
            }
        }


        /// <summary>
        /// Returns true if the specified element is a child of parent somewhere in the visual 
        /// tree. This method will work for Visual, FrameworkElement and FrameworkContentElement.
        /// </summary>
        /// <param name="element">The element that is potentially a child of the specified parent.</param>
        /// <param name="parent">The element that is potentially a parent of the specified element.</param>
        public static bool IsDescendantOf(DependencyObject element, DependencyObject parent)
        {
            return TreeHelper.IsDescendantOf(element, parent, true);
        }

        /// <summary>
        /// Returns true if the specified element is a child of parent somewhere in the visual 
        /// tree. This method will work for Visual, FrameworkElement and FrameworkContentElement.
        /// </summary>
        /// <param name="element">The element that is potentially a child of the specified parent.</param>
        /// <param name="parent">The element that is potentially a parent of the specified element.</param>
        public static bool IsDescendantOf(DependencyObject element, DependencyObject parent, bool recurseIntoPopup)
        {
            while (element != null)
            {
                if (element == parent)
                    return true;

                element = TreeHelper.GetParent(element, recurseIntoPopup);
            }

            return false;
        }
    }
}

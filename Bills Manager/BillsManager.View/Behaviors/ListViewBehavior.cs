using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BillsManager.View.Behaviors.AttachedBehaviors
{
    public sealed class ListViewBehavior
    {
        #region DeselectItemsOnMouseDown

        public static readonly DependencyProperty DeselectItemsOnClickOutProperty =
            DependencyProperty.RegisterAttached(
            "DeselectItemsOnClickOut", 
            typeof(bool), 
            typeof(ListViewBehavior), 
            new UIPropertyMetadata(false, OnDeselectItemsOnClickOutPropertyChanged));

        public static bool GetDeselectItemsOnClickOut(ListView ListView)
        {
            return Convert.ToBoolean(ListView.GetValue(DeselectItemsOnClickOutProperty));
        }

        public static void SetDeselectItemsOnClickOut(ListView ListView, bool value)
        {
            ListView.SetValue(DeselectItemsOnClickOutProperty, value);
        }

        private static void OnDeselectItemsOnClickOutPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            ListView item = depObj as ListView;
            if (item == null)
            {
                return;
            }

            if (e.NewValue.GetType() != typeof(bool))
            {
                return;
            }

            if (Convert.ToBoolean(e.NewValue))
            {
                item.MouseDown += OnListViewMouseDown;
            }
            else
            {
                item.MouseDown -= OnListViewMouseDown;
            }
        }

        private static void OnListViewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!object.ReferenceEquals(sender, e.Source))
            {
                return;
            }

            ListView item = e.Source as ListView;
            if ((item != null))
            {
                if (item.SelectionMode == SelectionMode.Single && item.SelectedItem != null)
                {
                    item.SelectedItem = null;
                }
                else if (item.SelectionMode != SelectionMode.Single && item.SelectedItems.Count > 0)
                {
                    item.SelectedItems.Clear();
                }
            }
        }

        #endregion

        #region ItemDoubleClickCommand

        public static DependencyProperty ItemDoubleClickCommandProperty = 
            DependencyProperty.RegisterAttached(
            "ItemDoubleClickCommand",
            typeof(ICommand),
            typeof(ListViewBehavior),
            new UIPropertyMetadata(OnItemDoubleClickCommandChanged));

        public static ICommand GetItemDoubleClickCommand(DependencyObject depObj)
        {
            return (ICommand)depObj.GetValue(ItemDoubleClickCommandProperty);
        }

        public static void SetItemDoubleClickCommand(DependencyObject depObj, ICommand value)
        {
            depObj.SetValue(ItemDoubleClickCommandProperty, value);
        }

        private static void OnItemDoubleClickCommandChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (depObj == null || depObj.GetType() != typeof(ListView))
                return;

            ListView item = (ListView)depObj;

            if (e.NewValue != null)
            {
                item.MouseDoubleClick += ListViewItemMouseDoubleClick;
            }
            else
            {
                item.MouseDoubleClick -= ListViewItemMouseDoubleClick;
            }

        }

        private static void ListViewItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender == null || !object.ReferenceEquals(sender, e.Source))
            {
                return;
            }

            ListView item = sender as ListView;

            if (IsInsideListViewItem(e.OriginalSource as DependencyObject))
            {
                ICommand command = GetItemDoubleClickCommand((DependencyObject)sender);
                if (command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }

        }

        private static bool IsInsideListViewItem(DependencyObject depObj)
        {
            if (depObj == null)
            {
                return false;

            }
            else if (depObj is ListViewItem)
            {
                return true;

            }

            dynamic parent = VisualTreeHelper.GetParent(depObj);
            return IsInsideListViewItem(parent);

        }

        #endregion
    }
}
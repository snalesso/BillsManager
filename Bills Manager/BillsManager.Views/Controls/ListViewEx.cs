using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BillsManager.Views.Controls
{
    public class ListViewEx : ListView
    {
        #region ICommands

        #region ItemDoubleLeftClickCommand

        public static DependencyProperty ItemDoubleLeftClickCommandProperty =
            DependencyProperty.Register("ItemDoubleLeftClickCommand", typeof(ICommand), typeof(ListViewEx));

        public ICommand ItemDoubleLeftClickCommand
        {
            get { return (ICommand)GetValue(ListViewEx.ItemDoubleLeftClickCommandProperty); }
            set { SetValue(ListViewEx.ItemDoubleLeftClickCommandProperty, value); }
        }

        public static DependencyProperty ItemDoubleLeftClickCommandParameterProperty =
            DependencyProperty.Register("ItemDoubleLeftClickCommandParameter", typeof(object), typeof(ListViewEx));

        public object ItemDoubleLeftClickCommandParameter
        {
            get { return (object)GetValue(ListViewEx.ItemDoubleLeftClickCommandParameterProperty); }
            set { SetValue(ListViewEx.ItemDoubleLeftClickCommandParameterProperty, value); }
        }

        #endregion

        #endregion

        #region events

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            e.Handled = !(this.IsChildOf<ListBoxItem>(e.OriginalSource as DependencyObject) || e.OriginalSource is ScrollViewer);
            //e.Handled = (e.OriginalSource is ScrollViewer);

            base.OnContextMenuOpening(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            //if (!this.IsChildOf<ListBoxItem>(e.OriginalSource as DependencyObject))
            if (e.OriginalSource is ScrollViewer)
            {
                if (this.SelectionMode == SelectionMode.Single)
                    this.SelectedItem = null;
                else
                    this.SelectedItems.Clear();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.LeftButton == MouseButtonState.Pressed)
                this.OnMouseDoubleLeftClick(e);
        }

        #region MouseDoubleLeftClick

        public event EventHandler MouseDoubleLeftClick;

        public void OnMouseDoubleLeftClick(MouseButtonEventArgs e)
        {
            if (this.MouseDoubleLeftClick != null)
            {
                this.MouseDoubleLeftClick(this, e);
            }

            if (this.IsChildOf<ListBoxItem>(e.OriginalSource as DependencyObject))
                this.OnItemMouseDoubleLeftClick(e);
        }

        #endregion

        #region ItemMouseDoubleLeftClick

        public event EventHandler ItemMouseDoubleLeftClick;

        public void OnItemMouseDoubleLeftClick(MouseButtonEventArgs e)
        {
            if (this.ItemDoubleLeftClickCommand != null && this.ItemDoubleLeftClickCommand.CanExecute(this.ItemDoubleLeftClickCommandParameter))
                this.ItemDoubleLeftClickCommand.Execute(this.ItemDoubleLeftClickCommandParameter);

            if (this.ItemMouseDoubleLeftClick != null)
            {
                this.ItemMouseDoubleLeftClick(this, e);
            }
        }

        #endregion

        #endregion

        #region support methods

        protected bool IsChildOf<T>(DependencyObject depObj)
        {
            var type = depObj.GetType();

            if (depObj == null || depObj is ListBox)
                return false;

            if (!(depObj is T))
            {
                return this.IsChildOf<T>(VisualTreeHelper.GetParent(depObj));
            }

            return true;
        }

        #endregion
    }
}
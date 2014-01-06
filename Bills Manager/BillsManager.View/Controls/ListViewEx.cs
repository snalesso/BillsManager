using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BillsManager.Views.Controls
{
    public class ListViewEx : ListView
    {
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            e.Handled = !this.IsChildOf<ListBoxItem>(e.OriginalSource as DependencyObject);

            base.OnContextMenuOpening(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (!this.IsChildOf<ListBoxItem>(e.OriginalSource as DependencyObject))
            {
                if (this.SelectionMode == SelectionMode.Single)
                    this.SelectedItem = null;
                else
                    this.SelectedItems.Clear();
            }

            base.OnMouseDown(e);
        }

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
    }
}
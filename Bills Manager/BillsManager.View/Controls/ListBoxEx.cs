using System.Windows.Controls;
using System.Windows;

namespace BillsManager.Views.Controls
{
    public sealed class ListBoxEx : ListBox
    {        
        public static readonly DependencyProperty ShowEmptyMessageDependencyProperty = DependencyProperty.Register("ShowEmptyMessage",
        typeof(bool),
        typeof(ListBoxEx),
        new PropertyMetadata(false));

        public bool ShowEmptyMessage
        {
            get
            {
                return (bool)this.GetValue(ShowEmptyMessageDependencyProperty);
            }
            set
            {
                this.SetValue(ShowEmptyMessageDependencyProperty, value);
            }
        }
        
        public static readonly DependencyProperty EmptyMessageDependencyProperty = DependencyProperty.Register("EmptyMessage",
        typeof(string),
        typeof(ListBoxEx),
        new PropertyMetadata("No items to display"));

        public string EmptyMessage
        {
            get
            {
                return (string)this.GetValue(EmptyMessageDependencyProperty);
            }
            set
            {
                this.SetValue(EmptyMessageDependencyProperty, value);
            }
        }
    }
}
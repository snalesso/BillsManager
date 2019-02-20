using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BillsManager.View.Controls
{
    public class WindowIcon : Image
    {
        public WindowIcon()
        {
            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                Binding b = new Binding();
                b.Source = parentWindow.Icon;
                b.Mode = BindingMode.OneWay;
                this.SetBinding(SourceProperty, b);
            }
        }
    }
}

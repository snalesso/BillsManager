using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BillsManager.View.Controls
{
    public class WindowTitleBar : TextBlock
    {
        public WindowTitleBar()
        {
            this.Text = "Title bar";

            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                Binding b = new Binding();
                b.Source = parentWindow.Title;
                b.Mode = BindingMode.OneWay;
                this.SetBinding(TextProperty, b);
            }
        }
    }
}

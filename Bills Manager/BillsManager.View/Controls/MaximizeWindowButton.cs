using System.Windows;
using System.Windows.Controls;

namespace BillsManager.View.Controls
{
    public class MaximizeWindowButton : Button
    {
        protected override void OnClick()
        {
            base.OnClick();

            Window.GetWindow(this).WindowState = WindowState.Maximized;

            //if (parentWindow.WindowState == WindowState.Normal)
                
            //else
            //    parentWindow.WindowState = WindowState.Normal;
        }
    }
}
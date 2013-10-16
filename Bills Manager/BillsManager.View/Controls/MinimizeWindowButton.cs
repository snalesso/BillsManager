using System.Windows;
using System.Windows.Controls;

namespace BillsManager.View.Controls
{
    public class MinimizeWindowButton : Button
    {
        protected override void OnClick()
        {
            base.OnClick();

            //Window.GetWindow(this).AllowsTransparency = false;
            //Window.GetWindow(this).WindowStyle = WindowStyle.SingleBorderWindow;
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }
    }
}

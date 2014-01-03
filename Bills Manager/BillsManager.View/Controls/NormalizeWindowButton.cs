using System.Windows;
using System.Windows.Controls;

namespace BillsManager.Views.Controls
{
    public class NormalizeWindowButton : Button
    {
        protected override void OnClick()
        {
            base.OnClick();

            Window.GetWindow(this).WindowState = WindowState.Normal;
        }
    }
}
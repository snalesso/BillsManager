using System.Windows;
using System.Windows.Controls;

namespace BillsManager.Views.Controls
{
    public class CloseWindowButton : Button
    {
        protected override void OnClick()
        {
            base.OnClick();

            Window.GetWindow(this).Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace BillsManager.View.Controls
{
    public class MinimizeWindowButton : Button
    {
        protected override void OnClick()
        {
            base.OnClick();

            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }
    }
}

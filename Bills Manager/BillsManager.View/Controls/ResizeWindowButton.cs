using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace BillsManager.View.Controls
{
    public class ResizeWindowButton : Button
    {
        protected override void OnClick()
        {
            base.OnClick();

            var parentWindow = Window.GetWindow(this);

            if (parentWindow.WindowState == WindowState.Normal)
                parentWindow.WindowState = WindowState.Maximized;
            else
                parentWindow.WindowState = WindowState.Normal;
        }
    }
}

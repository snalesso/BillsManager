using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace BillsManager.View.Behaviors
{
    public class WindowBehavior : DependencyObject
    {
        #region Windows API Imports

        #region constants
        private const int ICON_SMALL = 0;
        private const int ICON_BIG = 1;

        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint MF_ENABLED = 0x00000000;

        private const uint SC_CLOSE = 0xF060;

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int SWP_NOREPOSITION = 0x0200;

        private const int WS_SYSMENU = 0x80000;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WS_MAXIMIZEBOX = 0x1000;
        private const int WS_EX_DLGMODALFRAME = 0x0001;

        private const int GWL_EXSTYLE = -20;
        private const int GWL_STYLE = -16;

        private const int WM_SETICON = 0x0080;

        #endregion

        #region dll imports

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        #endregion

        #region sys calls

        private static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr64(hWnd, nIndex);
            else
                return GetWindowLongPtr32(hWnd, nIndex);
        }

        private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        #endregion

        #endregion

        #region IsCloseButtonEnabled attached property

        public static bool GetIsCloseButtonEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCloseButtonEnabledProperty);
        }

        public static void SetIsCloseButtonEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCloseButtonEnabledProperty, value);
        }

        public static readonly DependencyProperty IsCloseButtonEnabledProperty =
            DependencyProperty.RegisterAttached("IsCloseButtonEnabled", typeof(bool), typeof(WindowBehavior), new UIPropertyMetadata(true, OnIsCloseButtonEnabledChanged));

        private static void OnIsCloseButtonEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null)
                return;

            if (!(bool)e.NewValue)
                window.SourceInitialized += DisableCloseButton;

            // EnableMenuItem(hMenu, SC_CLOSE, 0x0000);
        }

        private static void DisableCloseButton(object sender, EventArgs e)
        {
            var window = sender as Window;

            IntPtr hMenu = GetSystemMenu(new WindowInteropHelper(window).Handle, false);
            if (hMenu != IntPtr.Zero)
            {
                EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }

            window.SourceInitialized -= DisableCloseButton;
        }

        #endregion

        #region minimize and maximize helpers

        private static void AddWindowStyle(Window window, int styleToAdd)
        {
            WindowInteropHelper wih = new WindowInteropHelper(window);
            int style = (int)GetWindowLongPtr(wih.EnsureHandle(), GWL_STYLE);
            style |= styleToAdd;
            SetWindowLongPtr(wih.Handle, GWL_STYLE, (IntPtr)style);
            SetWindowPos(wih.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOREPOSITION | SWP_NOREPOSITION | SWP_NOSIZE | SWP_NOZORDER);
        }

        private static void RemoveWindowStyle(Window window, int styleToRemove)
        {
            WindowInteropHelper wih = new WindowInteropHelper(window);
            int style = (int)GetWindowLongPtr(wih.EnsureHandle(), GWL_STYLE);
            style &= ~styleToRemove;
            SetWindowLongPtr(wih.Handle, GWL_STYLE, (IntPtr)style);
            SetWindowPos(wih.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOREPOSITION | SWP_NOREPOSITION | SWP_NOSIZE | SWP_NOZORDER);
        }

        #endregion

        #region CanMinimize attached property

        public static bool GetCanMinimize(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanMinimizeProperty);
        }

        public static void SetCanMinimize(DependencyObject obj, bool value)
        {
            obj.SetValue(CanMinimizeProperty, value);
        }

        public static readonly DependencyProperty CanMinimizeProperty =
            DependencyProperty.RegisterAttached("CanMinimize", typeof(bool), typeof(WindowBehavior), new UIPropertyMetadata(true, OnCanMinimizeChanged));

        private static void OnCanMinimizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null)
                return;

            if ((bool)e.NewValue)
                AddWindowStyle(window, WS_MINIMIZEBOX);
            else
                RemoveWindowStyle(window, WS_MINIMIZEBOX);
        }

        #endregion

        #region CanMaximize attached property

        public static bool GetCanMaximize(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanMaximizeProperty);
        }

        public static void SetCanMaximize(DependencyObject obj, bool value)
        {
            obj.SetValue(CanMaximizeProperty, value);
        }

        public static readonly DependencyProperty CanMaximizeProperty =
            DependencyProperty.RegisterAttached("CanMaximize", typeof(bool), typeof(WindowBehavior), new UIPropertyMetadata(true, OnCanMaximizeChanged));

        private static void OnCanMaximizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null)
                return;

            if ((bool)e.NewValue)
                AddWindowStyle(window, WS_MAXIMIZEBOX);
            else
                RemoveWindowStyle(window, WS_MAXIMIZEBOX);
        }

        #endregion

        #region HasSystemMenu attached property

        public static bool GetHasSystemMenu(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasSystemMenuProperty);
        }

        public static void SetHasSystemMenu(DependencyObject obj, bool value)
        {
            obj.SetValue(HasSystemMenuProperty, value);
        }

        public static readonly DependencyProperty HasSystemMenuProperty =
            DependencyProperty.RegisterAttached("HasSystemMenu", typeof(bool), typeof(WindowBehavior), new UIPropertyMetadata(true, OnHasSystemMenuChanged));

        private static void OnHasSystemMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null)
                return;

            if ((bool)e.NewValue)
                AddWindowStyle(window, WS_SYSMENU);
            else
                RemoveWindowStyle(window, WS_SYSMENU);
        }

        #endregion

        #region ShowIconAttachedProperty

        public static bool GetShowIcon(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowIconProperty);
        }

        public static void SetShowIcon(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowIconProperty, value);
        }

        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.RegisterAttached("ShowIcon", typeof(bool), typeof(WindowBehavior), new UIPropertyMetadata(true, OnShowIconChanged));

        private static void OnShowIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null)
                return;

            window.SourceInitialized += RemoveIcon;
        }

        private static void RemoveIcon(object sender, EventArgs e)
        {
            var window = sender as Window;

            IntPtr hWnd = new WindowInteropHelper(window).Handle;
            int currentStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, currentStyle | SWP_NOSIZE);
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);

            window.SourceInitialized -= RemoveIcon;
        }

        #endregion
    }
}
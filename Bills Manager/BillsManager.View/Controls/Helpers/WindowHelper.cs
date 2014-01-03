using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace BillsManager.Views.Controls
{
    public class WindowHelper
    {
        #region constants

        private static class WindowExStyles
        {
            public const int EXT_STYLE = -20;

            public const int WS_EX_ACCEPTFILES = 0x00000010;
            public const int WS_EX_APPWINDOW = 0x00040000;
            public const int WS_EX_CLIENTEDGE = 0x00000200;
            public const int WS_EX_COMPOSITED = 0x02000000;
            public const int WS_EX_CONTEXTHELP = 0x00000400;
            public const int WS_EX_CONTROLPARENT = 0x00010000;
            public const int WS_EX_DLGMODALFRAME = 0x00000001;
            public const int WS_EX_LAYERED = 0x00080000;
            public const int WS_EX_LAYOUTRTL = 0x00400000;
            public const int WS_EX_LEFT = 0x00000000;
            public const int WS_EX_LEFTSCROLLBAR = 0x00004000;
            public const int WS_EX_LTRREADING = 0x00000000;
            public const int WS_EX_MDICHILD = 0x00000040;
            public const int WS_EX_NOACTIVATE = 0x08000000;
            public const int WS_EX_NOINHERITLAYOUT = 0x00100000;
            public const int WS_EX_NOPARENTNOTIFY = 0x00000004;
            public const int WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE;
            public const int WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST;
            public const int WS_EX_RIGHT = 0x00001000;
            public const int WS_EX_RIGHTSCROLLBAR = 0x00000000;
            public const int WS_EX_RTLREADING = 0x00002000;
            public const int WS_EX_STATICEDGE = 0x00020000;
            public const int WS_EX_TOOLWINDOW = 0x00000080;
            public const int WS_EX_TOPMOST = 0x00000008;
            public const int WS_EX_TRANSPARENT = 0x00000020;
            public const int WS_EX_WINDOWEDGE = 0x00000100;
        }

        private static class WindowStyles
        {
            public const int STYLE = -16;

            public const int WS_BORDER = 0x00800000;
            public const int WS_CAPTION = 0x00C00000;
            public const int WS_CHILD = 0x40000000;
            public const int WS_CHILDWINDOW = 0x40000000;
            public const int WS_CLIPCHILDREN = 0x02000000;
            public const int WS_CLIPSIBLINGS = 0x04000000;
            public const int WS_DISABLED = 0x08000000;
            public const int WS_DLGFRAME = 0x00400000;
            public const int WS_GROUP = 0x00020000;
            public const int WS_HSCROLL = 0x00100000;
            public const int WS_ICONIC = 0x20000000;
            public const int WS_MAXIMIZE = 0x01000000;
            public const int WS_MAXIMIZEBOX = 0x00010000;
            public const int WS_MINIMIZE = 0x20000000;
            public const int WS_MINIMIZEBOX = 0x00020000;
            public const int WS_OVERLAPPED = 0x00000000;
            public const int WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
            public const int WS_POPUP = unchecked((int)0x80000000);
            public const int WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU;
            public const int WS_SIZEBOX = 0x00040000;
            public const int WS_SYSMENU = 0x00080000;
            public const int WS_TABSTOP = 0x00010000;
            public const int WS_THICKFRAME = 0x00040000;
            public const int WS_TILED = 0x00000000;
            public const int WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
            public const int WS_VISIBLE = 0x10000000;
            public const int WS_VSCROLL = 0x00200000;
        }

        private static class SystemCommands
        {
            public const uint SC_CLOSE = 0xF060;
            public const uint MF_BYCOMMAND = 0x00000000;
            public const uint MF_GRAYED = 0x00000001;
            public const uint MF_ENABLED = 0x00000000;
            public const uint MF_DISABLED = 0x00000002;
        }

        #endregion

        #region dll imports

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int index, int newLong);

        #endregion

        #region style helpers

        private static void UpdateStyle(Window window, int styleChange, bool remove = false)
        {
            if (window == null) throw new ArgumentNullException("window");

            IntPtr hWindow = new WindowInteropHelper(window).Handle;

            var style = GetWindowLong(hWindow, WindowStyles.STYLE);

            if (!remove)
                style |= styleChange;
            else
                style &= ~styleChange;

            SetWindowLong(hWindow, WindowStyles.STYLE, style);
        }

        private static void UpdateExtendedStyle(Window window, int exStyleChange, bool remove = false)
        {
            if (window == null) throw new ArgumentNullException("window");

            IntPtr hWindow = new WindowInteropHelper(window).Handle;

            var style = GetWindowLong(hWindow, WindowExStyles.EXT_STYLE);

            if (!remove)
                style |= exStyleChange;
            else
                style &= ~exStyleChange;

            SetWindowLong(hWindow, WindowExStyles.EXT_STYLE, style);
        }

        #endregion

        #region can close

        public static void SetCanClose(Window window, bool isEnabled)
        {
            IntPtr hWindow = new WindowInteropHelper(window).Handle;
            IntPtr hMenu = GetSystemMenu(hWindow, false);
            if (hMenu != IntPtr.Zero)
            {
                EnableMenuItem(hMenu, SystemCommands.SC_CLOSE, SystemCommands.MF_BYCOMMAND | (isEnabled ? SystemCommands.MF_ENABLED : SystemCommands.MF_GRAYED | SystemCommands.MF_DISABLED));
            }
            else
            {
                if (isEnabled)
                    window.SourceInitialized += EnableCanClose;
                else
                    window.SourceInitialized += DisableCanClose;
            }
        }

        private static void DisableCanClose(object sender, EventArgs e)
        {
            IntPtr hWindow = new WindowInteropHelper(sender as Window).Handle;
            IntPtr hMenu = GetSystemMenu(hWindow, false);
            EnableMenuItem(hMenu, SystemCommands.SC_CLOSE, SystemCommands.MF_BYCOMMAND | SystemCommands.MF_GRAYED | SystemCommands.MF_DISABLED);
        }

        private static void EnableCanClose(object sender, EventArgs e)
        {
            IntPtr hWindow = new WindowInteropHelper(sender as Window).Handle;
            IntPtr hMenu = GetSystemMenu(hWindow, false);
            EnableMenuItem(hMenu, SystemCommands.SC_CLOSE, SystemCommands.MF_BYCOMMAND | SystemCommands.MF_ENABLED);
        }

        #endregion

        #region can resize

        public static void SetCanMaximize(Window window, bool canResize)
        {
            UpdateStyle(window, WindowStyles.WS_MAXIMIZEBOX, canResize);
        }

        #endregion

        #region can minimize

        public static void SetCanMinimize(Window window, bool canMinimize)
        {
            UpdateStyle(window, WindowStyles.WS_MINIMIZEBOX, canMinimize);
        }

        #endregion

        #region use system menu

        public static void SetUseSystemMenu(Window window, bool useSystemMenu)
        {
            UpdateStyle(window, WindowStyles.WS_SYSMENU, useSystemMenu);
        }

        #endregion
    }
}
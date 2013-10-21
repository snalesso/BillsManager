using System.ComponentModel;
using System.Windows;
using System.Windows.Shell;

namespace BillsManager.View.Controls
{
    public class WindowEx : Window
    {
        #region properties

        #region CanClose

        [Browsable(true)]
        [Category("Common")]
        public bool CanClose
        {
            get { return (bool)this.GetValue(CanCloseAttachedProperty); }
            set { this.SetValue(CanCloseAttachedProperty, value); }
        }

        #region CanClose attached property

        protected static bool GetCanClose(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanCloseAttachedProperty);
        }

        protected static void SetCanClose(DependencyObject obj, bool value)
        {
            obj.SetValue(CanCloseAttachedProperty, value);
        }

        protected static readonly DependencyProperty CanCloseAttachedProperty =
            DependencyProperty.RegisterAttached(
            "CanCloseAttachedProperty",
            typeof(bool),
            typeof(WindowEx),
            new UIPropertyMetadata(true, OnCanCloseChanged));

        protected static void OnCanCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null)
                return;

            WindowHelper.SetCanClose(window, (bool)e.NewValue);
        }

        #endregion

        #endregion

        public new ResizeMode ResizeMode
        {
            get { return base.ResizeMode; }
            set
            {
                base.ResizeMode = value;

                switch (value)
                {
                    case System.Windows.ResizeMode.NoResize:
                    case System.Windows.ResizeMode.CanMinimize:
                            WindowHelper.SetCanMaximize(this, false);
                            break;
                }
            }
        }

        #region ResizeThickness

        [Browsable(true)]
        [Category("Common")]
        public Thickness ResizeThickness
        {
            get { return (Thickness)this.GetValue(ResizeThicknessAttachedProperty); }
            set { this.SetValue(ResizeThicknessAttachedProperty, value); }
        }

        #region ResizeThickness attached property

        protected static Thickness GetResizeThickness(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(ResizeThicknessAttachedProperty);
        }

        protected static void SetResizeThickness(DependencyObject obj, Thickness value)
        {
            obj.SetValue(ResizeThicknessAttachedProperty, value);
        }

        protected static readonly DependencyProperty ResizeThicknessAttachedProperty =
            DependencyProperty.RegisterAttached(
            "ResizeThicknessAttachedProperty",
            typeof(Thickness),
            typeof(WindowEx),
            new UIPropertyMetadata(new Thickness(7), UpdateResizeDirections));

        #endregion

        #endregion

        #region ResizeDirections

        [Browsable(true)]
        [Category("Common")]
        public Thickness ResizeDirections
        {
            get { return (Thickness)this.GetValue(ResizeDirectionsAttachedProperty); }
            set { this.SetValue(ResizeDirectionsAttachedProperty, value); }
        }

        #region ResizeDirections attached property

        protected static Thickness GetResizeDirections(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(ResizeDirectionsAttachedProperty);
        }

        protected static void SetResizeDirections(DependencyObject obj, Thickness value)
        {
            obj.SetValue(ResizeDirectionsAttachedProperty, value);
        }

        protected static readonly DependencyProperty ResizeDirectionsAttachedProperty =
            DependencyProperty.RegisterAttached(
            "ResizeDirectionsAttachedProperty",
            typeof(Thickness),
            typeof(WindowEx),
            new UIPropertyMetadata(new Thickness(1), UpdateResizeDirections));

        protected static void UpdateResizeDirections(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as WindowEx;
            if (window == null)
                return;

            var rbt = new Thickness();

            rbt.Left = window.ResizeDirections.Left * window.ResizeThickness.Left;
            rbt.Top = window.ResizeDirections.Top * window.ResizeThickness.Top;
            rbt.Right = window.ResizeDirections.Right * window.ResizeThickness.Right;
            rbt.Bottom = window.ResizeDirections.Bottom * window.ResizeThickness.Bottom;

            WindowChrome.GetWindowChrome(window).ResizeBorderThickness = rbt;
        }

        #endregion

        #endregion

        #endregion
    }
}
using System.Windows;
using System.Windows.Controls;

namespace BillsManager.View.Controls
{
    /// <summary>
    /// Logica di interazione per ControlBox.xaml
    /// </summary>
    public partial class ControlBox : UserControl
    {
        #region fields

        Window parentWindow;

        #endregion

        #region ctor

        public ControlBox()
        {
            InitializeComponent();

            this.parentWindow = Window.GetWindow(this);
        }

        #endregion

        #region properties

        public bool CanClose
        {
            get { return this.btnClose.Visibility == System.Windows.Visibility.Visible; }
            set
            {
                if (this.CanClose != value)
                {
                    this.btnClose.IsEnabled = value;
                    WindowHelper.SetCanClose(this.parentWindow, value);
                }
            }
        }

        public bool CanResize
        {
            get { return this.btnResize.Visibility == System.Windows.Visibility.Visible; }
            set
            {
                if (this.CanResize != value)
                {
                    if (value)
                        this.btnResize.Visibility = System.Windows.Visibility.Visible;
                    else
                        this.btnResize.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        public bool CanMinimize
        {
            get { return this.btnMinimize.Visibility == System.Windows.Visibility.Visible; }
            set
            {
                if (this.CanMinimize != value)
                {
                    if (value)
                        this.btnMinimize.Visibility = System.Windows.Visibility.Visible;
                    else
                        this.btnMinimize.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        #endregion
    }
}
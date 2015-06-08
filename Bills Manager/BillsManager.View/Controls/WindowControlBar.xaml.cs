using System.Windows.Controls;

namespace BillsManager.View.Controls
{
    /// <summary>
    /// Logica di interazione per WindowControlBar.xaml
    /// </summary>
    public partial class WindowControlBar : UserControl
    {
        #region ctor

        public WindowControlBar()
        {
            InitializeComponent();
        }

        #endregion

        #region properties

        public string Title
        {
            get { return this.tlbTitle.Text; }
            set
            {
                this.tlbTitle.Text = value;
            }
        }

        #endregion
    }
}

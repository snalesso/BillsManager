using System.Windows.Controls;

namespace BillsManager.Views
{
    // TODO: combobox drop down border needs to be not rounded on top, and cbx botcorn when radius != 0
    // TODO: localize validation error popups
    /// <summary>
    /// Logica di interazione per BillView.xaml
    /// </summary>
    public partial class BillAddEditView : UserControl
    {
        // URGENT: update datepicker style
        public BillAddEditView()
        {
            InitializeComponent();
        }

        private void SelectAllText(object sender, System.Windows.RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt != null)
                txt.SelectAll();
        }

        private void SelectAllText(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt != null)
                txt.SelectAll();
        }
    }
}
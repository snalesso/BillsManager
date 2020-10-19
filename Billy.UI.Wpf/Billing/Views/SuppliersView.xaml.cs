using Billy.Billing.ViewModels;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Billy.Billing.Views
{
    /// <summary>
    /// Interaction logic for SuppliersView.xaml
    /// </summary>
    public partial class SuppliersView : UserControl
    {
        public SuppliersView()
        {
            this.InitializeComponent();
        }

        private async void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left
                //&& e.ClickCount == 2
                && this.DataContext is SuppliersViewModel suppliersViewModel
                && sender is FrameworkElement fe
                && fe.DataContext is SupplierViewModel supplierViewModel)
            {
                await suppliersViewModel.ShowEditSupplierView.Execute(supplierViewModel);
            }
        }
    }
}

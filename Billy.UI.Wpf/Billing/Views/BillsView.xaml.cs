using Billy.Billing.ViewModels;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Billy.Billing.Views
{
    /// <summary>
    /// Interaction logic for BillsView.xaml
    /// </summary>
    public partial class BillsView : UserControl
    {
        public BillsView()
        {
            this.InitializeComponent();
        }

        private async void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left
                //&& e.ClickCount == 2
                && this.DataContext is BillsViewModel billsViewModel
                && sender is FrameworkElement fe
                && fe.DataContext is BillViewModel billViewModel)
            {
                await billsViewModel.ShowEditBillView.Execute(billViewModel);
            }
        }
    }
}

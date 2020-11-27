using Billy.Billing.ViewModels;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System;

namespace Billy.Billing.Views
{
    public class SuppliersViewUserControl : ReactiveUserControl<SuppliersViewModel> { }

    /// <summary>
    /// Interaction logic for SuppliersView.xaml
    /// </summary>
    public partial class SuppliersView : SuppliersViewUserControl
    {
        public SuppliersView()
        {
            this.InitializeComponent();

            //this.WhenActivated(disposables =>
            //{
            //    this.WhenAnyValue(x => x.ViewModel).InvokeCommand(this, x => x.ViewModel.LoadSuppliers).DisposeWith(disposables);
            //});
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

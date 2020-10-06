using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace Billy.UI.Wpf.Presentation.Billing
{
    /// <summary>
    /// Interaction logic for EditSupplierView.xaml
    /// </summary>
    public partial class EditSupplierView : UserControl//,IViewFor<AddSupplierViewModel>, IViewFor<EditSupplierViewModel>
    {
        public EditSupplierView()
        {
            this.InitializeComponent();
        }

        //public EditSupplierViewModel ViewModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //object IViewFor.ViewModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //AddSupplierViewModel IViewFor<AddSupplierViewModel>.ViewModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

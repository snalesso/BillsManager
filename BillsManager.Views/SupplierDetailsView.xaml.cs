using System.Windows.Controls;

/* TODO: when an enum needs to be localized for the view, 
 * there are 2 ways to localize it, in the VM through a [propertyName]String property
 * or through a datatrigger, which is the best way? */

// TODO: put ConverterCulture parameter in every Databinding which binds to a Date type property
namespace BillsManager.Views
{
    /// <summary>
    /// Interaction logic for SupplierDetailsView.xaml
    /// </summary>
    public partial class SupplierDetailsView : UserControl
    {
        public SupplierDetailsView()
        {
            InitializeComponent();
        }
    }
}
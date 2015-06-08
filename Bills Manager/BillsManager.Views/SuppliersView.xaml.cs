using System.Windows.Controls;

namespace BillsManager.Views
{
    /// <summary>
    /// Logica di interazione per SuppliersView.xaml
    /// </summary>
    public partial class SuppliersView : UserControl
    {
        //GridView gridView;

        public SuppliersView()
        {
            InitializeComponent();

            //this.gridView = this.lsvSuppliers.View as GridView;
        }

        //private void UpdateColumnWidths(object sender, ScrollChangedEventArgs e)
        //{
        //    if (e.VerticalChange != 0)
        //    {
        //        foreach (var column in this.gridView.Columns)
        //        {
        //            column.Width = 0;
        //            column.Width = double.NaN;
        //        }
        //    }
        //}
    }
}
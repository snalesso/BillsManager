using System.Windows.Controls;
//using BillsManager.Views.Controls.ExtensionMethods;

namespace BillsManager.Views
{
    /// <summary>
    /// Logica di interazione per BillsView.xaml
    /// </summary>
    public partial class BillsView : UserControl
    {
        //GridView gridView;

        public BillsView()
        {
            InitializeComponent();

            //this.gridView = this.lsvBills.View as GridView;
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
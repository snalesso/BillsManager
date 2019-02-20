using System.Linq;
using BillsManager.Services.Data;
using System.Collections.ObjectModel;
using BillsManager.ViewModels;

namespace BillsManager.DesignTime.ViewModels
{
    public sealed partial class DesignTimeBillsViewModel : BillsViewModel
    {
        #region ctor

        public DesignTimeBillsViewModel()
        {
            var mbp = new MockedDBConnector(2, 3);

            var bills = mbp.GetAllBills();

            var billViewModels = bills.Select(b => new DesignTimeBillDetailsViewModel(b));

            this.BillViewModels = new ObservableCollection<BillDetailsViewModel>(billViewModels);
        }

        #endregion

        #region methods

        //private void LoadDesignTimeData()
        //{
        //    Supplier supp = new Supplier(
        //        0,
        //        "Faber-Castell",
        //        "Via Stromboli",
        //        "14",
        //        "Milano",
        //        "20144",
        //        "MI",
        //        "Italia",
        //        "faber-castell@faber-castell.it",
        //        "http://www.faber-castell.it",
        //        "02/43069601",
        //        "02/43069601",
        //        "sconti 10/06 - 24/09.",
        //        "Barbara",
        //        "Robecchi",
        //        "347-7892234");

        //    Bill newBill = new Bill(
        //        0,
        //        supp.ID,
        //        DateTime.Today,
        //        DateTime.Today.AddDays(14),
        //        DateTime.Today.AddDays(-2),
        //        DateTime.Today,
        //        723.61,
        //        0,
        //        21.3,
        //        "X3V-KDM",
        //        "call agent for reduction");

        //    Bill newBill2 = new Bill(
        //        1,
        //        supp.ID,
        //        DateTime.Today.AddDays(-30),
        //        DateTime.Today.AddDays(-20),
        //        DateTime.Today.AddDays(-32),
        //        null,
        //        54.06,
        //        0,
        //        0.5,
        //        "DK23595",
        //        "ask for catalog");

        //    Bill newBill3 = new Bill(
        //        2,
        //        supp.ID,
        //        DateTime.Today,
        //        DateTime.Today.AddDays(7),
        //        DateTime.Today.AddDays(-3),
        //        null,
        //        54.06,
        //        9,
        //        0,
        //        "AZ381EY",
        //        "call agent for reductions and new winter orders");

        //    Bill newBill4 = new Bill(
        //        3,
        //        supp.ID,
        //        DateTime.Today.AddDays(-30),
        //        DateTime.Today.AddDays(-1),
        //        DateTime.Today.AddDays(-32),
        //        null,
        //        54.06,
        //        15.7,
        //        0,
        //        "DK23595",
        //        "ask for catalog");

        //    var bills = new List<BillDetailsViewModel>();

        //    //bills.Add(new BillDetailsViewModel(newBill) /*{ SupplierName = "faewf" }*/);
        //    //bills.Add(new BillDetailsViewModel(newBill2) /*{ SupplierName = "fawef" }*/);
        //    //bills.Add(new BillDetailsViewModel(newBill3) /*{ SupplierName = "gres" }*/);
        //    //bills.Add(new BillDetailsViewModel(newBill4) /*{ SupplierName = "erg" }*/);

        //    this.BillViewModels = new ObservableCollection<BillDetailsViewModel>(bills);

        //    this.SelectedBillViewModel = this.FilteredBillViewModels.FirstOrDefault();
        //}

        #endregion
    }
}
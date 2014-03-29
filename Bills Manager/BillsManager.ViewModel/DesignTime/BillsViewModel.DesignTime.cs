using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BillsManager.Models;
using Caliburn.Micro;
using System.Linq;

namespace BillsManager.ViewModels
{
#if DEBUG
    public partial class BillsViewModel
    {
        #region ctor

        public BillsViewModel()
        {
            if (Execute.InDesignMode)
                this.LoadDesignTimeData();
        }

        #endregion

        #region methods

        private void LoadDesignTimeData()
        {
            Supplier supp = new Supplier(
                0,
                "Faber-Castell",
                "Via Stromboli",
                "14",
                "Milano",
                "20144",
                "MI",
                "Italia",
                "faber-castell@faber-castell.it",
                "http://www.faber-castell.it",
                "02/43069601",
                "02/43069601",
                "sconti 10/06 - 24/09.",
                "Barbara",
                "Robecchi",
                "347-7892234");

            Bill newBill = new Bill(
                0,
                //21,
                DateTime.Today,
                DateTime.Today.AddDays(14),
                DateTime.Today,
                DateTime.Today.AddDays(-2),
                723.61,
                /*0,
                21.3,*/
                supp.ID,
                "call agent for reduction",
                "X3V-KDM");

            Bill newBill2 = new Bill(
                1,
                //64,
                DateTime.Today.AddDays(-30),
                DateTime.Today.AddDays(-20),
                null,
                DateTime.Today.AddDays(-32),
                54.06,
                /*0,
                0.5,*/
                supp.ID,
                "ask for catalog",
                "DK23595");

            Bill newBill3 = new Bill(
                2,
                //3,
                DateTime.Today,
                DateTime.Today.AddDays(7),
                null,
                DateTime.Today.AddDays(-3),
                54.06,
                /*9,
                0,*/
                supp.ID,
                "call agent for reductions and new winter orders",
                "AZ381EY");

            Bill newBill4 = new Bill(
                3,
                //22,
                DateTime.Today.AddDays(-30),
                DateTime.Today.AddDays(-1),
                null,
                DateTime.Today.AddDays(-32),
                54.06,
                /*15.7,
                 0,*/
                supp.ID,
                "ask for catalog",
                "DK23595");

            var bills = new List<BillDetailsViewModel>();

            bills.Add(new BillDetailsViewModel(newBill) /*{ SupplierName = "faewf" }*/);
            bills.Add(new BillDetailsViewModel(newBill2) /*{ SupplierName = "fawef" }*/);
            bills.Add(new BillDetailsViewModel(newBill3) /*{ SupplierName = "gres" }*/);
            bills.Add(new BillDetailsViewModel(newBill4) /*{ SupplierName = "erg" }*/);

            this.BillViewModels = new ObservableCollection<BillDetailsViewModel>(bills);

            this.SelectedBillViewModel = this.BillViewModels.FirstOrDefault();
        }

        #endregion
    }
#endif
}
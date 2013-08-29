using System;
using BillsManager.Model;
using Caliburn.Micro;
using BillsManager.ViewModel.Messages;

namespace BillsManager.ViewModel
{
    public partial class BillsViewModel : Screen
    {
        #region ctor

        public BillsViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.LoadDesignTimeData();
            }
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
                20144,
                "MI",
                "Italia",
                "faber-castell@faber-castell.it",
                "http://www.faber-castell.it",
                "02/43069601",
                "sconti 10/06 - 24/09."
            //,new[] { new Agent("Barbara", "Robecchi", "347-7892234", string.Empty) }
            );

            Bill newBill = new Bill(
                0,
                DateTime.Today,
                DateTime.Today.AddDays(14),
                DateTime.Today,
                DateTime.Today.AddDays(-2),
                723.61,
                supp.Name,
                "call agent for reduction",
                "X3V-KDM");

            Bill newBill2 = new Bill(
                1,
                DateTime.Today.AddDays(-30),
                DateTime.Today.AddDays(-20),
                null,
                DateTime.Today.AddDays(-32),
                54.06,
                supp.Name,
                "ask for catalog",
                "DK23595");

            Bill newBill3 = new Bill(
                2,
                DateTime.Today,
                DateTime.Today.AddDays(7),
                null,
                DateTime.Today.AddDays(-3),
                54.06,
                supp.Name,
                "call agent for reductions and new winter orders",
                "AZ381EY");

            Bill newBill4 = new Bill(
                3,
                DateTime.Today.AddDays(-30),
                DateTime.Today.AddDays(-1),
                null,
                DateTime.Today.AddDays(-32),
                54.06,
                supp.Name,
                "ask for catalog",
                "DK23595");

            this.BillViewModels.Add(new BillViewModel(newBill));
            this.BillViewModels.Add(new BillViewModel(newBill2));
            this.BillViewModels.Add(new BillViewModel(newBill3));
            this.BillViewModels.Add(new BillViewModel(newBill4));
        }

        #endregion
    }
}

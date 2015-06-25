using BillsManager.Models;
using BillsManager.ViewModels;
using Caliburn.Micro;
using System;

namespace BillsManager.DesignTime.ViewModels
{
    public sealed partial class DesignTimeBillDetailsViewModel : BillDetailsViewModel
    {
        #region field

        Supplier supplier;
        Bill bill;

        #endregion

        #region ctor

        public DesignTimeBillDetailsViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.LoadDesignTimeData();
            }
        }

        #endregion

        #region methods

        void LoadDesignTimeData()
        {
            this.supplier = new Supplier(
                   53,
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
                   "347-7892234"
               );

            this.bill = new Bill(
                0,
                this.supplier.ID,
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(14),
                DateTime.Today.AddDays(-8),
                DateTime.Today,
                723.61,
                66,
                0,
                "X3V-KDM",
                "call agent for reduction @additional comments to trigger validation rule");

            this.ExposedBill = this.bill;
        }

        public new string SupplierName
        {
            get { return this.supplier.Name; }
        }

        #endregion
    }
}
using BillsManager.Models;
using Caliburn.Micro;
using System;
using System.Linq;

namespace BillsManager.ViewModels
{
#if DEBUG
    public partial class BillAddEditViewModel
    {
        #region ctor

        public BillAddEditViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.LoadDesignTimeData();
            }
        }

        public BillAddEditViewModel(Bill exposedBill)
        {
            if (Execute.InDesignMode)
            {
                this.ExposedBill = exposedBill;
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
                "20144",
                "MI",
                "Italia",
                "faber-castell@faber-castell.it",
                "http://www.faber-castell.it",
                "02/43069601",
                "02/26367774",
                "sconti 10/06 - 24/09.",
                "Barbara",
                "Robecchi",
                "347-7892234");

            this.ExposedBill = new Bill(
                0,
                supp.ID,
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(14),
                DateTime.Today.AddDays(-8),
                DateTime.Today,
                723.61,
                32,
                0,
                null,
                "call agent for reduction @additional comments to trigger validation rule");

            this.AvailableSuppliers = new[] { supp };
            this.SelectedSupplier = this.AvailableSuppliers.FirstOrDefault();
        }

        #endregion
    }
#endif
}
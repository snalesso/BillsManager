using System;
using System.Linq;
using BillsManager.Model;
using Caliburn.Micro;

namespace BillsManager.ViewModel
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
                20144,
                "MI",
                "Italia",
                "faber-castell@faber-castell.it",
                "http://www.faber-castell.it",
                "02/43069601",
                "sconti 10/06 - 24/09."
                //,new[] { new Agent("Barbara", "Robecchi", "347-7892234", string.Empty) }
            );

            this.ExposedBill = new Bill(
                0,
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(14),
                DateTime.Today,
                DateTime.Today.AddDays(-8),
                723.61,
                supp.ID,
                "call agent for reduction @additional comments to trigger validation rule",
                "X3V-KDM");

            this.AvailableSuppliers = new[] { supp };
            this.SelectedSupplier = this.AvailableSuppliers.FirstOrDefault();
        }

        #endregion
    } 
#endif
}
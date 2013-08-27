using System;
using System.ComponentModel;
using BillsManager.Model;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class SupplierViewModel : Screen, IEditableObject
    {
        #region ctor

        public SupplierViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.LoadDesignTimeData();
            }
        }

        public SupplierViewModel(Supplier exposedSupplier)
        {
            if (Execute.InDesignMode)
            {
                this.ExposedSupplier = exposedSupplier;
            }
        }

        #endregion

        #region methods

        private void LoadDesignTimeData()
        {
            this.ExposedSupplier = new Supplier(
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

            this.ObligationAmount = -1936.27;

            //this.ExposedSupplier.Bills.Add(new Bill(
            //    0,
            //    DateTime.Today.AddDays(-2),
            //    DateTime.Today.AddDays(14),
            //    DateTime.Today,
            //    DateTime.Today.AddDays(-8),
            //    723.61,
            //    this.ExposedSupplier.Name,
            //    "call agent for reduction",
            //    "X3V-KDM"));
        }

        #endregion
    }
}

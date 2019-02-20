using BillsManager.Models;
using BillsManager.ViewModels;
using Caliburn.Micro;

namespace BillsManager.DesignTime.ViewModels
{
    public sealed partial class DesignTimeSupplierDetailsViewModel : SupplierDetailsViewModel
    {
        #region field

        Supplier supplier;

        #endregion

        #region ctor

        public DesignTimeSupplierDetailsViewModel()
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
            this.supplier = new Supplier(
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

            this.ExposedSupplier = this.supplier;

            this.ObligationAmount = -1936.27;
        }

        #endregion
    }
}
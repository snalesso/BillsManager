using BillsManager.Models;
using Caliburn.Micro;
using System;
using System.Linq;

namespace BillsManager.ViewModels
{
#if DEBUG
    public partial class SearchBillsViewModel : Screen
    {
        #region ctor

        public SearchBillsViewModel()
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
            var supps = new[]
            {
                new Supplier(
                    538,
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
                    "347-7892234")
            };

            this.AvailableSuppliers = supps;
            this.SelectedSupplier = this.AvailableSuppliers.FirstOrDefault();

            this.UseDueDateFilter = true;
            this.UseReleaseDateFilter = true;
            this.UseSupplierFilter = true;

            this.DueDateFilterValue = DateTime.Today.AddDays(15);
            this.ReleaseDateFilterValue = DateTime.Today.AddDays(-22);

            this.IsPaidFilterValue = null;
        }

        #endregion
    }
#endif
}
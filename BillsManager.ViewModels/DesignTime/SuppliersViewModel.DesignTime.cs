using System.Collections.ObjectModel;
using System.Linq;
using BillsManager.Models;
using Caliburn.Micro;

namespace BillsManager.ViewModels
{
#if DEBUG
    public partial class SuppliersViewModel : Screen
    {
        #region ctor

        public SuppliersViewModel()
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
            //var svm1 = new SupplierDetailsViewModel(new Supplier(
            //    0,
            //    "Faber-Castell",
            //    "Via Stromboli",
            //    "14",
            //    "Milano",
            //    "20144",
            //    "MI",
            //    "Italia",
            //    "faber-castell@faber-castell.it",
            //    "http://www.faber-castell.it",
            //    "02/43069601",
            //    "02/43069601",
            //    "sconti 10/06 - 24/09.",
            //    "Barbara", 
            //"Robecchi", 
            //"347-7892234")) { ObligationAmount = -1 };

            //svm1.ExposedSupplier.Bills.Add(new Bill(
            //    DateTime.Today.AddDays(-2),
            //    DateTime.Today.AddDays(14),
            //    DateTime.Today,
            //    DateTime.Today.AddDays(-8),
            //    723.61,
            //    svm1.ExposedSupplier,
            //    "call agent for reduction",
            //    "X3V-KDM"));

            //var svm2 = new SupplierDetailsViewModel(new Supplier(
            //    1,
            //    "Faber-Castell",
            //    "Via Stromboli",
            //    "14",
            //    "Milano",
            //    "20144",
            //    "MI",
            //    "Italia",
            //    "faber-castell@faber-castell.it",
            //    "http://www.faber-castell.it",
            //    "02/43069601",
            //    "02/26367774",
            //    "sconti 10/06 - 24/09.",
            //    "Barbara",
            //    "Robecchi",
            //    "347-7892234"));

            //var svm3 = new SupplierDetailsViewModel(new Supplier(
            //    2,
            //    "Faber-Castell",
            //    "Via Stromboli",
            //    "14",
            //    "Milano",
            //    "20144",
            //    "MI",
            //    "Italia",
            //    "faber-castell@faber-castell.it",
            //    "http://www.faber-castell.it",
            //    "02/43069601",
            //    "02/26367774",
            //    "sconti 10/06 - 24/09.",
            //    "Barbara",
            //    "Robecchi",
            //    "347-7892234")) { ObligationAmount = 1 }; // TODO: change obligation set way, add bills instead

            //svm3.ExposedSupplier.Bills.Add(new Bill(
            //    DateTime.Today.AddDays(-2),
            //    DateTime.Today.AddDays(14),
            //    DateTime.Today,
            //    DateTime.Today.AddDays(-8),
            //    -48.35,
            //    svm1.ExposedSupplier,
            //    "for changed order",
            //    "AB 325 MY"));

            this.SupplierViewModels = new ObservableCollection<SupplierDetailsViewModel>();
            //this.SupplierViewModels.Add(svm1);
            //this.SupplierViewModels.Add(svm2);
            //this.SupplierViewModels.Add(svm3);

            this.SelectedSupplierViewModel = this.SupplierViewModels.FirstOrDefault();
        }

        #endregion
    }
#endif
}
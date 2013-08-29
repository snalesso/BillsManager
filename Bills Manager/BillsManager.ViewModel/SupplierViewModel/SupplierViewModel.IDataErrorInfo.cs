using System.ComponentModel;
using BillsManager.ViewModel.Validation;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class SupplierViewModel : Screen, IDataErrorInfo
    {
        #region IDataErrorInfo

        private ValidationRulesTracker<SupplierViewModel> rulesTracker;

        // TODO: a simpler way to notify IsValid might be to check if the notifying property has a validation rule
        public bool IsValid
        {
            get { return string.IsNullOrEmpty(this.Error); }
        }

        public string Error
        {
            get { return this.rulesTracker.GetAllErrors(); }
        }

        public string this[string columnName]
        {
            get
            {
                if (this.rulesTracker == null)
                    this.rulesTracker = new ValidationRulesTracker<SupplierViewModel>(this);

                return this.rulesTracker.GetErrorsForProperty(columnName);
            }
        }

        #endregion
    }
}

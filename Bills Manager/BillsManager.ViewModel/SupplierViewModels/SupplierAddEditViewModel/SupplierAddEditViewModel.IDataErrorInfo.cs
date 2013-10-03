using System.ComponentModel;
using BillsManager.ViewModel.Validation;

namespace BillsManager.ViewModel
{
    public partial class SupplierAddEditViewModel : IDataErrorInfo
    {
        #region IDataErrorInfo

        private ValidationRulesTracker<SupplierAddEditViewModel> rulesTracker;

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
                // TODO: move initialization to ctor?
                if (this.rulesTracker == null)
                    this.rulesTracker = new ValidationRulesTracker<SupplierAddEditViewModel>(this);

                return this.rulesTracker.GetErrorsForProperty(columnName);
            }
        }

        #endregion
    }
}
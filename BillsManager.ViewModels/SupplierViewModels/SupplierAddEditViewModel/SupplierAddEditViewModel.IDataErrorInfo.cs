using System.ComponentModel;
using BillsManager.ViewModels.Validation;
using System;

namespace BillsManager.ViewModels
{
    public partial class SupplierAddEditViewModel : IDataErrorInfo
    {
        #region IDataErrorInfo

        private readonly ValidationRulesTracker<SupplierAddEditViewModel> rulesTracker;

        // TODO: a simpler way to notify IsValid might be to check if the notifying property has a validation rule
        public bool IsValid
        {
            get { return this.HasChanges & string.IsNullOrEmpty(this.Error); }
        }

        public string Error
        {
            get { return string.Join(Environment.NewLine, this.rulesTracker.GetAllErrors()); }
        }

        public string this[string columnName]
        {
            get
            {
                //if (this.rulesTracker == null)
                //    this.rulesTracker = new ValidationRulesTracker<SupplierAddEditViewModel>(this);

                var errors = this.rulesTracker.GetErrorsForProperty(columnName);
                return string.Join(Environment.NewLine, errors);
            }
        }

        #endregion
    }
}
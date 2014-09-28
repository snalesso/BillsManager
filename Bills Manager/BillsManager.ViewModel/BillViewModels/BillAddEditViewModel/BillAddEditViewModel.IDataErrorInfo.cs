using System.ComponentModel;
using BillsManager.ViewModels.Validation;

namespace BillsManager.ViewModels
{
    public partial class BillAddEditViewModel : IDataErrorInfo
    {
        #region IDataErrorInfo

        private ValidationRulesTracker<BillAddEditViewModel> rulesTracker;

        public bool IsValid // TODO: make cached?
        {
            get { return this.HasChanges & string.IsNullOrEmpty(this.Error); }
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
                    this.rulesTracker = new ValidationRulesTracker<BillAddEditViewModel>(this);

                var errors = this.rulesTracker.GetErrorsForProperty(columnName);
                return errors;
            }
        }

        #endregion
    }
}
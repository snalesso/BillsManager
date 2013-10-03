using System.ComponentModel;
using BillsManager.ViewModel.Validation;

namespace BillsManager.ViewModel
{
    public partial class BillAddEditViewModel : IDataErrorInfo
    {
        #region IDataErrorInfo

        private ValidationRulesTracker<BillAddEditViewModel> rulesTracker;

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
                    this.rulesTracker = new ValidationRulesTracker<BillAddEditViewModel>(this);

                return this.rulesTracker.GetErrorsForProperty(columnName);
            }
        }

        #endregion
    }
}
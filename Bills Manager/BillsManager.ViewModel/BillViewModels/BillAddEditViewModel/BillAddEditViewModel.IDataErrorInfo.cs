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
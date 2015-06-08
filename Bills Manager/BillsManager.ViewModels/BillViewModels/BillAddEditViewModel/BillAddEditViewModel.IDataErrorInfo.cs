using System.ComponentModel;
using BillsManager.ViewModels.Validation;
using System;

namespace BillsManager.ViewModels
{
    public partial class BillAddEditViewModel : IDataErrorInfo
    {
        #region IDataErrorInfo

        private readonly ValidationRulesTracker<BillAddEditViewModel> rulesTracker;

        public bool IsValid // TODO: make cached?
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
                //    this.rulesTracker = new ValidationRulesTracker<BillAddEditViewModel>(this);

                var errors = this.rulesTracker.GetErrorsForProperty(columnName);
                return string.Join(Environment.NewLine, errors);
            }
        }

        #endregion
    }
}
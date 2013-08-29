using System.ComponentModel;
using BillsManager.ViewModel.Validation;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class BillViewModel : Screen, IDataErrorInfo
    {
        #region IDataErrorInfo

        private ValidationRulesTracker<BillViewModel> rulesTracker;

        public bool IsValid
        {
            get { return string.IsNullOrEmpty(this.Error); }
        }

        public string Error
        {
            get
            {
                return this.rulesTracker.GetAllErrors();
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (this.rulesTracker == null)
                    this.rulesTracker = new ValidationRulesTracker<BillViewModel>(this);

                return this.rulesTracker.GetErrorsForProperty(columnName);
            }
        }

        #endregion
    }
}

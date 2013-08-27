using Caliburn.Micro;
using System.Linq;

namespace BillsManager.ViewModel
{
    public partial class SearchSuppliersViewModel
    {
        #region ctor

        public SearchSuppliersViewModel()
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
            this.UseObligationStateFilter = true;
            this.ObligationStateFilterValue = this.ObligationStates.FirstOrDefault();
        }

        #endregion
    }
}

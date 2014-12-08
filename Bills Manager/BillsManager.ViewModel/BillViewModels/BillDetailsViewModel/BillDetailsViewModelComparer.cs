using System.Collections.Generic;

namespace BillsManager.ViewModels
{
    public partial class BillDetailsViewModelComparer : IComparer<BillDetailsViewModel>
    {
        #region IComparer<BillDetailsViewModel> Members

        public int Compare(BillDetailsViewModel x, BillDetailsViewModel y)
        {
            return x.CompareTo(y);
        }

        #endregion

        public BillDetailsViewModelComparer() { }
    }
}
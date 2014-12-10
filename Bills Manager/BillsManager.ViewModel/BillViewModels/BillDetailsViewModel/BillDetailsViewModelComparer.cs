using System.Collections.Generic;

namespace BillsManager.ViewModels
{
    public partial class BillDetailsViewModelComparer : IComparer<BillDetailsViewModel>
    {
        #region IComparer<BillDetailsViewModel> Members

        public int Compare(BillDetailsViewModel x, BillDetailsViewModel y)
        {
            var change = x.CompareTo(y);

            return change;
        }

        #endregion

        public BillDetailsViewModelComparer() { }
    }
}
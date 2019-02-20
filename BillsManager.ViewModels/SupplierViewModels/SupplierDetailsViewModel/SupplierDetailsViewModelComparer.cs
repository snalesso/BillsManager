using System.Collections.Generic;

namespace BillsManager.ViewModels
{
    public class SupplierDetailsViewModelComparer : IComparer<SupplierDetailsViewModel>
    {
        #region IComparer<BillDetailsViewModel> Members

        public int Compare(SupplierDetailsViewModel x, SupplierDetailsViewModel y)
        {
            var change = x.CompareTo(y);

            return change;
        }

        #endregion

        public SupplierDetailsViewModelComparer() { }
    }
}
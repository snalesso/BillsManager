using System;

namespace BillsManager.ViewModels
{
    public partial class SupplierDetailsViewModel : IComparable<SupplierDetailsViewModel>
    {
        #region IComparable<SupplierDetailsViewModel> Members

        public int CompareTo(SupplierDetailsViewModel other)
        {
            return this.Name.CompareTo(other.Name);
        }

        #endregion
    }
}
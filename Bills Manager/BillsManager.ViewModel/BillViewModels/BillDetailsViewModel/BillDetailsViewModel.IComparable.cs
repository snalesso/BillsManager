using System;

namespace BillsManager.ViewModels
{
    public partial class BillDetailsViewModel : IComparable<BillDetailsViewModel>
    {
        #region IComparable<BillDetailsViewModel> Members

        public int CompareTo(BillDetailsViewModel other)
        {
            var paid = this.IsPaid.CompareTo(other.IsPaid);
            if (paid != 0) return paid;

            var due = other.DueDate.CompareTo(this.DueDate);
            if (due != 0) return due * (-1);

            var amount = this.Amount.CompareTo(other.Amount);
            if (amount != 0) return amount;

            var supplierName = this.SupplierName.CompareTo(other.SupplierName);
            if (supplierName != 0) return supplierName;

            var code = this.Code.CompareTo(other.Code);
            if (code != 0) return code;

            return 1;
        }

        #endregion
    }
}
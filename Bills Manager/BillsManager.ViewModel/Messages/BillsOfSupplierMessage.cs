using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class BillsOfSupplierMessage
    {
        public BillsOfSupplierMessage(
            string supplierName,
            IEnumerable<Bill> bills)
        {
            this.supplierName = supplierName;
            this.bills = bills;
        }

        private string supplierName;
        public string SupplierName
        {
            get { return supplierName; }
        }

        private IEnumerable<Bill> bills;
        public IEnumerable<Bill> Bills
        {
            get { return bills; }
        }
    }
}

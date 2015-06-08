using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class AddBillToSupplierOrder
    {
        public AddBillToSupplierOrder(Supplier supplier)
        {
            this.supplier = supplier;
        }

        private readonly Supplier supplier;
        public Supplier Supplier
        {
            get { return this.supplier; }
        }
    }
}
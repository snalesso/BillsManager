using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class EditSupplierRequestMessage
    {
        public EditSupplierRequestMessage(Supplier supplier)
        {
            this.supplier = supplier;
        }

        public EditSupplierRequestMessage(uint supplierID)
        {
            this.supplierID = supplierID;
        }

        private readonly Supplier supplier;
        public Supplier Supplier
        {
            get { return this.supplier; }
        }

        private readonly uint supplierID;
        public uint SupplierID
        {
            get { return this.supplierID; }
        }
    }
}
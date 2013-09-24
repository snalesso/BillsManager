using BillsManager.Model;
namespace BillsManager.ViewModel.Messages
{
    public class ActivateBillsSupplierFilterMessage
    {
        public ActivateBillsSupplierFilterMessage(Supplier supplier)
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
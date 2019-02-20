using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class SupplierAddedMessage : SupplierCRUDEvent
    {
        public SupplierAddedMessage(Supplier addedSupplier)
        {
            this.addedSupplier = addedSupplier;
        }

        private readonly Supplier addedSupplier;
        public Supplier AddedSupplier
        {
            get { return this.addedSupplier; }
        }
    }
}
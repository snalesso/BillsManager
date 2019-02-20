using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class SupplierDeletedMessage : SupplierCRUDEvent
    {
        public SupplierDeletedMessage(Supplier deletedSupplier)
        {
            this.deletedSupplier = deletedSupplier;
        }

        private readonly Supplier deletedSupplier;
        public Supplier DeletedSupplier
        {
            get { return deletedSupplier; }
        }
    }
}
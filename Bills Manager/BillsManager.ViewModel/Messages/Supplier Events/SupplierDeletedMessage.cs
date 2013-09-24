using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class SupplierDeletedMessage
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
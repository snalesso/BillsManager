using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    // TODO: update orders and requests classes' names
    public class ShowSuppliersBillsOrder
    {
        public ShowSuppliersBillsOrder(Supplier supplier)
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
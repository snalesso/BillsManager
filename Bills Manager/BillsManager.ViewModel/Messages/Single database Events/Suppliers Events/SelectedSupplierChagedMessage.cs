using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class SelectedSupplierChagedMessage
    {
        public SelectedSupplierChagedMessage(Supplier selectedSupplier)
        {
            this.selectedSupplier = selectedSupplier;
        }

        private readonly Supplier selectedSupplier;
        public Supplier SelectedSupplier
        {
            get { return this.selectedSupplier; }
        }

    }
}
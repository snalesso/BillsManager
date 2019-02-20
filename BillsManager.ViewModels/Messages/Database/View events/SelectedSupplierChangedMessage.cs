using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class SelectedSupplierChangedMessage
    {
        public SelectedSupplierChangedMessage(Supplier selectedSupplier)
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
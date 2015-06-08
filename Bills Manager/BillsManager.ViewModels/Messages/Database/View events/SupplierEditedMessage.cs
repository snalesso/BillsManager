using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class SupplierEditedMessage : SupplierCRUDEvent
    {
        public SupplierEditedMessage(Supplier newSupplierVersion, Supplier oldSupplierVersion)
        {
            this.newSupplierVersion = newSupplierVersion;
            this.oldSupplierVersion = oldSupplierVersion;
        }

        private readonly Supplier newSupplierVersion;
        public Supplier NewSupplierVersion
        {
            get { return this.newSupplierVersion; }
        }

        private readonly Supplier oldSupplierVersion;
        public Supplier OldSupplierVersion
        {
            get { return this.oldSupplierVersion; }
        }
    }
}
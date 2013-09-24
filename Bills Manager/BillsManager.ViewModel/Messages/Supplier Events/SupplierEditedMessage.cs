using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class SupplierEditedMessage
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
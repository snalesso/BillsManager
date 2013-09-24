using System;
using BillsManager.Model;
namespace BillsManager.ViewModel.Messages
{
    public class AskForSupplierMessage
    {
        public AskForSupplierMessage(uint supplierID, Action<Supplier> giveSupplier)
        {
            this.supplierID = supplierID;
            this.giveSupplier = giveSupplier;
        }

        private readonly uint supplierID;
        public uint SupplierID
        {
            get { return this.supplierID; }
        }

        private readonly Action<Supplier> giveSupplier;
        public Action<Supplier> GiveSupplier
        {
            get { return this.giveSupplier; }
        }
    }
}
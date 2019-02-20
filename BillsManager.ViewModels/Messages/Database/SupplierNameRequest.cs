using System;
using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class SupplierNameRequest
    {
        public SupplierNameRequest(uint supplierID, Action<string> giveSupplier)
        {
            this.supplierID = supplierID;
            this.giveSupplier = giveSupplier;
        }

        private readonly uint supplierID;
        public uint SupplierID
        {
            get { return this.supplierID; }
        }

        private readonly Action<string> giveSupplier;
        public Action<string> GiveSupplier
        {
            get { return this.giveSupplier; }
        }
    }
}
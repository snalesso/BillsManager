using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class SuppliersListChangedMessage
    {
        public SuppliersListChangedMessage(IEnumerable<Supplier> suppliers)
        {
            this.suppliers = suppliers;
        }

        private readonly IEnumerable<Supplier> suppliers;
        public IEnumerable<Supplier> Suppliers
        {
            get { return this.suppliers; }
        }
    }
}
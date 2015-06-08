using System.Collections.Generic;
using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class SuppliersListChangedMessage
    {
        public SuppliersListChangedMessage(
            ICollection<Supplier> suppliers)
        {
            this.suppliers = suppliers;
        }

        private readonly ICollection<Supplier> suppliers;
        public ICollection<Supplier> Suppliers
        {
            get { return this.suppliers; }
        }
    }
}
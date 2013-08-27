using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class AvailableSuppliersMessage
    {
        private readonly IEnumerable<Supplier> suppliers;

        public AvailableSuppliersMessage(IEnumerable<Supplier> suppliers)
        {
            this.suppliers = suppliers;
        }

        public IEnumerable<Supplier> AvailableSuppliers
        {
            get { return this.suppliers; }
        }
    }
}

using System;
using System.Collections.Generic;
using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class AvailableSuppliersRequestMessage
    {
        public AvailableSuppliersRequestMessage(Action<IEnumerable<Supplier>> acquireSuppliersAction)
        {
            this.acquireSuppliersAction = acquireSuppliersAction;
        }

        private readonly Action<IEnumerable<Supplier>> acquireSuppliersAction;
        public Action<IEnumerable<Supplier>> AcquireSuppliersAction
        {
            get { return this.acquireSuppliersAction; }
        }
    }
}
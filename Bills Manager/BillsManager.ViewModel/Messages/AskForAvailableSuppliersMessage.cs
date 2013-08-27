using System;
using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class AskForAvailableSuppliersMessage
    {
        private readonly Action<IEnumerable<Supplier>> acquireSuppliersAction;

        public AskForAvailableSuppliersMessage(Action<IEnumerable<Supplier>> acquireSuppliersAction)
        {
            this.acquireSuppliersAction = acquireSuppliersAction;
        }

        public Action<IEnumerable<Supplier>> AcquireSuppliersAction
        {
            get { return this.acquireSuppliersAction; }
        }
    }
}

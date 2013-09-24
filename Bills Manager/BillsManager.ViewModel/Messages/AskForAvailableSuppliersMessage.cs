using System;
using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class AskForAvailableSuppliersMessage
    {
        public AskForAvailableSuppliersMessage(Action<IEnumerable<Supplier>> acquireSuppliersAction)
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
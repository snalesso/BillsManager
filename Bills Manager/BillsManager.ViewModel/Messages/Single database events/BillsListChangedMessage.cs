using System.Collections.Generic;
using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class BillsListChangedMessage
    {
        public BillsListChangedMessage(IEnumerable<Bill> bills)
        {
            this.bills = bills;
        }

        private IEnumerable<Bill> bills;
        public IEnumerable<Bill> Bills
        {
            get { return this.bills; }
        }
    }
}
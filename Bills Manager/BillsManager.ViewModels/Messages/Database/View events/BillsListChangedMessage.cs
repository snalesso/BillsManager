using System.Collections.Generic;
using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class BillsListChangedMessage
    {
        public BillsListChangedMessage(ICollection<Bill> bills)
        {
            this.bills = bills;
        }

        private ICollection<Bill> bills;
        public ICollection<Bill> Bills
        {
            get { return this.bills; }
        }
    }
}
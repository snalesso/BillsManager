using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public abstract class BillCRUDEvent
    {
        public BillCRUDEvent(Bill bill)
        {
            this.bill = bill;
        }

        private readonly Bill bill;
        public Bill Bill
        {
            get { return this.bill; }
        }
    }
}
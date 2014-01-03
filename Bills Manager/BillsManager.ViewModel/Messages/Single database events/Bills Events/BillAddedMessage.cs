using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class BillAddedMessage : BillCRUDEvent
    {
        public BillAddedMessage(Bill addedBill)
        {
            this.addedBill = addedBill;
        }

        private readonly Bill addedBill;
        public Bill AddedBill
        {
            get { return this.addedBill; }
        }
    }
}
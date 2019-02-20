using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class BillEditedMessage : BillCRUDEvent
    {
        public BillEditedMessage(Bill newBill, Bill oldBill)
            : base(newBill)
        {
            this.oldBill = oldBill;
        }

        private readonly Bill oldBill;
        public Bill OldBill
        {
            get { return this.oldBill; }
        }
    }
}
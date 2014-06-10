using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class BillDeletedMessage : BillCRUDEvent
    {
        public BillDeletedMessage(Bill deletedBill)
        {
            this.deletedBill = deletedBill;
        }

        private readonly Bill deletedBill;
        public Bill DeletedBill
        {
            get { return this.deletedBill; }
        }
    }
}
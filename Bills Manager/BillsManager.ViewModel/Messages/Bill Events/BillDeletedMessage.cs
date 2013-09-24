using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class BillDeletedMessage
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
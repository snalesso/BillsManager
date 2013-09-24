using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class BillAddedMessage
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
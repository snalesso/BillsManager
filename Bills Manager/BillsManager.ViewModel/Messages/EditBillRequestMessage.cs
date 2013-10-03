using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class EditBillRequestMessage
    {
        public EditBillRequestMessage(Bill bill)
        {
            this.bill = bill;
        }

        public EditBillRequestMessage(uint billID)
        {
            this.billID = billID;
        }

        private readonly Bill bill;
        public Bill Bill
        {
            get { return this.bill; }
        }

        private readonly uint billID;
        public uint BillID
        {
            get { return this.billID; }
        }
    }
}
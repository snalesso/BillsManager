using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class EditBillOrder
    {
        public EditBillOrder(Bill bill)
        {
            this.bill = bill;
        }

        public EditBillOrder(uint billID)
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
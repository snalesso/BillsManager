using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class BillEditedMessage : BillCRUDEvent
    {
        public BillEditedMessage(Bill newBillVersion, Bill oldBillVersion)
        {
            this.newBillVersion = newBillVersion;
            this.oldBillVersion = oldBillVersion;
        }

        private readonly Bill newBillVersion;
        public Bill NewBillVersion
        {
            get { return this.newBillVersion; }
        }

        private readonly Bill oldBillVersion;
        public Bill OldBillVersion
        {
            get { return this.oldBillVersion; }
        }
    }
}
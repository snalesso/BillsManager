using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class BillEditedMessage
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
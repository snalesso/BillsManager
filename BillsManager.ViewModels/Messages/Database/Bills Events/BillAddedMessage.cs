using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class BillAddedMessage : BillCRUDEvent
    {
        public BillAddedMessage(Bill addedBill)
            : base(addedBill)
        {
        }
    }
}
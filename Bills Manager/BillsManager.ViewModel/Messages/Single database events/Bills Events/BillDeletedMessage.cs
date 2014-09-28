using BillsManager.Models;

namespace BillsManager.ViewModels.Messages
{
    public class BillDeletedMessage : BillCRUDEvent
    {
        public BillDeletedMessage(Bill deletedBill)
            : base(deletedBill)
        {
        }
    }
}
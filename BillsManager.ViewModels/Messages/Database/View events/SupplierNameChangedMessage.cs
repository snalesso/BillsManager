namespace BillsManager.ViewModel.Messages
{
    public class SupplierNameChangedMessage
    {
        public SupplierNameChangedMessage(string oldName, string newName)
        {
            this.oldName = oldName;
            this.newName = newName;
        }

        private readonly string oldName;
        public string OldName
        {
            get { return this.oldName; }
        }

        private readonly string newName;
        public string NewName
        {
            get { return this.newName; }
        }
    }
}
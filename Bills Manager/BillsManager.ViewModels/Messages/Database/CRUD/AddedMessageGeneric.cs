namespace BillsManager.ViewModels.Messages
{
    public partial class AddedMessage<T> : CRUDMessage<T>
        where T : class
    {
        public AddedMessage(T addedItem)
        {
            this.addedItem = addedItem;
        }

        private readonly T addedItem;
        public T AddedItem
        {
            get { return this.addedItem; }
        }
    }
}
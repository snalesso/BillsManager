namespace BillsManager.ViewModels.Messages
{
    public partial class DeletedMessage<T> : CRUDMessage<T>
        where T : class
    {
        public DeletedMessage(T deletedItem)
        {
            this.deletedItem = deletedItem;
        }

        private readonly T deletedItem;
        public T DeletedItem
        {
            get { return this.deletedItem; }
        }
    }
}
namespace BillsManager.ViewModels.Messages
{
    public partial class EditedMessage<T> : CRUDMessage<T>
        where T : class
    {
        public EditedMessage(T oldItem, T newItem)
        {
            this.oldItem = oldItem;
            this.newItem = newItem;
        }

        private readonly T oldItem;
        public T OldItem
        {
            get { return this.oldItem; }
        }

        private readonly T newItem;
        public T NewItem
        {
            get { return this.newItem; }
        }
    }
}
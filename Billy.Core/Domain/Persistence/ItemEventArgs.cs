using System;

namespace Billy.Domain.Persistence
{
    public sealed class ItemEventArgs<T> : EventArgs
    {
        public ItemEventArgs(T item)
        {
            this.Item = item != null ? item : throw new ArgumentNullException(nameof(item));
        }

        public T Item { get; }
    }
}

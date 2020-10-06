using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Core.Domain.Persistence
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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Collections.ObjectModel
{
    [Serializable]
    [DebuggerDisplay("Count = {Count}, HasFilters = {Filters != null}, HasComparer = {Comparer != null}")]
    public class ReadOnlyObservableCollectionEx<T> : IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
        where T : class, INotifyPropertyChanged
    {
        #region fields

        private readonly ObservableCollection<T> source;
        private IList<T> filteredSource;

        [NonSerialized]
        private Object _syncRoot;

        #endregion

        #region ctor

        public ReadOnlyObservableCollectionEx(ObservableCollection<T> source, IEnumerable<Predicate<T>> filters, IComparer<T> comparer)
        {
            if (source == null)
                throw new ArgumentNullException();

            this.source = source;
            this.filters = filters;
            this.comparer = comparer;

            this.UpdateItemsAndSort();

            this.SubscribeToSourceINCC(this.source);
            this.SubscribeToItemsINPC(this.source);

        }

        public ReadOnlyObservableCollectionEx(ObservableCollection<T> source)
            : this(source, null, null)
        {
        }

        #endregion ctor

        #region properties

        private IEnumerable<Predicate<T>> filters;
        public IEnumerable<Predicate<T>> Filters
        {
            get { return this.filters; }
            set
            {
                if (this.filters == value & value == null) return;

                this.filters = value;
                this.NotifyOfPropertyChange();

                this.UpdateItemsAndSort();

                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private IComparer<T> comparer;
        public IComparer<T> Comparer
        {
            get { return this.comparer; }
            set
            {
                if (this.comparer == value) return;

                this.comparer = value;
                this.NotifyOfPropertyChange("Comparer");

                if (this.Comparer != null)
                {
                    this.UpdateItemsOrder();
                }
                else
                {
                    this.UpdateItems();
                    this.NotifyOfPropertyChange("Count[]");
                }
                this.NotifyOfPropertyChange("Item[]");
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        #endregion properties

        #region methods

        private void UpdateItems()
        {
            var fs = this.source.AsEnumerable<T>();

            if (this.Filters != null)
                fs = fs.Where(i => this.Filters.All(f => f(i)));

            this.filteredSource = fs.ToList();
        }

        private void UpdateItemsOrder()
        {
            if (this.Comparer != null)
            {
                var x = this.filteredSource.AsEnumerable<T>();
                var ox = x.OrderBy(t => t, this.Comparer);
                this.filteredSource = ox.ToList();
            }
        }

        private void UpdateItemsAndSort()
        {
            var fs = this.source.AsEnumerable<T>();

            if (this.Filters != null)
                fs = fs.Where(i => this.Filters.All(f => f(i)));

            if (this.Comparer != null)
                fs = fs.OrderBy(t => t, this.Comparer);

            this.filteredSource = fs.ToList();
        }

        private bool IsItemAllowed(T item)
        {
            return (this.Filters == null || (this.Filters != null && this.Filters.All(f => f(item))));
        }

        private void SubscribeToSourceINCC(INotifyCollectionChanged source)
        {
            ((INotifyCollectionChanged)source).CollectionChanged += new NotifyCollectionChangedEventHandler(HandleCollectionChanged);
            ((INotifyPropertyChanged)source).PropertyChanged += new PropertyChangedEventHandler(HandlePropertyChanged);
        }

        private void SubscribeToItemsINPC(IEnumerable<T> items)
        {
            foreach (var i in this.source)
            {
                i.PropertyChanged += ItemPropertyChanged;
            }
        }

        private void UnsubscribeFromItemsINPC(IEnumerable<T> items)
        {
            foreach (var i in this.source)
            {
                i.PropertyChanged -= ItemPropertyChanged;
            }
        }

        #endregion methods

        #region implementations

        #region IList<T>, IList, IReadOnlyList<T>

        public int Count
        {
            get { return this.filteredSource.Count; }
        }

        public T this[int index]
        {
            get { return this.filteredSource[index]; }
        }

        public bool Contains(T value)
        {
            return this.filteredSource.Contains(value);
        }

        public void CopyTo(T[] array, int index)
        {
            this.filteredSource.CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.filteredSource.GetEnumerator();
        }

        public int IndexOf(T value)
        {
            return this.filteredSource.IndexOf(value);
        }

        protected IList<T> Items
        {
            get
            {
                return this.filteredSource;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        T IList<T>.this[int index]
        {
            get { return this.filteredSource[index]; }
            set
            {
                throw new NotSupportedException();
            }
        }

        void ICollection<T>.Add(T value)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        void IList<T>.Insert(int index, T value)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T value)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.filteredSource).GetEnumerator();
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    ICollection c = this.filteredSource as ICollection;
                    if (c != null)
                    {
                        _syncRoot = c.SyncRoot;
                    }
                    else
                    {
                        System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                    }
                }
                return _syncRoot;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException();
            }

            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException();
            }

            if (index < 0)
            {
                throw new ArgumentException();
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException();
            }

            T[] items = array as T[];
            if (items != null)
            {
                this.filteredSource.CopyTo(items, index);
            }
            else
            {
                //
                // Catch the obvious case assignment will fail.
                // We can found all possible problems by doing the check though.
                // For example, if the element type of the Array is derived from T,
                // we can't figure out if we can successfully copy the element beforehand.
                //
                Type targetType = array.GetType().GetElementType();
                Type sourceType = typeof(T);
                if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                {
                    throw new ArgumentException();
                }

                //
                // We can't cast array of value type to object[], so we don't support 
                // widening of primitive types here.
                //
                object[] objects = array as object[];
                if (objects == null)
                {
                    throw new ArgumentException();
                }

                int count = this.filteredSource.Count;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        objects[index++] = this.filteredSource[i];
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException();
                }
            }
        }

        bool IList.IsFixedSize
        {
            get { return true; }
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        object IList.this[int index]
        {
            get { return this.filteredSource[index]; }
            set
            {
                throw new NotSupportedException();
            }
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        private static bool IsCompatibleObject(object value)
        {
            // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>. 
            return ((value is T) || (value == null && default(T) == null));
        }

        bool IList.Contains(object value)
        {
            if (IsCompatibleObject(value))
            {
                return this.Contains((T)value);
            }
            return false;
        }

        int IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
            {
                return this.IndexOf((T)value);
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion IList<T>, IList, IReadOnlyList<T>

        #region INotifyCollectionChanged

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { this.CollectionChanged += value; }
            remove { this.CollectionChanged -= value; }
        }

        [field: NonSerialized]
        protected virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, args);
            }
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var newItem = e.NewItems[0] as T;

                        newItem.PropertyChanged += this.ItemPropertyChanged;

                        // TODO: try invert if conditions order -> might remove the repeated check in this.IsItemAllowed
                        if (this.IsItemAllowed(newItem)) // if it has to be added
                        {
                            int addIndex = e.NewStartingIndex;

                            if (this.Comparer != null) // if it has to be sorted
                            {
                                this.filteredSource.Insert(0, newItem); // add it anywhere and sort
                                this.UpdateItemsOrder();

                                addIndex = this.IndexOf(newItem);
                            }
                            else // if it doesn't have to be sorted
                            {
                                if (this.Filters != null) // if there are filters
                                {
                                    this.UpdateItems(); // update the whole list to put it in the right index

                                    addIndex = this.IndexOf(newItem);
                                }
                                else // if no filtering and no sorting
                                    this.filteredSource.Insert(addIndex, newItem);
                            }

                            this.NotifyOfPropertyChange("Item[]");
                            this.NotifyOfPropertyChange("Count");
                            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, addIndex));
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    {
                        var movedItem = e.OldItems[0] as T;

                        if (this.Comparer == null) // if the item IS contained AND sorting is NOT ACTIVE
                        {
                            if (this.Filters != null) // if there are filters
                            {
                                if (this.Filters.All(f => f(movedItem))) // if the it is contained
                                {
                                    var oldIndex = this.IndexOf(movedItem);
                                    this.UpdateItems(); // TODO: check if there's a way to calculate the new index
                                    var newIndex = this.IndexOf(movedItem);

                                    this.NotifyOfPropertyChange("Item[]");
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.OldItems, newIndex, oldIndex));
                                }
                                else
                                {
                                    // this condition is never reached
                                }
                            }
                            else // if no filtering and no sorting
                            {
                                this.filteredSource.RemoveAt(e.OldStartingIndex);
                                this.filteredSource.Insert(e.NewStartingIndex, movedItem);

                                this.NotifyOfPropertyChange("Item[]");
                                this.OnCollectionChanged(e);
                            }
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        var removedItem = e.OldItems[0] as T;

                        removedItem.PropertyChanged -= this.ItemPropertyChanged;

                        if (this.IsItemAllowed(removedItem)) // if it is contained, remove it
                        {
                            var removeIndex = e.OldStartingIndex;

                            if (this.Filters != null || this.Comparer != null) // if the index might be different
                                removeIndex = this.IndexOf(removedItem);

                            this.filteredSource.RemoveAt(removeIndex);

                            this.NotifyOfPropertyChange("Item[]");
                            this.NotifyOfPropertyChange("Count");
                            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, removeIndex));
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        var oldItem = e.OldItems[0] as T;
                        var newItem = e.NewItems[0] as T;

                        oldItem.PropertyChanged -= this.ItemPropertyChanged;
                        newItem.PropertyChanged += this.ItemPropertyChanged;

                        if (this.Filters != null)
                        {
                            int addIndex = e.NewStartingIndex;
                            var removeIndex = this.IndexOf(oldItem); // remove it

                            bool oldRemoved = false;
                            bool newAdded = false;

                            if (this.Filters.All(f => f(oldItem))) // if the old item was contained
                            {
                                this.filteredSource.RemoveAt(removeIndex);
                                oldRemoved = true;
                            }

                            if (this.Filters.All(f => f(newItem))) // if the new item has to be inserted
                            {
                                if (this.Comparer != null) // if it has to be sorted
                                {
                                    this.filteredSource.Insert(0, newItem); // add it anywhere and sort
                                    this.UpdateItemsOrder();
                                }
                                else // if it doesn't have to be sorted
                                {
                                    this.UpdateItems(); // update the whole list to put it in the right index
                                }

                                addIndex = this.IndexOf(newItem);
                                newAdded = true;
                            }

                            if (oldRemoved)
                            {
                                if (newAdded)
                                    if (removeIndex == addIndex)
                                    {
                                        this.NotifyOfPropertyChange("Item[]");
                                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems, e.OldItems, removeIndex));
                                    }
                                    else
                                    {
                                        this.NotifyOfPropertyChange("Item[]");
                                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, removeIndex));
                                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, addIndex));
                                    }
                                else
                                {
                                    this.NotifyOfPropertyChange("Item[]");
                                    this.NotifyOfPropertyChange("Count[]");
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, removeIndex));
                                }
                            }
                            else
                            {
                                if (newAdded)
                                {
                                    this.NotifyOfPropertyChange("Item[]");
                                    this.NotifyOfPropertyChange("Count[]");
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, addIndex));
                                }
                            }
                        }
                        else // if there are no filters
                        {
                            var removeIndex = e.OldStartingIndex;
                            var insertIndex = e.NewStartingIndex;

                            if (this.Comparer != null) // if sorting is active
                            {
                                removeIndex = this.IndexOf(oldItem);
                                this.filteredSource.RemoveAt(removeIndex);
                                this.filteredSource.Insert(0, newItem);
                                this.UpdateItemsOrder();
                                insertIndex = this.IndexOf(newItem);

                                this.NotifyOfPropertyChange("Item[]");
                                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, removeIndex));
                                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, insertIndex));
                            }
                            else
                            {
                                removeIndex = this.IndexOf(oldItem);
                                this.filteredSource.RemoveAt(removeIndex);
                                this.filteredSource.Insert(insertIndex, newItem);
                                insertIndex = this.IndexOf(newItem);

                                this.NotifyOfPropertyChange("Item[]");
                                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems, e.OldItems, removeIndex));
                            }
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    {
                        // URGENT: this doesn't remove subscriptions from removed items, just adds to new ones

                        // when the source collection is reset, we must refresh subscriptions, in order to unsubscribe from removed elements
                        this.UnsubscribeFromItemsINPC(this.source);
                        this.UpdateItemsAndSort();
                        this.SubscribeToItemsINPC(this.source);

                        this.OnCollectionChanged(e);
                        break;
                    }

                default:
                    throw new InvalidEnumArgumentException(@"Unknown collection action: " + e.Action);
            }
        }

        #endregion INotifyCollectionChanged

        #region INotifyPropertyChanged

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this.PropertyChanged += value; }
            remove { this.PropertyChanged -= value; }
        }

        [field: NonSerialized]
        protected virtual event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, args);
            }
        }

        void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e);
        }

        public virtual void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var changedItem = sender as T;
            if (sender == null)
                throw new InvalidOperationException("sender is not of type " + typeof(T).Name);

            if (this.IsItemAllowed(changedItem)) // if the item is allowed
            {
                if (this.Contains(changedItem))
                {
                    this.UpdateItemsOrder();
                    this.NotifyOfPropertyChange("Item[]");
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
                else // if it has to be added
                {
                    if (this.Comparer != null)
                    {
                        // TODO: check whether add is more performant than insert(0)
                        this.filteredSource.Insert(0, changedItem); // add it anywhere an sort
                        this.UpdateItemsOrder();
                    }
                    else
                    {
                        this.UpdateItems();
                    }
                    var addIndex = this.IndexOf(changedItem);
                    this.NotifyOfPropertyChange("Item[]");
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItem, addIndex));
                }
            }
            else
            {
                if (this.Contains(changedItem))
                {
                    var removeIndex = this.IndexOf(changedItem);
                    this.filteredSource.Remove(changedItem);
                    this.NotifyOfPropertyChange("Item[]");
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItem, removeIndex));
                }
            }
        }

        #endregion INotifyPropertyChanged

        #endregion implementations
    }
}
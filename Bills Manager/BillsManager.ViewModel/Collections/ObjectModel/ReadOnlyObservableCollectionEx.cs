using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Collections.ObjectModel
{
    [Serializable]
    [DebuggerDisplay("Count = {Count}, HasFilters = {Filters != null}")]
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

        public ReadOnlyObservableCollectionEx(ObservableCollection<T> source, IEnumerable<Predicate<T>> filters)
        {
            if (source == null)
                throw new ArgumentNullException();

            this.source = source;
            this.filters = filters;

            this.UpdateFiltering();

            ((INotifyCollectionChanged)this.source).CollectionChanged += new NotifyCollectionChangedEventHandler(HandleCollectionChanged);
            ((INotifyPropertyChanged)this.source).PropertyChanged += new PropertyChangedEventHandler(HandlePropertyChanged);

            this.SubscribeItems(this.source);

        }

        public ReadOnlyObservableCollectionEx(ObservableCollection<T> source)
            : this(source, null)
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

                this.UpdateFiltering();
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        #endregion properties

        #region methods

        private void UpdateFiltering()
        {
            if (this.Filters != null)
                this.filteredSource = this.source.Where(i => this.Filters.All(f => f(i))).ToList();
            else
                this.filteredSource = this.source.ToList();

            this.NotifyOfPropertyChange("Item[]");
            this.NotifyOfPropertyChange("Count");
        }

        private void SubscribeItems(IEnumerable<T> items)
        {
            foreach (var i in this.source)
            {
                i.PropertyChanged += itemPropertyChanged;
            }
        }

        private void UnsubscribeItems(IEnumerable<T> items)
        {
            foreach (var i in this.source)
            {
                i.PropertyChanged -= itemPropertyChanged;
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

        void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        (e.NewItems[0] as T).PropertyChanged += this.itemPropertyChanged;

                        if (this.Filters != null)
                        {
                            if (this.Filters.All(f => f(e.NewItems[0] as T))) // if the added item passes all filters
                            {
                                // add it
                                this.UpdateFiltering(); // TODO: check if there's a way to find the insert position and just add the single item
                                var addIndex = this.IndexOf(e.NewItems[0] as T);

                                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, addIndex));
                            }
                        }
                        else // if there is no filter
                        {
                            this.filteredSource.Insert(e.NewStartingIndex, e.NewItems[0] as T);

                            this.OnCollectionChanged(e);
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    {
                        if (this.Filters != null)
                        {
                            // if the moved item passes the filter
                            if (this.Filters.All(f => f(e.OldItems[0] as T)))
                            {
                                // if it was already in the filtered list
                                var wasAlreadyContained = this.Contains(e.OldItems[0] as T);
                                int oldIndex = -1;

                                if (wasAlreadyContained)
                                    oldIndex = this.IndexOf(e.OldItems[0] as T);

                                this.UpdateFiltering(); // TODO: check if there's a way to just add the item
                                var newIndex = this.IndexOf(e.OldItems[0] as T);

                                if (wasAlreadyContained)
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.OldItems, newIndex, oldIndex));
                                else
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.OldItems, newIndex));
                            }
                            // if the moved item doesn't pass the filter but it's contained
                            else if (this.Contains(e.OldItems[0] as T))
                            {
                                // remove it
                                var removeIndex = this.IndexOf(e.OldItems[0] as T);
                                this.filteredSource.RemoveAt(removeIndex);

                                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.OldItems, removeIndex));
                            }
                        }
                        else // if there are no filters
                        {
                            this.filteredSource.RemoveAt(e.OldStartingIndex);
                            this.filteredSource.Insert(e.NewStartingIndex, e.NewItems[0] as T);

                            this.OnCollectionChanged(e);
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        (e.OldItems[0] as T).PropertyChanged -= this.itemPropertyChanged;

                        if (this.Filters != null)
                        {
                            // if the item is contained (passes the filter)
                            if (this.Filters.All(f => f(e.OldItems[0] as T)))
                            {
                                // remove it
                                var removeIndex = this.IndexOf(e.OldItems[0] as T);
                                this.filteredSource.RemoveAt(removeIndex);

                                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, removeIndex));
                            }
                        }
                        else // if there are no filters
                        {
                            this.filteredSource.RemoveAt(e.OldStartingIndex);

                            this.OnCollectionChanged(e);
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        (e.OldItems[0] as T).PropertyChanged -= this.itemPropertyChanged;
                        (e.NewItems[0] as T).PropertyChanged += this.itemPropertyChanged;

                        if (this.Filters != null)
                        {
                            // if the item that has been replaced is contained (passes the filter)
                            if (this.Filters.All(f => f(e.OldItems[0] as T)))
                            {
                                // remove it
                                var replaceIndex = this.IndexOf(e.OldItems[0] as T);
                                this.filteredSource.RemoveAt(replaceIndex);

                                // if the new one is allowed
                                if (this.Filters.All(f => f(e.NewItems[0] as T)))
                                {
                                    // replace it
                                    this.filteredSource.Insert(replaceIndex, e.NewItems[0] as T);

                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems, e.OldItems, replaceIndex));
                                }
                                else // if the new one it's not allowed
                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, replaceIndex));
                            }
                            else // if the replaced item is not contained
                            {
                                // but the new one is allowed
                                if (this.Filters.All(f => f(e.NewItems[0] as T)))
                                {
                                    // add it
                                    this.UpdateFiltering(); // TODO: check if there's a way to just add the item
                                    var addIndex = this.IndexOf(e.NewItems[0] as T);

                                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems, addIndex));
                                }
                            }
                        }
                        else // if there are no filters
                        {
                            this.filteredSource.RemoveAt(e.OldStartingIndex);
                            this.filteredSource.Insert(e.NewStartingIndex, e.NewItems[0] as T);

                            this.OnCollectionChanged(e);
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    {
                        // URGENT: this doesn't remove subscriptions from removed items, just adds to new ones
                        this.UnsubscribeItems(this.source);
                        this.UpdateFiltering();
                        this.SubscribeItems(this.source);

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

        void itemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as T;
            if (sender == null)
                throw new InvalidOperationException("sender is not a " + typeof(T).Name);

            if (this.Filters == null || this.Filters.All(f => f(item))) // if the item passes the filters
            {
                this.UpdateFiltering();
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            else
            {
                if (this.Contains(item))
                {
                    var tiIndex = this.IndexOf(item);
                    this.filteredSource.Remove(item);
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, tiIndex));
                }
            }
        }

        #endregion INotifyPropertyChanged

        #endregion implementations
    }
}
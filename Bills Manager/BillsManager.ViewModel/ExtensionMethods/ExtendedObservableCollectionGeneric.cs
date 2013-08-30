using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BillsManager.ViewModel
{
    public class ExtendedObservableCollection<T> : ObservableCollection<T>
    {
        #region ctor

        public ExtendedObservableCollection()
            : base()
        {
        }

        public ExtendedObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public ExtendedObservableCollection(List<T> list)
            : base(list)
        {
        }

        #endregion

        #region ranged

        public void AddRange(IEnumerable<T> newItems)
        {
            this.CheckReentrancy();

            foreach (T item in newItems)
            {
                this.Items.Add(item);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        }

        public void RemoveRange(IEnumerable<T> deleteItems)
        {
            this.CheckReentrancy();

            foreach (T item in deleteItems)
            {
                this.Items.Remove(item);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, deleteItems));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        }

        #endregion

        //#region auto sorting

        //private bool autoSort;
        //public bool AutoSort
        //{
        //    get { return this.autoSort; }
        //    set
        //    {
        //        if (this.autoSort != value)
        //        {
        //            this.autoSort = value;
        //        }

        //        if (this.AutoSort)
        //            this.Sort();
        //    }
        //}

        //protected Comparer<T> sorter;
        //public Comparer<T> Sorter
        //{
        //    get { return this.sorter; }
        //    set
        //    {
        //        if (this.sorter != value)
        //        {
        //            this.sorter = value;
        //            this.OnPropertyChanged(new PropertyChangedEventArgs("Sorter"));

        //            if (this.AutoSort)
        //                this.Sort();
        //        }
        //    }
        //}

        //public void Sort()
        //{
        //    this.CheckReentrancy();

        //    this.Items.OrderBy(item => item, this.Sorter);

        //    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));
        //}

        //#endregion
    }
}

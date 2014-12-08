using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BillsManager.ViewModels
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<T> data, IEnumerable<Predicate<T>> predicates) // TODO: optimize
        {
            return data.Where(i => predicates.All(p => p(i)));

            //if (predicates == null)
            //{
            //    foreach (T item in data)
            //        yield return item;
            //}
            //else
            //{
            //    bool respects;

            //    foreach (T item in data)
            //    {
            //        respects = true;

            //        foreach (Predicate<T> pred in predicates)
            //        {
            //            if (!pred.Invoke(item))
            //            {
            //                respects = false;
            //                break;
            //            }
            //        }

            //        if (respects)
            //            yield return item;
            //    }
            //}
        }

        public static void AddSorted<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            int i = 0;
            while (i < list.Count && comparer.Compare(list[i], item) < 0)
                i++;

            list.Insert(i, item);
        }

        public static void SortEdited<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            list.Remove(item);

            int i = 0;
            while (i < list.Count && comparer.Compare(list[i], item) < 0)
                i++;

            var obs = list as ObservableCollection<T>;
            if (obs != null)
                obs.Move(obs.IndexOf(item), i);
            else
                list.Insert(i, item);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BillsManager.ViewModels
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<T> data, IEnumerable<Predicate<T>> predicates) // TODO: optimize
        {
            bool respects;

            foreach (T value in data)
            {
                respects = true;

                foreach (Predicate<T> pred in predicates)
                {
                    if (!pred.Invoke(value))
                    {
                        respects = false;
                        break;
                    }
                }

                if (respects)
                    yield return value;
            }
        }

        public static ulong ULongCount<T>(this IEnumerable<T> data)
        {
            ulong count = 0;
            foreach (var item in data)
                count++;
            return count;
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

            list.AddSorted(item, comparer);
        }
    }
}
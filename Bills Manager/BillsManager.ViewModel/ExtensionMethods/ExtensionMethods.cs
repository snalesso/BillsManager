using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BillsManager.ViewModel
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<T> data, IEnumerable<Func<T, bool>> predicates) // TODO: optimize
        {
            bool respects;

            foreach (T value in data)
            {
                respects = true;

                foreach (Func<T, bool> pred in predicates)
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
    }
}

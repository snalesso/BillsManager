﻿using System;
using System.Collections.Generic;

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
    }
}
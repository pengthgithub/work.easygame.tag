using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public static class CollectionHelper
    {
        public static List<T> AddExclusive<T>(this List<T> list, T item, out bool success)
        {
            success = false;
            int idx = list.IndexOf(item);
            if (idx < 0)
            {
                list.Add(item);
                success = true;
            }

            return list;
        }

        public static List<T> AddExclusive<T>(this List<T> list, T item)
        {
            bool success = false;
            return AddExclusive(list, item, out success);
        }
    }


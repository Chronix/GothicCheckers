using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public static class ClassExtensions
    {
        public static void AddRange<T>(this Collection<T> coll, IEnumerable<T> items)
        {
            foreach (T item in items) coll.Add(item);
        }

        public static void OnIndices<T>(this IList<T> list, Action<T> action, IEnumerable<int> indices) where T : class
        {
            foreach (int i in indices)
            {
                action(list[i]);
            }
        }
    }
}

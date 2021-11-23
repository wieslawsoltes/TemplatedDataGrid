using System;
using System.Collections.Generic;
using System.Linq;

namespace TemplatedDataGrid
{
    internal static class EnumerableExtesions
    {
        public static IEnumerable<T> Walk<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> children)
        {
            foreach (var item in items)
            {
                if (children(item).Walk(children).Any())
                {
                    yield return item;
                }
            }
        }
    }
}

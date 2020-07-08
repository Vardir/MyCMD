using System;
using System.Collections.Generic;

namespace Vardirsoft.MyCmd.Core.Helpers
{
    public static class CollectionHelper
    {
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            foreach (var item in collection)
            {
                if (predicate(item))
                    continue;

                yield return item;
            }
        }
    }
}
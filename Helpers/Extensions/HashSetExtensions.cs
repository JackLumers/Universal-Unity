using System.Collections.Generic;

namespace UniversalUnity.Helpers.Extensions
{
    public static class HashSetExtensions
    {
        public static bool TryAdd<T>(this ISet<T> set, T item)
        {
            if (set.Contains(item)) return false;
            set.Add(item);
            return true;

        }
    }
}
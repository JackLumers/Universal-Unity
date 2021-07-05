using System.Collections.Generic;

namespace UniversalUnity.Helpers.Extensions
{
    public static class HashSetExtension
    {
        public static void SafeAdd<T>(this ISet<T> set, T item)
        {
            if (!set.Contains(item))
            {
                set.Add(item);
            }
        }
    }
}
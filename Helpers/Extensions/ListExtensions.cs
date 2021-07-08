using System.Collections.Generic;

namespace UniversalUnity.Helpers.Extensions
{
    public static class ListExtension
    {
        /// <remarks>Note that random is not save here and can only be used if you don't need safety!</remarks>
        public static T GetRandomItem<T>(this IList<T> source)
        {
            var randomIndex = UnityEngine.Random.Range(0, source.Count);
            return source[randomIndex];
        }
    }
}
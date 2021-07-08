using System;
using System.Collections.Generic;
using System.Linq;
using Random = Unity.Mathematics.Random;

namespace UniversalUnity.Helpers.Extensions
{
    public static class CollectionExtension
    {
        private static Random _random = new Random((uint) UnityEngine.Random.Range(1, 100000));
        
        /// <summary>
        /// Returns specified number of items randomly selected from <see cref="source"/>
        /// </summary>
        /// <remarks>Note that random is not save here and can only be used if you don't need safety!</remarks>
        /// <param name="source">Source with items</param>
        /// <param name="quantity">Number of items to get from <see cref="source"/></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ICollection<T> GetRandomQuantity<T>(this ICollection<T> source, int quantity)
        {
            if (source.Count < quantity)
            {
                throw new ArgumentException($"Param {nameof(quantity)} must be less or equal to the {nameof(source)} length!");
            }
            
            var randomlyOrdered = source.OrderBy(x => _random.NextInt());
            var selected = new List<T>();

            int i = 0;
            foreach (var random in randomlyOrdered)
            {
                if (i >= quantity) break;
                selected.Add(random);
                i++;
            }

            return selected;
        }
    }
}
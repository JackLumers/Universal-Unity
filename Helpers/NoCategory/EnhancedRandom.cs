using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Random = Unity.Mathematics.Random;

namespace Common.Helpers
{
    public static class EnhancedRandom
    {
        private static Random _random = new Random((uint) UnityEngine.Random.Range(1, 100000));
        
        /// <summary>
        /// Returns specified number of items randomly selected from <see cref="source"/>
        /// </summary>
        /// <param name="source">Source with items</param>
        /// <param name="quantity">Number of items to get from <see cref="source"/></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static ICollection<TSource> GetRandomFrom<TSource>(ICollection<TSource> source, int quantity)
        {
            if (source.Count < quantity)
            {
                throw new ArgumentException($"Param {nameof(quantity)} must be less or equal to the {nameof(source)} length!");
            }
            
            var randomlyOrdered = source.OrderBy(x => _random.NextInt());
            var selected = new List<TSource>();

            int i = 0;
            foreach (var random in randomlyOrdered)
            {
                if (i >= quantity) break;
                selected.Add(random);
                i++;
            }

            return selected;
        }

        public static TItem GetRandomItem<TItem>(List<TItem> source)
        {
            var randomIndex = UnityEngine.Random.Range(0, source.Count);
            return source[randomIndex];
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversalUnity.Helpers.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Merges any number of dictionaries together.
        /// </summary>
        /// <exception cref="ArgumentException">In case of duplicated keys.</exception>
        /// <returns>Merged result.</returns>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            params Dictionary<TKey, TValue>[] dictionaries)
        {
            return dictionaries.SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
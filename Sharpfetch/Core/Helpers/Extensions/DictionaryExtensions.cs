using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Sharpfetch.Core.Helpers.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                dictionary.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="index"></param>
        /// <param name="items"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddRangeAt<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, int index, IEnumerable<KeyValuePair<TKey, TValue>> items) where TKey : notnull
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (index < 0 || index > dictionary.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            IDictionary<TKey, TValue> frontDictPart = dictionary.Take(index).ToDictionary();

            IDictionary<TKey, TValue> remainderDictPart = dictionary.Skip(index).ToDictionary();

            frontDictPart.AddRange(items);

            foreach (var item in remainderDictPart)
            {
                frontDictPart.Add(item.Key, item.Value);
            }

            dictionary.Clear();

            foreach (var item in frontDictPart)
            {
                dictionary.Add(item.Key, item.Value);
            }
        }
    }
}

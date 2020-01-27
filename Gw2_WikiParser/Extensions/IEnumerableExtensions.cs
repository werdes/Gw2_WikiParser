using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gw2_WikiParser.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> oSource, Func<TSource, TKey> oKeySelector)
        {
            HashSet<TKey> oSeenKeys = new HashSet<TKey>();
            foreach (TSource oElement in oSource)
            {
                if (oSeenKeys.Add(oKeySelector(oElement)))
                {
                    yield return oElement;
                }
            }
        }

        public static string Join(this IEnumerable<string> lst, string separator)
        {
            return string.Join(separator, lst);
        }

        public static void AddIfNotNull<T>(this List<T> lst, T obj)
        {
            if (obj != null)
                lst.Add(obj);
        }
        public static void InsertIfNotNull<T>(this List<T> lst, int index, T obj)
        {
            if (obj != null)
                lst.Insert(index, obj);
        }

        public static void AddRangeIfNotNull<T>(this List<T> lst, IEnumerable<T> objects)
        {
            if (objects != null)
                lst.AddRange(objects);

        }

        public static bool ContainsAny<T>(this IEnumerable<T> haystack, IEnumerable<T> needles) where T : new()
        {
            return haystack.Any(x => needles.Contains(x));
        }


        public static List<List<T>> SplitIntoChunks<T>(this IEnumerable<T> list, int chunkSize)
        {
            List<List<T>> retVal = new List<List<T>>();

            if (chunkSize <= 0)
                chunkSize = list.Count();

            if (list.Count() > 0)
            {
                int index = 0;

                while (index < list.Count())
                {
                    int count = list.Count() - index > chunkSize ? chunkSize : list.Count() - index;
                    retVal.Add(list.Skip(index).Take(count).ToList());

                    index += chunkSize;
                }
            }

            return retVal;
        }


        public static void AddIfNotNullOrEmpty(this List<string> lst, string element)
        {
            if (!string.IsNullOrEmpty(element))
            {
                lst.Add(element);
            }
        }
        public static void AddIfNotNullOrWhiteSpace(this List<string> lst, string element)
        {
            if (!string.IsNullOrWhiteSpace(element))
            {
                lst.Add(element);
            }
        }

        public static void AddRange<K, V>(this Dictionary<K, V> dict, IEnumerable<KeyValuePair<K, V>> kvps)
        {
            foreach (KeyValuePair<K, V> kvp in kvps)
            {
                dict.Add(kvp.Key, kvp.Value);
            }
        }

        public static void AddRangeIfNotContainsKey<K, V>(this Dictionary<K, V> dict, IEnumerable<KeyValuePair<K, V>> kvps)
        {
            foreach (KeyValuePair<K, V> kvp in kvps)
            {
                if(!dict.ContainsKey(kvp.Key))
                    dict.Add(kvp.Key, kvp.Value);
            }
        }

        public static void ForEach<K, V>(this Dictionary<K, V> dict, Action<K, V> action)
        {
            foreach(K key in dict.Keys)
            {
                action(key, dict[key]);
            }
        }
    }
}

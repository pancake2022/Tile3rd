using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSFramework
{
    public static class IEnumerableExtension
    {
        public static int IndexOf<T> (this IEnumerable<T> collection, T element)
        {
            var index = 0;
            var iterator = collection.GetEnumerator();
            while (iterator.MoveNext())
            {
                if (iterator.Current.Equals(element))
                    return index;
                else
                    ++index;
            }
            return -1;
        }

        public static int IndexOf<T> (this IEnumerable<T> collection, Func<T, bool> condition)
        {
            if (condition != null)
            {
                var index = 0;
                var iterator = collection.GetEnumerator();
                while (iterator.MoveNext())
                {
                    if (condition.Invoke(iterator.Current))
                        return index;
                    else
                        ++index;
                }
            }
            return -1;
        }
    }

    public static class ListExtension
    {
        public static T FirstElement<T> (this List<T> list)
        {
            if (list.Count > 0)
                return list[0];
            return default(T);
        }

        public static T LastElement<T> (this List<T> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            return default(T);
        }

        public static T EnsureGet<T> (this List<T> list, int index)
        {
            if (index >= 0 && index < list.Count)
                return list[index];
            else
                return default(T);
        }

        public static List<T> Shuffle<T> (this List<T> list, System.Random random = null)
        {
            if (random == null)
                random = new System.Random();
                
            var n = list.Count;
            while (n > 1)
            {
                --n;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public static List<T> QuickEach<T> (this List<T> list, Action<int, T> cb)
        {
            for (var i = 0; i < list.Count; ++i)
                cb(i, list[i]);
            return list;
        }

        public static List<T> QuickEach<T> (this List<T> list, Func<int, T, bool> cb)
        {
            for (var i = 0; i < list.Count; ++i)
            {
                if (!cb(i, list[i]))
                    break;
            }
            return list;
        }
    }

    public static class DictionaryExtension
    {
        public static TValue EnsureGet<TKey, TValue> (this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> constructor = null)
        {
            if (!dict.TryGetValue(key, out var value))
            {
                if (constructor == null)
                {
                    value = (TValue)System.Activator.CreateInstance(typeof(TValue));
                }
                else
                {
                    value = constructor();
                }
                dict[key] = value;
            }
            return value;
        }
    }

    public static class TransformExtension
    {
        public static void SetActive (this Transform t, bool value)
        {
            if (t)
                t.gameObject.SetActive(value);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSFramework
{
    [Serializable]
    public class StorageList<TValue> : List<TValue>
    {
        protected bool _force_save = false;

        public StorageList (bool force_save = false)
        {
            _force_save = force_save;
        }

        protected void dirty ()
        {
            Framework.SetStorageDirty(_force_save);
        }

        public new void Add(TValue item)
        {
            base.Add(item);
            dirty();
        }

        public new void AddRange(IEnumerable<TValue> collection)
        {
            base.AddRange(collection);
            dirty();
        }

        public new void Clear()
        {
            base.Clear();
            dirty();
        }

        public new List<TOutput> ConvertAll<TOutput>(Converter<TValue, TOutput> converter)
        {
            return base.ConvertAll(converter);
        }

        public new void Insert(int index, TValue item)
        {
            base.Insert(index, item);
            dirty();
        }

        public new void InsertRange(int index, IEnumerable<TValue> collection)
        {
            base.InsertRange(index, collection);
            dirty();
        }

        public new bool Remove(TValue item)
        {
            dirty();
            return base.Remove(item);
        }

        public new int RemoveAll(Predicate<TValue> match)
        {
            dirty();
            return base.RemoveAll(match);
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            dirty();
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            dirty();
        }

        public new void Reverse()
        {
            base.Reverse();
            dirty();
        }

        public new void Sort(Comparison<TValue> comparison)
        {
            base.Sort(comparison);
            dirty();
        }

        public new TValue this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                if (!value.Equals(base[index]))
                {
                    base[index] = value;
                    dirty();
                }
            }
        }
    }
}
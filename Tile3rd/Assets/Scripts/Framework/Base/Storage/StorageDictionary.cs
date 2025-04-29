using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSFramework
{
    [Serializable]
    public class StorageDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public delegate TValue StorageValueConstructor();
        protected bool _force_save = false;

        public StorageDictionary (bool force_save = false)
        {
            _force_save = force_save;
        }

        protected void dirty ()
        {
            Framework.SetStorageDirty(_force_save);
        }

        public new void Add (TKey key, TValue value)
        {
            base.Add(key, value);
            dirty();
        }

        public new void Clear ()
        {
            base.Clear();
            dirty();
        }

        public new bool Remove (TKey key)
        {
            var result = base.Remove(key);
            dirty();
            return result;
        }

        public new TValue this[TKey key]
        {
            get
            {
                return base[key];
            }
            set
            {
                if (!base.ContainsKey(key) || !value.Equals(base[key]))
                {
                    base[key] = value;
                    dirty();
                }
            }
        }

        public TValue EnsureGetValue (TKey key, StorageValueConstructor constructor = null)
        {
            if (!TryGetValue(key, out var value))
            {
                if (constructor == null)
                {
                    value = (TValue)System.Activator.CreateInstance(typeof(TValue));
                }
                else
                {
                    value = constructor();
                    
                }
                base[key] = value;
            }
            return value;
        }
    }
}
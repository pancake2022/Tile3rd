using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework
{
    public class ShareDataManager : Module<Framework>
    {
        private Dictionary<Type, object> _data_dict;

        protected override IEnumerator on_init(params object[] param_list)
        {
            _data_dict = new Dictionary<Type, object>();
            yield return null;
        }

        public T Data<T> () where T : ShareData<T>
        {
            var type = typeof(T);
            if (_data_dict.TryGetValue(type, out var data))
                return data as T;
            data = type.GetConstructors()[0].Invoke(new object[]{});
            _data_dict[type] = data;
            return data as T;
        }

        public void ResetData<T> () where T : ShareData<T>
        {
            var type = typeof(T);
            _data_dict[type] = (T)type.GetConstructors()[0].Invoke(new object[]{});
        }

        public void RemoveData<T> () where T : ShareData<T>
        {
            var type = typeof(T);
            _data_dict.Remove(type);
        }

        public void ClearAllData (HashSet<Type> except_set = null)
        {
            var key_list = new List<Type>(_data_dict.Keys);
            foreach (var type in key_list)
            {
                if (except_set == null || !except_set.Contains(type))
                    _data_dict[type] = type.GetConstructors()[0].Invoke(new object[]{});
            }
        }

        protected override void on_tick (float dt)
        {
            var key_list = new List<Type>(_data_dict.Keys);
            foreach (var key in key_list)
            {
                if (_data_dict.TryGetValue(key, out var data))
                    data.GetType().GetMethod("Tick").Invoke(data, new System.Object[] { dt });
            }
        }
    }
}
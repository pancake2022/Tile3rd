using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework
{
    public class GameObjectPoolManager : Module<Framework>
    {
        private Dictionary<string, GameObjectPool> _pool_dict;
        private Transform _pool_root;

        protected override IEnumerator on_init (params object[] param_list)
        {
            _pool_dict = new Dictionary<string, GameObjectPool>();
            _pool_root = _main_module.Context.GameObjectPoolRoot;
            yield return null;
        }

        public GameObjectPool Preload (string prefab_path)
        {
            if (!_pool_dict.TryGetValue(prefab_path, out var pool))
            {
                var pool_obj = new GameObject(prefab_path);
                pool_obj.transform.SetParent(_pool_root);
                pool = Utils.GetOrCreateComponent<GameObjectPool>(pool_obj);
                pool.Init(_main_module.ResourcesManager, prefab_path);
                _pool_dict[prefab_path] = pool;
            }
            return pool;
        }

        public GameObject Spawn (string prefab_path, Transform parent)
        {
            return Preload(prefab_path).Spawn(parent).gameObject;
        }

        public void Recycle (GameObject obj)
        {
            var info = obj.GetComponent<PoolGameObjectInfo>();
            if (info && _pool_dict.TryGetValue(info.PrefabPath, out var pool))
                pool.Recycle(info);
            else
                Destroy(obj);
        }

        public void ClearPool ()
        {
            _main_module.ResetDrillTime();
            foreach (var pool in _pool_dict.Values)
                pool.Clear();
            _pool_dict.Clear();
            _main_module.DrillTime("Clear GameObjectPoolManager");
        }
    }
}
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework
{
    public interface IPoolableComponent
    {
        void OnSpawn();

        void OnRecycle();
    }

    public class PoolGameObjectInfo : CSBehaviour
    {
        public string PrefabPath;
    }

    public class GameObjectPool : CSBehaviour
    {
        private string _prefab_path;
        private GameObject _prefab;
        // private Queue<PoolGameObjectInfo> _object_queue;

        public void Init (ResourcesManager resources_manager, string prefab_path)
        {
            _prefab_path = prefab_path;
            var prefab = resources_manager.LoadResource<GameObject>(prefab_path);
            _prefab = GameObject.Instantiate(prefab, transform);
            // _object_queue = new Queue<PoolGameObjectInfo>();
        }

        public PoolGameObjectInfo Spawn (Transform parent)
        {
            PoolGameObjectInfo info;
            // if (_object_queue.Count > 0)
            // {
            //     info = _object_queue.Dequeue();
            //     info.transform.SetParent(parent);
            // }
            // else
            {
                var obj = GameObject.Instantiate(_prefab, parent);
                obj.name = Utils.RemoveClone(obj.name);
                info = Utils.GetOrCreateComponent<PoolGameObjectInfo>(obj);
                info.PrefabPath = _prefab_path;
            }

            var component = info.GetComponent<IPoolableComponent>();
            if (component != null)
                component.OnSpawn();
            
            // reset position
            // info.gameObject.transform.localPosition = _prefab.transform.localPosition;
            // info.gameObject.transform.localScale = _prefab.transform.localScale;
            // info.gameObject.transform.localRotation = _prefab.transform.localRotation;
            return info;
        }

        public void Recycle (PoolGameObjectInfo info)
        {
            Destroy(info.gameObject);
            // var component = info.GetComponent<IPoolableComponent>();
            // if (component != null)
            //     component.OnRecycle();
            // info.transform.SetParent(transform);
            // _object_queue.Enqueue(info);
        }

        public void Clear ()
        {
            Destroy(gameObject);
        }
    }
}
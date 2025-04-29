using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSFramework
{
    public class StorageManager : Module<Framework>
    {
        public Action PrepareSaveStorageCallback;
        public Action ResetStorageCallback;
        public static readonly string LocalStorageKey = "Storage";
        public static readonly string LocalStorageVersionKey = "StorageVersion";
        public static readonly string SyncStorageVersionKey = "SyncStorageVersion";
        public static readonly string SendSyncStorageVersionKey = "SendSyncStorageVersion";
        public static readonly string OnceKey = "Once";
        public static readonly float AutoSaveInterval = 1.0f;
        public static readonly string EmptyStorageContent = "{}";
        public ulong LocalStorageVersion { get; private set; }
        public ulong SyncStorageVersion { get; private set; }
        public ulong SendSyncStorageVersion { get; private set; }
        private Dictionary<string, Storage> _storage_dict;
        private bool _force_save = false;
        private bool _dirty = false;
        private float _escape_time = 0.0f;
        
        public T Storage<T> () where T : Storage
        {
            var storage_type = typeof(T);
            _storage_dict.TryGetValue(storage_type.Name, out var storage);
            return storage as T;
        }

        protected override IEnumerator on_init (params object[] param_list)
        {
            _storage_dict = new Dictionary<string, Storage>();
            init_storage_dict();
            load();
            yield return null;
        }

        protected void init_storage_dict ()
        {
            var registry = _main_module.Context.StorageRegistry;
            if (registry != null)
            {
                var storage_list = registry.GetRegisteredStorage();
                foreach (var storage in storage_list)
                    _storage_dict[storage.GetType().Name] = storage;
            }
            else
            {
                Logger.Error(string.Format("StorageManager: StorageRegistry is null"));
            }
        }

        protected override IEnumerator on_cleanup ()
        {
            _storage_dict = null;
            yield return null;
        }

        public void Dirty (bool force_save)
        {
            if (force_save)
                _force_save = true;
            _dirty = true;
        }

        protected override void on_tick(float dt)
        {
            _escape_time += Time.deltaTime;
            if (_force_save || _escape_time >= AutoSaveInterval)
            {
                if (_dirty)
                {
                    save();
                    _dirty = false;
                }

                _force_save = false;
                _escape_time = 0.0f;
            }
        }

        protected void save ()
        {
            PrepareSaveStorageCallback?.Invoke();
            var str_json = SerializeStorage();
            Utils.SafeSetString(LocalStorageKey, str_json);

            ++LocalStorageVersion;
            save_version();
        }

        protected void load ()
        {
            load_storage(Utils.SafeGetString(LocalStorageKey, EmptyStorageContent));

            load_version();
        }

        protected void load_storage (string str_json)
        {
            var setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;

            var json_obj = JObject.Parse(str_json);
            foreach (var item in _storage_dict)
            {
                var json_value = json_obj[item.Key];
                if (json_value != null)
                    JsonConvert.PopulateObject(json_value.ToString(), item.Value, setting);
            }
        }

        public T ParseStorage<T> (string str_json) where T : Storage
        {
            return ParseStorage<T>(JObject.Parse(str_json));
        }

        public T ParseStorage<T> (JObject json_obj) where T : Storage
        {
            var storage_type = typeof(T);
            var storage_obj = System.Activator.CreateInstance(storage_type) as T;
            
            var setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            var json_value = json_obj[storage_type.Name];
            if (json_value != null)
                JsonConvert.PopulateObject(json_value.ToString(), storage_obj, setting);

            return storage_obj;
        }

        protected void save_version ()
        {
            Utils.SafeSetString(LocalStorageVersionKey, LocalStorageVersion.ToString());
            Utils.SafeSetString(SyncStorageVersionKey, SyncStorageVersion.ToString());
            Utils.SafeSetString(SendSyncStorageVersionKey, SendSyncStorageVersion.ToString());
        }

        protected void load_version ()
        {
            var str_local_version = Utils.SafeGetString(LocalStorageVersionKey);
            if (ulong.TryParse(str_local_version, out var local_storage_version))
                LocalStorageVersion = local_storage_version;
            else
                LocalStorageVersion = 0;
            
            var str_sync_version = Utils.SafeGetString(SyncStorageVersionKey);
            if (ulong.TryParse(str_sync_version, out var sync_storage_version))
                SyncStorageVersion = sync_storage_version;
            else
                SyncStorageVersion = 0;

            var str_send_sync_version = Utils.SafeGetString(SendSyncStorageVersionKey);
            if (ulong.TryParse(str_send_sync_version, out var send_sync_storage_version))
                SendSyncStorageVersion = send_sync_storage_version;
            else
                SendSyncStorageVersion = 0;
        }

        public void ForceSave ()
        {
            save();
            _dirty = false;
        }

        public void ClearStorage ()
        {
            LocalStorageVersion = 0;
            SyncStorageVersion = 0;
            SendSyncStorageVersion = 0;
            ReplaceStorage(EmptyStorageContent);
        }

        public void ReplaceStorage (string str_json)
        {
            if (string.IsNullOrEmpty(str_json))
                str_json = EmptyStorageContent;
            _storage_dict.Clear();
            init_storage_dict();
            load_storage(str_json);
            save();
            ResetStorageCallback?.Invoke();
        }

        public string SerializeStorage ()
        {
            var setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.SerializeObject(_storage_dict, setting);
        }

        public void SetLocalStorageVersion (ulong version, bool immediately_save = true)
        {
            LocalStorageVersion = version;
            if (immediately_save)
                save_version();
        }

        public void SetSyncStorageVersion (ulong version, bool immediately_save = true)
        {
            SyncStorageVersion = version;
            if (immediately_save)
                save_version();
        }

        public void SetSendSyncStorageVersion (ulong version, bool immediately_save = true)
        {
            SendSyncStorageVersion = version;
            if (immediately_save)
                save_version();
        }
    }
}
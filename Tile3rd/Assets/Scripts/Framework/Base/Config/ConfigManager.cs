using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework
{
    public class ConfigManager : Module<Framework>
    {
        public ConfigRegistry Registry { get; private set; }
        private Dictionary<Type, object> _config_group_list_dict;

        public T ConfigGroup<T> (Predicate<T> match) where T : ConfigGroup
        {
            var config_list = ConfigGroupList<T>();
            foreach (var config in config_list)
            {
                if (match(config))
                    return config;
            }
            return null;
        }

        public T SingleConfigGroup<T>() where T : ConfigGroup
        {
            var config_list = ConfigGroupList<T>();
            foreach (var config in config_list)
                return config;
            return null;
        }

        public List<T> ConfigGroupList<T> () where T : ConfigGroup
        {
            var config_group_type = typeof(T);
            if (_config_group_list_dict.TryGetValue(config_group_type, out var config_group_list))
                return config_group_list as List<T>;
            return null;
        }

        public void ClearItem (ConfigRegistryItem item)
        {
            var config_group_type_name = $"CSFramework.{item.GroupClassName}";
            var config_group_type = Type.GetType(config_group_type_name);
            _config_group_list_dict.Remove(config_group_type);
        }

        public static object DeserializeConfigGroup (ConfigRegistryItem item, string str_json)
        {
            var config_group_type_name = $"CSFramework.{item.GroupClassName}";
            var config_group_type = Type.GetType(config_group_type_name);
            var config_group = System.Activator.CreateInstance(config_group_type);
            var table = JsonConvert.DeserializeObject<Dictionary<string, object>>(str_json);
            foreach (var property_item in item.TableToPropertyDict)
            {
                var property_type = Type.GetType($"System.Collections.Generic.List`1[CSFramework.{property_item.Value.PropertyType}]");
                if (table.TryGetValue(property_item.Key, out var table_value))
                {
                    var str_table_value = JsonConvert.SerializeObject(table_value);
                    var property_value = JsonConvert.DeserializeObject(str_table_value, property_type);
                    config_group_type.GetField(property_item.Value.PropertyName).SetValue(config_group, property_value);
                }
                else
                {
                    config_group_type.GetField(property_item.Value.PropertyName).SetValue(config_group, System.Activator.CreateInstance(property_type));
                }
            }
            return config_group;
        }

        public bool LoadItem (ConfigRegistryItem item, TextAsset text_asset = null)
        {
            if (text_asset == null)
                text_asset = _main_module.ResourcesManager.LoadResource<TextAsset>($"{_main_module.Context.ConfigJsonRoot}/{item.JsonPath.Replace(".json", "")}"); // todo remove magic

            if (text_asset)
            {
                try
                {
                    var config_group = DeserializeConfigGroup(item, text_asset.text);
                    var config_group_type = config_group.GetType();

                    if (!_config_group_list_dict.TryGetValue(config_group_type, out var config_group_list))
                    {
                        var config_group_list_type_name = $"System.Collections.Generic.List`1[{config_group_type}]";
                        var config_group_list_type = Type.GetType(config_group_list_type_name);
                        config_group_list = System.Activator.CreateInstance(config_group_list_type);
                        _config_group_list_dict.Add(config_group_type, config_group_list);
                    }
                    config_group_list.GetType().GetMethod("Add").Invoke(config_group_list, new object[] { config_group });
                    return true;
                }
                catch (Exception e)
                {
                    error($"LoadItem json exception: {e}");
                    return false;
                }
            }
            else
            {
                error($"LoadItem json failed: {item.JsonPath}");
                return false;
            }
        }

        protected override IEnumerator on_init (params object[] param_list)
        {
            _config_group_list_dict = new Dictionary<Type, object>();
            var registry_text = _main_module.ResourcesManager.LoadResource<TextAsset>(_main_module.Context.ConfigRegistryPath);
            if (registry_text == null)
            {
                error($"load registry failed: {_main_module.Context.ConfigRegistryPath}");
                yield break;
            }

            Registry = JsonConvert.DeserializeObject<ConfigRegistry>(registry_text.text);
            foreach (var registry_item in Registry.ItemDict.Values)
            {
                if (registry_item.LoadOnStart)
                    LoadItem(registry_item);
            }
        }

        protected override IEnumerator on_cleanup ()
        {
            _config_group_list_dict = null;
            yield return null;
        }

        public void InitFromEditor ()
        {
            _config_group_list_dict = new Dictionary<Type, object>();
        }
    }
}
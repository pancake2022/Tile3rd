using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework
{
    public class Config
    {
        public static bool operator true(Config a)
        {
            return a != null;
        }

        public static bool operator false(Config a)
        {
            return a == null;
        }

        protected List<T> create_config_list<T> (Hashtable table, string key)
        {
            var temp_object = table[key];
            var temp_object_str = JsonConvert.SerializeObject(temp_object);
            return JsonConvert.DeserializeObject<List<T>>(temp_object_str);
        }

        protected Dictionary<TKey, TValue> create_config_dict<TKey, TValue> (Hashtable table)
        {
            var temp_object_str = JsonConvert.SerializeObject(table);
            return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(temp_object_str);
        }

        public static bool TryReadInt (string[] data_list, int index, out int value)
        {
            if (index < data_list.Length)
            {
                if (int.TryParse(data_list[index], out value))
                    return true;
            }
            value = 0;
            return false;
        }

        public static bool TryReadFloat (string[] data_list, int index, out float value)
        {
            if (index < data_list.Length)
            {
                if (float.TryParse(data_list[index], out value))
                    return true;
            }
            value = 0;
            return false;
        }

        public static bool TryReadString (string[] data_list, int index, out string value)
        {
            if (index < data_list.Length)
            {
                value = data_list[index];
                return true;
            }
            value = null;
            return false;
        }
    }

    public class ConfigGroup
    {
        public static bool operator true(ConfigGroup a)
        {
            return a != null;
        }

        public static bool operator false(ConfigGroup a)
        {
            return a == null;
        }
    }

    public class ConfigRegistryItemProperty
    {
        public string PropertyName;
        public string PropertyType;
    }

    public class ConfigRegistryItem
    {
        public string JsonPath;
        public string GroupClassName;
        public Dictionary<string, ConfigRegistryItemProperty> TableToPropertyDict;
        public bool LoadOnStart;
    }

    public class ConfigRegistry
    {
        public Dictionary<string, ConfigRegistryItem> ItemDict = new Dictionary<string, ConfigRegistryItem>(); // <Path, ItemData>

        public ConfigRegistryItem RequireItem (string json_path, string group_class_name)
        {
            if (!ItemDict.TryGetValue(json_path, out var item))
            {
                item = new ConfigRegistryItem { JsonPath = json_path, GroupClassName = group_class_name, TableToPropertyDict = new Dictionary<string, ConfigRegistryItemProperty>(), LoadOnStart = false };
                ItemDict[json_path] = item;
            }

            return item;
        }
    }
}
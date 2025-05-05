using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using ExcelDataReader;

namespace CSFramework
{
    public class StorageExporter
    {
        private class StorageColumnHead
        {
            static public readonly char ListSeparator = '#';
            public string Key;
            public string Type;
            public string DefaultValue;
            public bool ForceSave;
            public string Desc;

            public StorageColumnHead (string key, string type, bool force_save, string desc, string default_value)
            {
                Key = key;
                Type = type; //.ToLower();
                ForceSave = force_save;
                Desc = desc;
                DefaultValue = default_value;
            }
        }

        private DataSet _data_set;

        private string _class_template;
        private string _base_property_template;
        private string _object_property_template;
        private string _registry_template;
        private string _registry_storage_template;

        public void Read (string file_path, string template_path, string registry_template_path)
        {
            using (var fs = File.OpenRead(file_path))
            {
                var excel_reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                _data_set = excel_reader.AsDataSet();
            }

            // load template
            var template_content = File.ReadAllText(template_path);

            _class_template = Utils.FilterContent(template_content, "#ClassStart", "#ClassEnd", true);
            _base_property_template = Utils.FilterContent(template_content, "#BasePropertyStart", "#BasePropertyEnd");
            _object_property_template = Utils.FilterContent(template_content, "#ObjectPropertyStart", "#ObjectPropertyEnd");
            
            var registry_template_content = File.ReadAllText(registry_template_path);
            _registry_template = Utils.FilterContent(registry_template_content, "StorageStart", "StorageEnd", true);
            _registry_storage_template = Utils.FilterContent(registry_template_content, "StorageStart", "StorageEnd");
        }

        public void Export (string out_root)
        {
            var storage_class_dict = new Dictionary<string, string>();
            var storage_class_set = new HashSet<string>();
            foreach (DataTable table in _data_set.Tables)
                storage_class_set.Add(table.TableName);

            foreach (DataTable table in _data_set.Tables)
            {
                if (table.TableName == "Registry")
                {
                    var storage_list_content = "";
                    var start_col_index = 1; // 第一列是注释
                    for (var col_index = start_col_index; col_index < table.Columns.Count; ++col_index)
                    {
                        var storage_name = table.Rows[0][col_index] + "Storage";
                        var storage_desc = table.Rows[1][col_index].ToString();
                        storage_list_content += _registry_storage_template
                            .Replace("${StorageName}", storage_name)
                            .Replace("${Desc}", storage_desc.Replace('\n', ' '));
                    }
                    var class_content = _registry_template.Replace("${StorageList}", storage_list_content);
                    storage_class_dict.Add("StorageRegistry", class_content);
                    continue;
                }

                var row_count = table.Rows.Count;
                var col_count = table.Columns.Count;
                if (row_count >= 4)
                {
                    // init table head
                    var col_key_list = table.Rows[0];
                    var col_type_list = table.Rows[1];
                    var col_desc_list = table.Rows[2];
                    var col_force_save_list = table.Rows[3];
                    var col_default_value_list = table.Rows[4];

                    var head_list = new List<StorageColumnHead>();
                    var start_col_index = 1; // 第一列是注释
                    for (var col_index = start_col_index; col_index < col_count; ++col_index)
                    {
                        var col_key = col_key_list[col_index].ToString();
                        var col_type = col_type_list[col_index].ToString();
                        var col_desc = col_desc_list[col_index].ToString();
                        var col_force_save = col_force_save_list[col_index].ToString();
                        var col_default_value = col_default_value_list[col_index].ToString();

                        if (!string.IsNullOrEmpty(col_key) && !string.IsNullOrEmpty(col_type))
                        {
                            var force_save = false;
                            if (!string.IsNullOrEmpty(col_force_save))
                                bool.TryParse(col_force_save, out force_save);

                            var head = new StorageColumnHead(col_key, col_type, force_save, col_desc, col_default_value);
                            head_list.Add(head);
                        }
                        else
                        {
                            Logger.Error(string.Format("[{0}] col [{1}] head error", table.TableName, col_index));
                        }
                    }

                    var all_property_content = "";
                    foreach (var head in head_list)
                    {
                        var str_type = Utils.ConvertType(head.Type, out var is_object, "Storage", storage_class_set);
                        var str_template = is_object ? _object_property_template : _base_property_template;
                        string property_content = str_template
                            .Replace("${PropertyName}", head.Key)
                            .Replace("${PropertyPrivateName}", "_" + head.Key.ToLower())
                            .Replace("${PropertyDefaultValue}", head.DefaultValue.ToLower())
                            .Replace("${PropertyType}", str_type)
                            .Replace("${ForceSave}", head.ForceSave ? "true" : "")
                            .Replace("${Desc}", head.Desc.Replace('\n', ' '));
                        all_property_content += property_content;
                    }

                    var class_name = table.TableName + "Storage";
                    var class_content = _class_template
                            .Replace("${ClassName}", class_name)
                            .Replace("${ClassContent}", all_property_content);
                    storage_class_dict.Add(class_name, class_content);
                }
            }

            if (Directory.Exists(out_root))
                Directory.Delete(out_root, true);
            Directory.CreateDirectory(out_root);

            foreach (var item in storage_class_dict)
            {
                var storage_file_path = string.Format("{0}/{1}.cs", out_root, item.Key);
                using (var fs = new FileStream(storage_file_path, FileMode.CreateNew))
                {
                    using (var tw = new StreamWriter(fs))
                    {
                        tw.Write(item.Value);
                    }
                }
            }
        }
    }
}
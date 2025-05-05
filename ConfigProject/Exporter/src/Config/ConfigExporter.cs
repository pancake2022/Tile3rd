using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using ExcelDataReader;

namespace CSFramework
{
    public class ConfigExporter
    {
        private class ConfigColumnHead
        {
            public string Key;
            public string Type;
            public string Desc;

            public ConfigColumnHead (string key, string type, string desc)
            {
                Key = key;
                Type = type; //.ToLower();
                Desc = desc;
            }
        }

        private DataSet _data_set;
        private string _class_template;
        private string _property_template;

        public void Read (string file_path, string template_content)
        {
            using (var fs = File.OpenRead(file_path))
            {
                var excel_reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                _data_set = excel_reader.AsDataSet();
            }

            _class_template = Utils.FilterContent(template_content, "PropertyStart", "PropertyEnd", true);
            _property_template = Utils.FilterContent(template_content, "PropertyStart", "PropertyEnd");
        }

        public void Export (string out_json_path, string out_code_root)
        {
            var table_object = new Dictionary<string, object>();
            var config_class_dict = new Dictionary<string, string>();
            foreach (DataTable table in _data_set.Tables)
            {
                var row_count = table.Rows.Count;
                var col_count = table.Columns.Count;
                if (row_count > 3)
                {
                    // init table head
                    var col_key_list = table.Rows[0];
                    var col_type_list = table.Rows[1];
                    var col_desc_list = table.Rows[2];
                    var head_dict = new Dictionary<int, ConfigColumnHead>();
                    var col_start_index = 1; // 第一列为描述
                    for (var col_index = col_start_index; col_index < col_count; ++col_index)
                    {
                        var col_key = col_key_list[col_index].ToString();
                        var col_type = col_type_list[col_index].ToString();
                        var col_desc = col_desc_list[col_index].ToString();

                        if (col_type.ToLower() == "desc" || col_type.ToLower() == "note" || col_type.ToLower() == "ignore")
                            continue;

                        if (!string.IsNullOrEmpty(col_key) && !string.IsNullOrEmpty(col_type))
                        {
                            var head = new ConfigColumnHead(col_key, col_type, col_desc);
                            head_dict.Add(col_index, head);
                        }
                        else
                        {
                            Logger.Error(string.Format("[{0}] col [{1}] head error", table.TableName, col_index));
                        }
                    }

                    // var table_object = new Dictionary<string, Dictionary<string, object>>();
                    for (var row_index = 3; row_index < row_count; ++row_index)
                    {
                        var row_obj = new Dictionary<string, object>();
                        var row_key = "";
                        for (var col_index = 0; col_index < col_count; ++col_index)
                        {
                            if (head_dict.TryGetValue(col_index, out var head))
                            {
                                var value = Utils.ConvertValue(head.Type, table.Rows[row_index][col_index]);
                                if (value != null)
                                {
                                    row_obj[head.Key] = value;
                                    if (string.IsNullOrEmpty(row_key))
                                        row_key = value.ToString();
                                }
                            }
                        }
                        table_object.Add(row_key, row_obj);
                    }
                    // table_dict.Add(table.TableName, table_object);

                    var all_property_content = "";
                    foreach (var head in head_dict.Values)
                    {
                        var str_type = Utils.ConvertType(head.Type, out var is_object);
                        var str_template = _property_template;
                        string property_content = str_template
                            .Replace("${PropertyName}", head.Key)
                            .Replace("${PropertyType}", str_type)
                            .Replace("${Desc}", head.Desc.Replace('\n', ' '));
                        all_property_content += property_content;
                    }

                    var class_name = table.TableName + "Config";
                    var class_content = _class_template
                            .Replace("${ClassName}", class_name)
                            .Replace("${ClassContent}", all_property_content);
                    config_class_dict.Add(class_name, class_content);

                    break; // todo，目前只导出第一个表，后面的表之后作为映射关系，以支持树形结构的配置表
                }
            }

            Utils.SaveFile(out_json_path, JsonConvert.SerializeObject(table_object, Formatting.Indented));
            foreach (var item in config_class_dict)
            {
                Utils.SaveFile(string.Format("{0}/{1}.cs", out_code_root, item.Key), item.Value);
            }
        }
    }
}
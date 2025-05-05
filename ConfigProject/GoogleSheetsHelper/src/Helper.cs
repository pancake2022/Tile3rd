using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CSFramework;
using System.Text.RegularExpressions;

public class TableHead 
{
    public string Name;
    public string Type;
    public string Note;

    // 优化排序
    public int OriginIndex;
    public int SortedIndex;

    public static int CompareHead (string h1, string h2)
    {
        var h1_pure_str = separate_number(h1, out var h1_number_str);
        var h2_pure_str = separate_number(h2, out var h2_number_str);
        if (h1_pure_str == h2_pure_str && int.TryParse(h1_number_str, out var h1_number) && int.TryParse(h2_number_str, out var h2_number))
        {
            return h1_number == h2_number ? 0 : (h1_number > h2_number ? 1 : -1);
        }
        else
        {
            return h1_pure_str.CompareTo(h2_pure_str);
        }
    }

    public static int CompareHead (TableHead h1, TableHead h2)
    {
        return CompareHead(h1.Name, h2.Name);
    }

    private static bool is_number (char c)
    {
        return c >= '0' && c <= '9';
    }

    private static string separate_number (string s, out string s_number)
    {
        s_number = "";
        for (var i = s.Length - 1; i >= 0; --i)
        {
            var c = s[i];
            if (is_number(c))
                s_number = c + s_number;
            else
                return s.Substring(0, i + 1);
        }
        return "";
    }
}

public class TempClass
{
    private class TempClassProperty
    {
        public string Name;
        public string Type;
        public string Note;
    }

    private string _class_name;
    private string _class_template;
    private string _property_template;
    private Dictionary<string, TempClassProperty> _property_dict;

    public TempClass (string class_name, string class_template, string property_template)
    {
        _class_name = class_name;
        _class_template = class_template;
        _property_template = property_template;
        _property_dict = new Dictionary<string, TempClassProperty>();
    }

    public void AppendProperty (string property_name, string property_type, string note)
    {
        // check property_name
        var first_char = property_name[0];
        if ((first_char >= 'a' && first_char <= 'z') || (first_char >= 'A' && first_char <= 'Z'))
        {
            if (_property_dict.TryGetValue(property_name, out var property))
            {
                // check type
                if (Helper.CompareStrType(property_type, property.Type))
                    property.Type = property_type;
            }
            else
            {
                _property_dict[property_name] = new TempClassProperty
                {
                    Name = property_name,
                    Type = property_type,
                    Note = note,
                };
            }
            if (!_property_dict.ContainsKey(property_name))
            {
                _property_dict[property_name] = new TempClassProperty
                {
                    Name = property_name,
                    Type = property_type,
                    Note = note,
                };
            }
        }
    }

    public string ExportContent ()
    {
        var property_content = "";
        foreach (var property in _property_dict.Values)
        {
            property_content += _property_template
                .Replace("${PropertyName}", property.Name)
                .Replace("${PropertyType}", Utils.ConvertType(property.Type, out var is_object))
                .Replace("${Desc}", property.Note.Replace('\n', ' '));
        }
        return _class_template
            .Replace("${ClassName}", _class_name)
            .Replace("${ClassContent}", property_content);
    }
}

public static class Helper
{
    public static readonly string ApplicationName = "ProjectTile3rd";
    public static readonly string CredentialsPath = "res/credentials.json";
    public static readonly string TokenPath = "res/token.json";
    public static readonly string CachePath = "cache/cache.json";
    public static readonly string ConfigRegistryPath = "res/tile3rd/Common/registry.json";
    public static readonly string[] Scopes = { SheetsService.Scope.Drive, SheetsService.Scope.DriveFile, SheetsService.Scope.Spreadsheets, SheetsService.Scope.SpreadsheetsReadonly };

    public static UserCredential LoadCredential ()
    {
        using (var s = new FileStream("res/credentials.json", FileMode.Open, FileAccess.Read))
        {
            var token_path = "res/token.json";
            return GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(s).Secrets, 
                Scopes, 
                "user", 
                CancellationToken.None,
                new FileDataStore(token_path, true)).Result;
        }
    }

    public static SheetsService CreateService ()
    {
        var credential = LoadCredential();
        return new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    public static void ReadFromRemote (string res_root, string out_cs_root, string limit_sub_dir_name = null)
    {
        var cache = HelperCache.LoadOrCreate(CachePath);
        var service = CreateService();

        var registry_content = File.Exists(ConfigRegistryPath) ? File.ReadAllText(ConfigRegistryPath) : "{}";
        var registry = JsonConvert.DeserializeObject<ConfigRegistry>(registry_content);

        var template_content = File.ReadAllText("src/Config/ConfigTemplate.template");
        var class_template = Utils.FilterContent(template_content, "PropertyStart", "PropertyEnd", true);
        var property_template = Utils.FilterContent(template_content, "PropertyStart", "PropertyEnd");

        var group_template_content = File.ReadAllText("src/Config/ConfigGroupTemplate.template");
        var group_class_template = Utils.FilterContent(group_template_content, "PropertyStart", "PropertyEnd", true);
        var group_property_template = Utils.FilterContent(group_template_content, "PropertyStart", "PropertyEnd");
        
        var class_path_to_temp_class_dict = new Dictionary<string, TempClass>();
        var config_path_list = new List<string>();

        var request_times = 0;
        foreach (var item in cache.FilePathToSheetID)
        {
            try
            {
                var file_path = item.Key;
                if (!string.IsNullOrEmpty(limit_sub_dir_name) && !file_path.StartsWith(limit_sub_dir_name))
                    continue;

                var sheet_id = item.Value;

                // get property
                var title_list = new List<string>();
                var range_list = new List<string>();
                var property_req = service.Spreadsheets.Get(sheet_id);
                var property_resp = property_req.Execute();
                ++request_times;
                var sheet_count = property_resp.Sheets.Count;
                for (var i = 0; i < sheet_count; ++i)
                {
                    var properties = property_resp.Sheets[i].Properties;
                    var grid_properties = properties.GridProperties;
                    title_list.Add(properties.Title);
                    range_list.Add($"{properties.Title}!1:{grid_properties.RowCount}");
                }

                // get data
                var get_value_req = service.Spreadsheets.Values.BatchGet(sheet_id);
                get_value_req.Ranges = range_list;
                var get_value_resp = get_value_req.Execute();
                ++request_times;

                var total_table = new Dictionary<string, object>();
                var group_name = Utils.ToTableName(Path.GetFileNameWithoutExtension(file_path));
                var group_class_name = Utils.ToTableName($"{group_name}ConfigGroup");
                // var group_class_path = $"{group_name}/{group_class_name}";
                var group_class_path = $"{group_class_name}";
                var registry_item = registry.RequireItem(file_path, group_class_name);
                if (!class_path_to_temp_class_dict.TryGetValue(group_class_path, out var group_temp_class))
                {
                    group_temp_class = new TempClass(group_class_name, group_class_template, group_property_template);
                    class_path_to_temp_class_dict[group_class_path] = group_temp_class;
                }
                // var group_property_set = new HashSet<string>();

                for (var i = 0; i < sheet_count; ++i)
                {
                    var title = title_list[i];
                        
                    var sheet_data = get_value_resp.ValueRanges[i];
                    var row_data_list = sheet_data.Values;
                    if (row_data_list == null)
                        continue;

                    var single_table_row_count = 4; // 暂时这么判定, 4列的为singletable
                    var is_single_table = false;
                    var first_row = row_data_list[0];
                    if (first_row.Count == single_table_row_count) // 是否是竖型单表
                    {
                        var first_row_type = first_row[1].ToString();
                        var first_row_value = first_row[3];

                        var str_value = Utils.ConvertValue(first_row_type, first_row_value) as string;
                        if (str_value != "unknown")
                            is_single_table = true;
                    }
                    
                    var sheet_table = new List<Dictionary<string, object>>();
                    var head_list = new List<TableHead>();
                    var row_count = row_data_list.Count;
                    if (is_single_table || row_count >= 3)
                    {
                        total_table.Add(title, sheet_table);

                        if (is_single_table)
                        {
                            // generate head
                            var col_data = new List<string>();
                            foreach (var row_data in row_data_list)
                            {
                                head_list.Add(new TableHead
                                {
                                    Name = row_data[0].ToString(),
                                    Type = row_data[1].ToString(),
                                    Note = row_data[2].ToString(),
                                });
                                col_data.Add(row_data.Count < 4 ? "" : row_data[3].ToString());
                            }

                            // generate json
                            var row_table = new Dictionary<string, object>();
                            sheet_table.Add(row_table);

                            for (var row_index = 0; row_index < col_data.Count; ++row_index)
                            {
                                var head = head_list[row_index];
                                var cell_data = col_data[row_index];
                                
                                var value = Utils.ConvertValue(head.Type, cell_data);
                                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                                    row_table.Add(head.Name, value);
                            }
                        }
                        else
                        {
                            // generate head
                            var col_name_list = row_data_list[0];
                            var col_type_list = row_data_list[1];
                            var col_note_list = row_data_list[2];
                            for (var index = 0; index < col_name_list.Count; ++index)
                            {
                                head_list.Add(new TableHead
                                {
                                    Name = col_name_list[index].ToString(),
                                    Type = index < col_type_list.Count ? col_type_list[index].ToString() : "",
                                    Note = index < col_note_list.Count ? col_note_list[index].ToString() : "",
                                    OriginIndex = index,
                                });
                            }

                            // sort and refresh index
                            head_list.Sort(TableHead.CompareHead);
                            for (var index = 0; index < head_list.Count; ++index)
                                head_list[index].SortedIndex = index;

                            // generate json
                            for (var row_index = 3; row_index < row_count; ++row_index)
                            {
                                var row_data = row_data_list[row_index];
                                var row_table = new Dictionary<string, object>();
                                sheet_table.Add(row_table);

                                // for (var col_index = 0; col_index < row_data.Count; ++col_index)
                                for (var head_index = 0; head_index < head_list.Count; ++head_index) // 根据排序后的headlist进行检索
                                {
                                    var head = head_list[head_index];
                                    var col_index = head.OriginIndex;
                                    if (col_index >= row_data.Count)
                                        continue;

                                    // var head = head_list.Find(a => a.OriginIndex == col_index);
                                    // var head = head_list[col_index];
                                    var cell_data = row_data[col_index];
                                    
                                    if (CheckFlex(head, head_list, out var flex_property_name, out var flex_end_head))
                                    {
                                        // Console.WriteLine($"Flex: {flex_property_name}={col_index}-{flex_end_index} {row_data.Count}");
                                        var flex_list = new List<object>();
                                        row_table.Add(flex_property_name, flex_list);
                                        for (var flex_head_index = head_index; flex_head_index <= flex_end_head.SortedIndex; ++flex_head_index)
                                        {
                                            var flex_head = head_list[flex_head_index];
                                            // var flex_cell_Data = row_data[flex_head_index];
                                            if (flex_head.OriginIndex >= row_data.Count)
                                                break;

                                            var flex_cell_Data = row_data[flex_head.OriginIndex];
                                            flex_list.Add(Utils.ConvertValue(flex_head.Type, flex_cell_Data.ToString()));
                                        }
                                        // col_index = flex_end_index;
                                        head_index = flex_end_head.SortedIndex;
                                    }
                                    else
                                    {
                                        var value = Utils.ConvertValue(head.Type, cell_data.ToString());
                                        if (value != null && !string.IsNullOrEmpty(value.ToString()))
                                            row_table.Add(head.Name, value);
                                    }
                                }
                            }
                        }

                        // generate class
                        var raw_class_name = Regex.Replace(title, @"\d", "");
                        var class_name = Utils.ToTableName($"{raw_class_name}Config");
                        var class_path = $"{group_name}/{class_name}";
                        if (!class_path_to_temp_class_dict.TryGetValue(class_path, out var temp_class))
                        {
                            temp_class = new TempClass(class_name, class_template, property_template);
                            class_path_to_temp_class_dict[class_path] = temp_class;
                        }

                        for (var head_i = 0; head_i < head_list.Count; ++head_i)
                        {
                            var head = head_list[head_i];
                            if (!head.Name.ToLower().StartsWith("note")) // todo
                            {
                                if (CheckFlex(head, head_list, out var flex_property_name, out var flex_end_head))
                                {
                                    temp_class.AppendProperty(flex_property_name, $"list<{head.Type}>", head.Note);
                                    head_i = flex_end_head.SortedIndex;
                                }
                                else
                                {
                                    temp_class.AppendProperty(head.Name, head.Type, head.Note);
                                }
                            }
                        }

                        // fill group class property
                        var group_property_name = Utils.ToTableName($"{title}ConfigList");
                        group_temp_class.AppendProperty(group_property_name, $"List<{class_name}>", title);

                        registry_item.TableToPropertyDict[title] = new ConfigRegistryItemProperty
                        {
                            PropertyName = group_property_name,
                            PropertyType = class_name,
                        };
                    }
                }

                // if (!class_path_to_temp_class_dict.ContainsKey(group_class_path))
                // {
                //     var group_class_content = group_class_template
                //         .Replace("${ClassName}", group_class_name)
                //         .Replace("${ClassContent}", group_class_property_content);
                    
                //     class_path_to_temp_class_dict[group_class_path] = group_class_content;
                //     Console.WriteLine($"Fill group class {group_class_name} into {file_path}");
                // }

                var save_json_path = $"{res_root}/{file_path}";
                Utils.SaveFile(save_json_path, JsonConvert.SerializeObject(total_table, Formatting.Indented));
                Console.WriteLine($"{save_json_path} Save Finished");

                // break; // todo

                if (request_times >= 20) // 限制每分钟只能发送20个请求 by GoogleAPI limit
                {
                    Console.WriteLine($"request_times = {request_times}, start sleep");
                    request_times = 0;
                    Thread.Sleep(30000);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{item.Key} Exception: {e}");
            }
        }

        // class_dict
        foreach (var item in class_path_to_temp_class_dict)
        {
            var save_cs_path = $"{out_cs_root}/{item.Key}.cs";
            Utils.SaveFile(save_cs_path, item.Value.ExportContent());
            Console.WriteLine($"{save_cs_path} Save Finished");
        }

        // registry
        Utils.SaveFile(ConfigRegistryPath, JsonConvert.SerializeObject(registry, Formatting.Indented));
    }

    private static string GeneratePropertyContent (this string property_template, TableHead head)
    {
        return property_template
            .Replace("${PropertyName}", head.Name)
            .Replace("${PropertyType}", Utils.ConvertType(head.Type, out var is_object))
            .Replace("${Desc}", head.Note.Replace('\n', ' '));
    }

    private static bool CheckFlex (TableHead head, List<TableHead> head_list, out string flex_property_name, out TableHead flex_end_head)
    {
        var last_char = head.Name[head.Name.Length - 1];
        if (last_char == '1')
        {
            flex_property_name = head.Name.Substring(0, head.Name.Length - 1);
            var next_index = head.SortedIndex + 1;
            var flex_count = 1;

            while (next_index < head_list.Count)
            {
                var next_head = head_list[next_index];
                // var next_head = head_list.Find(a => a.OriginIndex == next_index);
                if (next_head.Name == $"{flex_property_name}{flex_count + 1}")
                {
                    ++next_index;
                    ++flex_count;
                }
                else
                {
                    break;
                }
            }
            if (flex_count > 1)
            {
                flex_end_head = head_list[head.SortedIndex + flex_count - 1];
                return true;
            }
        }
        flex_property_name = null;
        flex_end_head = head;
        return false;
    }

    public class LocalizeConfig
    {
        public string Key;
        public string Value;
    }

    public static void WriteLocalizeToRemote ()
    {
        var cache = HelperCache.LoadOrCreate(CachePath);
        var service = CreateService();

        var res_root_full_path = Path.GetFullPath("res/tile3rd");
        Console.WriteLine(res_root_full_path);
        var file_suffix = ".json";
        var file_list = Utils.GetFileList($"{res_root_full_path}/LocalizeConfig");
        var localize_dict = new Dictionary<string, List<LocalizeConfig>>();
        foreach (var file in file_list)
        {
            try
            {
                if (file.Extension != ".json")
                    continue;

                var relative_path = Path.GetRelativePath(res_root_full_path, file.FullName);
                if (!cache.FilePathToSheetID.ContainsKey(relative_path))
                {
                    var config_list = JsonConvert.DeserializeObject<List<LocalizeConfig>>(File.ReadAllText(file.FullName));

                    var spreadsheet = new Spreadsheet();
                    spreadsheet.Properties = new SpreadsheetProperties();
                    spreadsheet.Properties.Title = "LocalizeConfig/localize"; //relative_path.Replace(file_suffix, "");
                    spreadsheet.Sheets = new List<Sheet>();

                    var sheet = new Sheet();
                    sheet.Properties = new SheetProperties
                    {
                        Title = "Localize"
                    };
                    var grid_data = new GridData();

                    grid_data.RowData = new List<RowData>();
                    
                    // title row
                    var title_row = new RowData { Values = new List<CellData>() };
                    grid_data.RowData.Add(title_row);
                    // type row
                    var type_row = new RowData { Values = new List<CellData>() };
                    grid_data.RowData.Add(type_row);
                    // note row
                    var note_row = new RowData { Values = new List<CellData>() };
                    grid_data.RowData.Add(note_row);

                    foreach (var config in config_list)
                    {
                        var row = new RowData { Values = new List<CellData>
                        {
                            new CellData
                            {
                                UserEnteredValue = new ExtendedValue
                                {
                                    StringValue = config.Key,
                                },
                            },
                            new CellData
                            {
                                UserEnteredValue = new ExtendedValue
                                {
                                    StringValue = config.Value,
                                },
                            },
                        } };
                        grid_data.RowData.Add(row);
                    }

                    sheet.Data = new List<GridData>{ grid_data };
                    spreadsheet.Sheets.Add(sheet);
                    
                    var req = service.Spreadsheets.Create(spreadsheet);
                    var resp = req.Execute();

                    cache.FilePathToSheetID[relative_path] = resp.SpreadsheetId;

                    Console.WriteLine($"{relative_path} create resp: {resp.SpreadsheetUrl}");
                }
                break;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        cache.Save();
    }

    public static void WriteToRemote (string res_root)
    {
        var cache = HelperCache.LoadOrCreate(CachePath);
        var service = CreateService();

        var request_times = 0;

        var res_root_full_path = Path.GetFullPath(res_root);
        Console.WriteLine(res_root_full_path);
        var file_suffix = ".json";
        var file_list = Utils.GetFileList(res_root_full_path, except_subdir_set: new HashSet<string> { "MagicMapConfig", "LocalizeConfig" } , filter_extension_set: new HashSet<string> {file_suffix});
        foreach (var file in file_list)
        {
            try
            {
                var relative_path = Path.GetRelativePath(res_root_full_path, file.FullName);
                if (relative_path == "Common/registry.json")
                    continue;

                var table = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(file.FullName));

                if (cache.FilePathToSheetID.TryGetValue(relative_path, out var sheet_id))
                {
                    // var body = new BatchUpdateValuesRequest();
                    // // update
                    // var req = service.Spreadsheets.Values.BatchUpdate(body, sheet_id);
                    // body.Data = new List<ValueRange>();
                    // foreach (var key in table.Keys)
                    // {
                    //     var jarray = table[key] as JArray;
                    //     var value_range = new ValueRange();
                    //     body.Data.Add(value_range);
                    //     value_range.Values = new List<IList<object>>();
                    //     foreach (JObject jobj in jarray)
                    //     {
                    //         var value = new List<object>();
                    //         foreach (var jobj_value in jobj)
                    //             value.Add(jobj_value);

                    //         value_range.Values.Add(value);
                    //     }
                    //     value_range.Range = $"{key}!1:{jarray.Count}";
                    // }

                    // var resp = req.Execute();

                    // Console.WriteLine(resp);

                    // Console.WriteLine($"{relative_path} update resp: {resp}");
                    // ++request_times;
                }
                else
                {
                    // create
                    var spreadsheet = TableToSpreadsheet(relative_path.Replace(file_suffix, ""), table);
                    var req = service.Spreadsheets.Create(spreadsheet);
                    var resp = req.Execute();

                    cache.FilePathToSheetID[relative_path] = resp.SpreadsheetId;

                    Console.WriteLine($"{relative_path} create resp: {resp.SpreadsheetUrl}");
                    ++request_times;

                    // break; // todo
                }

                if (request_times >= 20) // 限制每分钟只能发送20个请求 by GoogleAPI limit
                {
                    Console.WriteLine($"request_times = {request_times}, start sleep");
                    request_times = 0;
                    Thread.Sleep(30000);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        cache.Save();
    }

    public static Spreadsheet TableToSpreadsheet (string name, Dictionary<string, object> table)
    {
        var spreadsheet = new Spreadsheet();
        spreadsheet.Properties = new SpreadsheetProperties();
        spreadsheet.Properties.Title = name;
        spreadsheet.Sheets = new List<Sheet>();
        foreach (var key in table.Keys)
        {
            var jarray = table[key] as JArray;

            var sheet = new Sheet();
            sheet.Properties = new SheetProperties
            {
                Title = key.ToString()
            };
            var grid_data = new GridData();
            grid_data.RowData = new List<RowData>();
            if (jarray.Count > 0)
            {
                if (jarray.Count == 1) // single table
                {
                    var first_jobj = jarray[0] as JObject;
                    foreach (var jobj_value in first_jobj)
                    {
                        var row_data = new RowData();
                        row_data.Values = new List<CellData>();

                        var name_col = new CellData
                        {
                            UserEnteredValue = new ExtendedValue
                            {
                                StringValue = Utils.ToTableName(jobj_value.Key.ToString()),
                            }
                        };
                        row_data.Values.Add(name_col);

                        var type_col = new CellData
                        {
                            UserEnteredValue = new ExtendedValue
                            {
                                StringValue = JTokenToType(jobj_value.Value),
                            }
                        };
                        row_data.Values.Add(type_col);

                        var note_col = new CellData
                        {
                            UserEnteredValue = new ExtendedValue
                            {
                                StringValue = "note",
                            }
                        };
                        row_data.Values.Add(note_col);

                        var value_col = new CellData
                        {
                            UserEnteredValue = JTokenToExtendedValue(jobj_value.Value)
                        };
                        row_data.Values.Add(value_col);

                        grid_data.RowData.Add(row_data);
                    }
                }
                else
                {
                    var is_timeline_table = key == "timeline"; // todo remove magic

                    // title row
                    var title_row = new RowData { Values = new List<CellData>() };
                    grid_data.RowData.Add(title_row);
                    // type row
                    var type_row = new RowData { Values = new List<CellData>() };
                    grid_data.RowData.Add(type_row);
                    // note row
                    var note_row = new RowData { Values = new List<CellData>() };
                    grid_data.RowData.Add(note_row);

                    var property_name_dict = new Dictionary<string, KeyValuePair<string, JToken>>();
                    foreach (JObject jobj_i in jarray)
                    {
                        // var p_index = 0;
                        foreach (var jobj_value in jobj_i)
                        {
                            // 跳过没有有效值的属性
                            if (jobj_value.Value.Type == JTokenType.Null || (jobj_value.Value.Type == JTokenType.String && string.IsNullOrEmpty(jobj_value.Value.ToString())))
                                continue;

                            if (property_name_dict.TryGetValue(jobj_value.Key, out var old_jobj_value))
                            {
                                // 比较是否需要升级
                                if (CompareJTokenType(jobj_value.Value, old_jobj_value.Value))
                                {
                                    property_name_dict[jobj_value.Key] = jobj_value;
                                }
                            }
                            else
                            {
                                property_name_dict[jobj_value.Key] = jobj_value;
                            }
                        }
                    }

                    foreach (var property_name in property_name_dict.Keys)
                    {
                        var jobj_value = property_name_dict[property_name];
                        title_row.Values.Add(new CellData
                        {
                            UserEnteredValue = new ExtendedValue
                            {
                                StringValue = Utils.ToTableName(jobj_value.Key.ToString()),
                            }
                        });
                        type_row.Values.Add(new CellData
                        {
                            UserEnteredValue = new ExtendedValue
                            {
                                StringValue = is_timeline_table && property_name.StartsWith("Items") ? "TimelineItem" : JTokenToType(jobj_value.Value),
                            }
                        });
                        note_row.Values.Add(new CellData
                        {
                            UserEnteredValue = new ExtendedValue
                            {
                                StringValue = "note",
                            }
                        });
                    }

                    // data row
                    foreach (JObject jobj in jarray)
                    {
                        var row_data = new RowData();
                        row_data.Values = new List<CellData>();
                        foreach (var property_name in property_name_dict.Keys)
                        {
                            if (jobj.TryGetValue(property_name, out var row_value))
                            {
                                row_data.Values.Add(new CellData { UserEnteredValue = is_timeline_table && property_name.StartsWith("Items") ? ItemToExtendedValue(row_value) : JTokenToExtendedValue(row_value) } );
                            }
                            else
                            {
                                row_data.Values.Add(new CellData());
                            }
                        }
                        
                        grid_data.RowData.Add(row_data);
                    }
                }
            }

            sheet.Data = new List<GridData>
            {
                grid_data
            };

            spreadsheet.Sheets.Add(sheet);
        }

        return spreadsheet;
    }

    public static ExtendedValue JTokenToExtendedValue (JToken jt)
    {
        var value = new ExtendedValue();
        var type = jt.Type;
        if (type == JTokenType.Array)
        {
            var jarray = jt as JArray;
            var str_array = new List<string>();
            foreach (var ji in jarray)
                str_array.Add(ji.ToString());
            value.StringValue = string.Join(',', str_array);
        }
        else if (type == JTokenType.Integer)
        {
            value.NumberValue = (int)jt;
        }
        else if (type == JTokenType.Float)
        {
            value.NumberValue = (float)jt;
        }
        else if (type == JTokenType.Boolean)
        {
            value.BoolValue = (bool)jt;
        }
        else if (type == JTokenType.Null)
        {
            value.StringValue = null;
        }
        else // to string
        {
            value.StringValue = jt.ToString();
        }
        return value;
    }

    public static ExtendedValue ItemToExtendedValue (JToken jt)
    {
        var str = JsonConvert.SerializeObject(jt);
        var item = JsonConvert.DeserializeObject<TimelineItem>(str);
        return new ExtendedValue
        {
            StringValue = TimelineItem.ToStr(item),
        };
    }

    public static string JTokenToType (JToken jt)
    {
        var type = jt.Type;
        if (type == JTokenType.Array)
        {
            var jarray = jt as JArray;

            JToken max_ji = null;
            foreach (var ji in jarray)
            {
                if (max_ji == null || CompareJTokenType(ji, max_ji))
                    max_ji = ji;
            }

            var subtype = max_ji != null ? JTokenToType(max_ji) : "";
            if (string.IsNullOrEmpty(subtype))
                return "list";
            else
                return $"list<{subtype}>";
        }
        else if (type == JTokenType.Integer)
        {
            return "int";
        }
        else if (type == JTokenType.Float)
        {
            return "float";
        }
        else if (type == JTokenType.Boolean)
        {
            return "bool";
        }
        else if (type == JTokenType.Null)
        {
            return "string";
        }
        else // to string
        {
            return "string";
        }
    }

    public static bool CompareJTokenType (JToken jt1, JToken jt2) // if jt1 > jt2, return true
    {
        var t1 = jt1.Type;
        var t2 = jt2.Type;
        if (t2 == JTokenType.Integer)
        {
            if (t1 == JTokenType.Float)
                return true;
        }
        return false;
    }

    public static bool CompareStrType (string str_t1, string str_t2) // if str_t1 > str_t2, return true
    {
        str_t1 = str_t1.ToLower();
        str_t2 = str_t2.ToLower();

        if (str_t2 == "int" && (str_t1 == "float" || str_t1 == "double"))
            return true;

        return false;
    }
}
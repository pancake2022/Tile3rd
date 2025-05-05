using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Security.Cryptography;

namespace CSFramework
{
    public static class Utils
    {
        #region File/Path
        public static string NormalizePath (string path)
        {
            return path.Replace(@"\", "/");
        }
        
        public static List<FileInfo> GetFileList (string path, HashSet<string> except_extension_set = null, HashSet<string> except_subdir_set = null, HashSet<string> filter_extension_set = null, bool include_sub_dir = true)
        {
            var file_list = new List<FileInfo>();

            var dir_info = new DirectoryInfo(path);
            
            var file_arr = dir_info.GetFiles();
            foreach (var file in file_arr)
            {
                var except = false;
                if (except_extension_set != null)
                    except = except_extension_set.Contains(file.Extension);
                
                if (!except && filter_extension_set != null)
                    except = !filter_extension_set.Contains(file.Extension);

                if (!except)
                    file_list.Add(file);
            }

            if (include_sub_dir)
            {
                var sub_dir_list = dir_info.GetDirectories();
                foreach (var sub_dir in sub_dir_list)
                {
                    if (except_subdir_set == null || !except_subdir_set.Contains(sub_dir.Name))
                        file_list.AddRange(GetFileList(sub_dir.FullName, except_extension_set, null, filter_extension_set, include_sub_dir));
                }
            }

            return file_list;
        }

        public static void SaveFile (string full_file_path, string content, bool single_line = false)
        {
            var dir = Path.GetDirectoryName(full_file_path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var file_info = new FileInfo(full_file_path);
            using (var writer = file_info.CreateText())
            {
                if (single_line)
                    writer.WriteLine(content);
                else
                    writer.Write(content);
            }
        }

        public static void CopyFile (string path_src, string path_dst)
        {
            var dir_dst = Path.GetDirectoryName(path_dst);
            if (!Directory.Exists(dir_dst))
                Directory.CreateDirectory(dir_dst);
            File.Copy(path_src, path_dst, true);
        }

        public static void MakeSureDirectoryEmpty (string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }
        #endregion


        #region Template
        public static string FilterContent (string src_content, string start_tag, string end_tag, bool reverse = false)
        {
            var start_index = src_content.IndexOf(start_tag);
            var end_index = src_content.IndexOf(end_tag);
            if (start_index < 0)
            {
                Logger.Error(string.Format("FilterContent error: not found start_tag [{0}]", start_tag));
                return "";
            }
            else if (end_index < 0)
            {
                Logger.Error(string.Format("FilterContent error: not found end_tag [{0}]", end_tag));
                return "";
            }
            else
            {
                if (reverse)
                {
                    var end_end_index = end_index + end_tag.Length;
                    return string.Format("{0}{1}", src_content.Substring(0, start_index - 1), src_content.Substring(end_end_index, src_content.Length - end_end_index));
                }
                else
                {
                    var start_end_index = start_index + start_tag.Length;
                    return src_content.Substring(start_end_index, end_index - start_end_index - 1);
                }
            }
        }

        public static string ConvertType (string src_type, out bool is_object, string object_prefix = "", HashSet<string> prefix_class_set = null)
        {
            var type = src_type.ToLower();
            if (type.Length > 6 && type.Substring(0, 5) == "list<" && type[type.Length - 1] == '>')
            {
                is_object = true;
                var sub_type = src_type.Substring(5, type.Length - 6);
                sub_type = ConvertType(sub_type, out var io);
                return string.Format("{0}List<{1}>", object_prefix, sub_type);
            }
            else if (type.Length > 12 && type.Substring(0, 11) == "dictionary<" && type[type.Length - 1] == '>')
            {
                is_object = true;
                var sub_type_list = src_type.Substring(11, src_type.Length - 12).Split(',');
                if (sub_type_list.Length < 2)
                {
                    return string.Format("{0}Dictionary<{1}>", 
                                            object_prefix, 
                                            ConvertType(sub_type_list[0], out var is_obj, object_prefix));
                }
                else
                {
                    return string.Format("{0}Dictionary<{1}, {2}>", 
                                            object_prefix, 
                                            ConvertType(sub_type_list[0], out var is_obj_1, object_prefix), 
                                            ConvertType(sub_type_list[1], out var is_obj_2, object_prefix));
                }
            }
            else
            {
                is_object = false;
                if (type == "bool" || 
                    type == "int" || 
                    type == "uint" || 
                    type == "long" || 
                    type == "ulong" || 
                    type == "float" || 
                    type == "double" || 
                    type == "string")
                {
                    return type;
                }
                else
                {
                    is_object = true;
                    return string.Format("{0}{1}", src_type, prefix_class_set != null && prefix_class_set.Contains(src_type) ? object_prefix : "");
                }
            }
        }

        public static bool TryParseBaseType (string str_type, out Type type)
        {
            switch (str_type)
            {
                case "bool":
                {
                    type = typeof(bool);
                    return true;
                }
                case "int":
                {
                    type = typeof(int);
                    return true;
                }
                case "uint":
                {
                    type = typeof(uint);
                    return true;
                }
                case "long":
                {
                    type = typeof(long);
                    return true;
                }
                case "ulong":
                {
                    type = typeof(ulong);
                    return true;
                }
                case "float":
                {
                    type = typeof(float);
                    return true;
                }
                case "double":
                {
                    type = typeof(double);
                    return true;
                }
                case "string":
                {
                    type = typeof(string);
                    return true;
                }
            }
            type = default(Type);
            return false;
        }

        public static object ConvertValue (string type, object value)
        {
            if (type.Length > 6 && type.Substring(0, 5) == "list<" && type[type.Length - 1] == '>')
            {
                var sub_type = type.Substring(5, type.Length - 6);
                var value_list = value.ToString().Split(',');
                var list = new List<object>();
                for (var i = 0; i < value_list.Length; ++i)
                {
                    var sub_value = value_list[i];
                    if (!string.IsNullOrEmpty(sub_value))
                    {
                        var cv = ConvertValue(sub_type, sub_value);
                        if (cv != null)
                            list.Add(cv);
                    }
                }
                return list;
            }
            // else if (type.Length > 9 && type.Substring(0, 8) == "biglist<" && type[type.Length - 1] == '>')
            // {
            //     var sub_type = type.Substring(58, type.Length - 9);
            //     var value_list = value.ToString().Split('#');
            //     var list = new List<object>();
            //     for (var i = 0; i < value_list.Length; ++i)
            //     {
            //         var sub_value = value_list[i];
            //         if (!string.IsNullOrEmpty(sub_value))
            //             list.Add(ConvertValue(sub_type, sub_value));
            //     }
            //     return list;
            // }
            // else if (type.Substring(0, 11) == "dictionary<" && type[type.Length - 1] == '>')
            // {
            //     var sub_type = type.Substring(10, type.Length - 12);
            // }
            else if (TryParseBaseType(type, out var t))
            {
                if (t == typeof(string))
                {
                    return value.ToString();
                }
                else
                {
                    var func = t.GetMethod("TryParse", new Type[] { typeof(string), t.MakeByRefType() });
                    if (func != null)
                    {
                        var parem_list = new Object[] { value.ToString(), null };
                        if ((bool)func.Invoke(null, parem_list))
                            return parem_list[1];
                        else
                            return t.IsValueType ? Activator.CreateInstance(t) : null;
                    }
                    else
                    {
                        Logger.Error("ConvertValue Error, UnknowType: " + t.ToString());
                    }
                }
            }

            var possible_name = string.Format("CSFramework.{0}", type);
            var possible_type = Type.GetType(possible_name);
            if (possible_type != null)
            {
                var func = possible_type.GetMethod("TryParse", new Type[] { typeof(string), possible_type.MakeByRefType() });
                if (func != null)
                {
                    var parem_list = new Object[] { value.ToString(), null };
                    if ((bool)func.Invoke(null, parem_list))
                        return parem_list[1];
                }
                return null;
            }

            return "unknown";
        }
        #endregion

        public static string ToTableName (string name)
        {
            if (!string.IsNullOrEmpty(name))
                return name[0].ToString().ToUpper() + name.Substring(1);
            return "";
        }
    }
}
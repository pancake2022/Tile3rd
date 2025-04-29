using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using DG.Tweening;

namespace CSFramework
{
    public static class UtilsExtension
    {
        public static string ToDownloadPath (this string path)
        {
            return $"{Environment.PersistentDataDirectory}/{path}";
        }

        public static string ToPackagePath (this string path)
        {
            return $"{Environment.StreamingAssetsDirectory}/{path}";
        }
    }

    public static class Utils
    {
        public static readonly ulong DaySeconds = 86400; //  24 * 60 * 60
        public static readonly ulong DayMillisecond = 86400000; //  24 * 60 * 60 * 1000

        #region File/Path
        public static string NormalizePath (string path)
        {
            return path.Replace(@"\", "/");
        }
        
        public static string GetEditorExtraResourcesPath (string path)
        {
            return string.Format("Assets/{0}/{1}", Environment.ExtraResourcesPath, path);
        }

        public static string GetRawResourcesPath (string path)
        {
            return string.Format("Assets/{0}/{1}", Environment.RawResourcesPath, path);
        }

        public static string ConvertRawPathToExtraPath (string path)
        {
            return path.Replace(Environment.RawResourcesPath, Environment.ExtraResourcesPath);
        }

        public static List<FileInfo> GetFileList (string path, HashSet<string> except_extension_set = null, bool include_sub_dir = true)
        {
            var file_list = new List<FileInfo>();

            var dir_info = new DirectoryInfo(path);
            
            var file_arr = dir_info.GetFiles();
            foreach (var file in file_arr)
            {
                var except = false;
                if (except_extension_set != null)
                    except = except_extension_set.Contains(file.Extension);

                if (!except)
                    file_list.Add(file);
            }

            if (include_sub_dir)
            {
                var sub_dir_list = dir_info.GetDirectories();
                foreach (var sub_dir in sub_dir_list)
                    file_list.AddRange(GetFileList(sub_dir.FullName, except_extension_set, include_sub_dir));
            }

            return file_list;
        }

        public static List<DirectoryInfo> GetDirectoryList (string path, bool include_sub_dir = true)
        {
            var dir_list = new List<DirectoryInfo>();

            var dir_info = new DirectoryInfo(path);
            var sub_dir_list = dir_info.GetDirectories();
            foreach (var sub_dir in sub_dir_list)
            {
                dir_list.Add(sub_dir);
                if (include_sub_dir)
                    dir_list.AddRange(GetDirectoryList(sub_dir.FullName, include_sub_dir));
            }

            return dir_list;
        }

        public static List<FileInfo> GetExtraResourcesList (string relative_path, bool include_sub_dir = true)
        {
            return GetFileList(GetEditorExtraResourcesPath(relative_path), new HashSet<string>
            {
                ".meta",
                ".DS_Store",
                ".gitkeep",
            }, include_sub_dir);
        }

        public static string GetRelativePath (string full_path)
        {
            full_path = NormalizePath(full_path);
            return full_path.Replace(Application.dataPath, "Assets");
        }

        public static string GetRelativeExtraResourcesPath (string full_path)
        {
            full_path = NormalizePath(full_path);
            var prefix = string.Format("{0}/{1}/", Application.dataPath, Environment.ExtraResourcesPath);
            return full_path.Replace(prefix, "");
        }

        public static string GetFileMD5 (string file_path)
        {
            var str_md5 = "";
            if (File.Exists(file_path))
            {
                try
                {
                    using (var file_stream = File.OpenRead(file_path))
                    {
                        var md5 = MD5.Create();
                        var md5_bytes = md5.ComputeHash(file_stream);
                        str_md5 = FormatMD5(md5_bytes);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            return str_md5;
        }

        public static bool IsDiskFull(Exception e)
        {
            if(e == null)
                return false;

            const int HR_ERROR_HANDLE_DISK_FULL = unchecked((int)0x80070027);
            const int HR_ERROR_DISK_FULL = unchecked((int)0x80070070);

            return e.HResult == HR_ERROR_HANDLE_DISK_FULL || e.HResult == HR_ERROR_DISK_FULL;
        }

        public static string FormatMD5(Byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "").ToLower();
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

        public static void MakeSureDirectoryExist (string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public static string SafeGetString (string key, string default_value = null)
        {
            if (PlayerPrefs.HasKey(key))
            {
                try
                {
                    var bytes = System.Convert.FromBase64String(PlayerPrefs.GetString(key));
                    return RijndaelUtils.DecryptStringFromBytes(bytes);
                }
                catch (Exception e)
                {
                    Logger.Error($"GetString error: {e}");
                    return default_value;
                }
            }
            else
            {
                return default_value;
            }
        }

        public static void SafeSetString (string key, string value)
        {
            var bytes = RijndaelUtils.EncryptStringToBytes(value);
            PlayerPrefs.SetString(key, System.Convert.ToBase64String(bytes));
        }
        #endregion

        public static void ExecuteShell (string shell_path)
        {
            Logger.Log($"ExecuteShell: {shell_path}");
            var filename = Directory.GetCurrentDirectory()+ "/" + shell_path;
            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                CreateNoWindow = true,
                FileName = filename,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            var proc = new System.Diagnostics.Process()
            {
                EnableRaisingEvents = true,
                StartInfo = startInfo,
            };
            proc.Start();
            proc.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Logger.Log(e.Data);
            };
            proc.ErrorDataReceived += (s, e) => 
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Logger.Error(e.Data);
            };
            proc.EnableRaisingEvents = true;
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            proc.WaitForExit();
            if (proc.ExitCode != 0)
                throw new Exception($"{shell_path} Execute Failed");
        }

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

        public static string ConvertType (string src_type, out bool is_object, string object_prefix = "")
        {
            var type = src_type.ToLower();
            if (type.Length > 6 && type.Substring(0, 5) == "list<" && type[type.Length - 1] == '>')
            {
                is_object = true;
                var sub_type = src_type.Substring(5, type.Length - 6);
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
                    return string.Format("{0}{1}", src_type, object_prefix);
                }
            }
        }

        public static object ConvertValue (string type, object value)
        {
            if (type.Length > 6 && type.Substring(0, 5) == "list<" && type[type.Length - 1] == '>')
            {
                var sub_type = type.Substring(5, type.Length - 6);
                var value_list = value.ToString().Split('#');
                var list = new List<object>();
                for (var i = 0; i < value_list.Length; ++i)
                {
                    var sub_value = value_list[i];
                    if (!string.IsNullOrEmpty(sub_value))
                        list.Add(ConvertValue(sub_type, sub_value));
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
            else
            {
                switch (type)
                {
                    case "bool":
                    {
                        return bool.TryParse(value.ToString(), out var v) ? v : false;
                    }
                    case "int":
                    {
                        return int.TryParse(value.ToString(), out var v) ? v : 0;
                    }
                    case "uint":
                    {
                        return uint.TryParse(value.ToString(), out var v) ? v : 0;
                    }
                    case "long":
                    {
                        return long.TryParse(value.ToString(), out var v) ? v : 0;
                    }
                    case "ulong":
                    {
                        return ulong.TryParse(value.ToString(), out var v) ? v : 0;
                    }
                    case "float":
                    {
                        return float.TryParse(value.ToString(), out var v) ? v : 0;
                    }
                    case "double":
                    {
                        return double.TryParse(value.ToString(), out var v) ? v : 0;
                    }
                    case "string":
                    {
                        return value.ToString();
                    }
                }
            }
            
            return "unknown";
        }
        #endregion

        #region Component
        public static Component GetOrCreateComponent (Type type, Component t)
        {
            return GetOrCreateComponent(type, t.gameObject);
        }

        public static Component GetOrCreateComponent (Type type, GameObject obj)
        {
            var component = obj.GetComponent(type);
            if (!component)
                component = obj.AddComponent(type);
            return component;
        }

        public static T GetOrCreateComponent<T> (Component t) where T : Component
        {
            return GetOrCreateComponent<T>(t.gameObject);
        }

        public static T GetOrCreateComponent<T> (GameObject obj) where T : Component
        {
            var component = obj.GetComponent<T>();
            if (!component)
                component = obj.AddComponent<T>();
            return component;
        }

        public static void SetPosition (GameObject obj, Vector2 position)
        {
            SetPosition(obj.transform, position);
        }

        public static void SetPosition (Transform transform, Vector2 position)
        {
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }

        #endregion

        #region Utils

        public static Tween DelayedCall (float delay_time, TweenCallback callback, bool ignore_time_scale = true)
        {
            return DOVirtual.DelayedCall(delay_time, () => 
            {
                try
                {
                    callback?.Invoke();
                }
                catch (Exception e)
                {
                    Logger.Error(string.Format("DelayedCall exception: {0}", e));
                }
            }, ignore_time_scale);
        }

        public static string RemoveClone (string name)
        {
            return name.Replace("(Clone)", "");
        }

        public static Vector3 ConvertPosition (Vector3 src_position, Camera src_camera, Camera target_camera)
        {
            var target_position = src_camera.WorldToScreenPoint(src_position);
            return target_camera.ScreenToWorldPoint(target_position);
        }

        public static ulong CurrentTimestamp()
        {
            var ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return Convert.ToUInt64(ts.TotalMilliseconds);
        }

        public static int CurrentSecondTimestamp()
        {
            var ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return Convert.ToInt32(ts.TotalSeconds);
        }

        public static String TimestampToTimeString (long timestamp, string format = "yyyy-MM-dd HH:mm:ss")
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp);
            return dt.ToString(format);
        }

        public static String FormatMillisecondToTimeString (long millisecond, bool full_format = false)
        {
            millisecond = millisecond < 0 ? 0 : millisecond;
            var second = (int)(millisecond / 1000);
            return FormatSecondToTimeString(second, full_format);
        }

        public static String FormatSecondToTimeString (int second, bool full_format = false)
        {
            int hour = 0;
            int minute = 0;
            int day = 0;

            if (second >= 60)
            {
                minute = second / 60;
                second = second % 60;
            }
            if (minute >= 60)
            {
                hour = minute / 60;
                minute = minute % 60;
            }

            if (hour >= 24)
            {
                day = hour / 24;
                hour = hour % 24;
            }

            var d = "d";
            if (full_format)
            {
                return day > 0 ? $"{day}{d} {hour:D2}:{minute:D2}" : $"{hour:D2}:{minute:D2}:{second:D2}";
            }
            else if (day > 0)
            {
                var h = "h";
                if (hour > 0)
                {
                    return $"{day}{d} {hour}{h}";
                }
                else if (minute >= 30)
                {
                    return $"{day}{d} 1{h}";
                }
                else
                {
                    return $"{day}{d}";
                }
            }
            else
            {
                return $"{hour:D2}:{minute:D2}:{second:D2}";
            }
        }
        #endregion

        public static bool AnimationExist (Animator animator, string animation_name)
        {
            if (animator == null)
                return false;
                
            if (animator.runtimeAnimatorController == null)
                return false;

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name.Equals(animation_name))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool PlayAnimation (Animator animator, string animation_name)
        {
            if (AnimationExist(animator, animation_name))
            {
                animator.Play(animation_name);
                return true;
            }
            return false;
        }

        public static void SafeCall (Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static Vector3 LerpByPrePercent (Vector3 current, Vector3 target, float pre_percent, float percent)
        {
            var pre_p = pre_percent / (1 - pre_percent);
            var start = current + (current - target) * pre_p;
            return Vector3.Lerp(start, target, percent);
        }

        public static Vector2 LerpByPrePercent (Vector2 current, Vector2 target, float pre_percent, float percent)
        {
            var pre_p = pre_percent / (1 - pre_percent);
            var start = current + (current - target) * pre_p;
            return Vector2.Lerp(start, target, percent);
        }
    }
}

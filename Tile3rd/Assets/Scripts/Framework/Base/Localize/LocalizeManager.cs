using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;

namespace CSFramework
{
    public delegate Dictionary<string, string> RequireLocalizeDataFunc (string local_name);
    public delegate bool RequireLocalizeFontFunc (TMP_FontAsset old_font, string local_name, out TMP_FontAsset font_asset);
    public delegate bool RequireLocalizeMaterialFunc (TMP_FontAsset old_font, string local_name, string material_name, out Material material);
    public delegate string ParseFontNameFunc (TMP_FontAsset old_font);

    public class LocalizeContext
    {
        public RequireLocalizeDataFunc RequireLocalizeDataFunc = null;
        public RequireLocalizeFontFunc RequireLocalizeFontFunc = null;
        public RequireLocalizeMaterialFunc RequireLocalizeMaterialFunc = null;
        public ParseFontNameFunc ParseFontNameFunc = null;
        public List<string> SupportLocalNameList = new List<string>();
        public string LocalizeKeyPrefix = string.Empty;
    }

    public class LocalizeManager : Module<Framework>
    {
        public LocalizeContext Context { get { return _context; } }
        public string LocalName { get { return _current_local_name; } }

        private LocalizeContext _context;
        private Dictionary<string, Dictionary<string, string>> _localize_data_dict;
        private Dictionary<string, Dictionary<string, TMP_FontAsset>> _font_cache;
        private Dictionary<string, Dictionary<string, Material>> _material_cache;
        private string _current_local_name;
        private Dictionary<string, string> _current_localize_data;

        protected override IEnumerator on_init (params object[] param_list)
        {
            _context = _main_module.Context.LocalizeContext;
            _localize_data_dict = new Dictionary<string, Dictionary<string, string>>();
            _font_cache = new Dictionary<string, Dictionary<string, TMP_FontAsset>>();
            _material_cache = new Dictionary<string, Dictionary<string, Material>>();

            var ui_manager = _main_module.UIManager;
            ui_manager.OnCreateUICallback += on_create_ui;

            return null;
        }

        private void on_create_ui (BaseUI ui, bool is_new_gameobject)
        {
            if (is_new_gameobject)
                localize_ui(ui);
        }

        private void localize_ui (BaseUI ui)
        {
            var component_list = ui.GetComponentsInChildren<LocalizeTextMeshProUGUI>(true);
            foreach (var component in component_list)
            {
                if (component.UseFormat)
                    continue;

                var key = component.GetKey();
                if (!string.IsNullOrEmpty(key))
                {
                    component.SetText(GetLocalString(key));
                    component.RefreshFont(this);
                }
            }
        }

        private Dictionary<string, string> require_localize_data (string local_name)
        {
            if (!_localize_data_dict.TryGetValue(local_name, out var localize_data))
            {
                localize_data = _context.RequireLocalizeDataFunc?.Invoke(local_name);

                if (localize_data == null)
                    CSFramework.Logger.Error($"LocalizeManager.localize_data: not found localize_data [{local_name}]");
                else
                    _localize_data_dict[local_name] = localize_data;
            }

            return localize_data;
        }

        public void SetLocalName (string local_name)
        {
            if (_current_local_name != local_name)
            {
                _current_local_name = local_name;
                _current_localize_data = require_localize_data(local_name);

                // refresh
                var all_window = _main_module.UIManager.AllWindow;
                foreach (var window in all_window)
                    localize_ui(window);

                var all_root_ui = _main_module.UIManager.AllRootUI;
                foreach (var ui in all_root_ui)
                    localize_ui(ui);
            }
        }

        public string GetLocalString (string key, string local_name = "")
        {
            if (string.IsNullOrEmpty(key))
                return key;

            if (string.IsNullOrEmpty(local_name))
                local_name = _current_local_name;

            var localize_data = local_name == _current_local_name? _current_localize_data : require_localize_data(local_name);
            if (localize_data == null)
            {
                return key;
            }
            else
            {
                return get_local_string(localize_data, key);
            }
        }

        public string GetLocalStringWithFormats (string key, string local_name, params object[] values)
        {
            var str = GetLocalString(key, local_name);
            str = string_format_transformer(str);
            str = String.Format(str, values);
            return str;
        }

        private string string_format_transformer (string str_origin)
        {
            if (String.IsNullOrEmpty(str_origin))
            {
                return String.Empty;
            }
            // 增加替换全角％的功能
            var str_result = str_origin.Replace("%S", "%s").Replace("％S", "%s").Replace("％s", "%s");

            var index = 0;
            while (true)
            {
                var s = replace_first(str_result, "%s", "{" + index + "}");
                if (str_result.Equals(s))
                    break;
                str_result = s;
                index++;
            }
            return str_result;
        }

        private string replace_first(string text, string search, string replace)
        {
            var pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
                return text;
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public bool TryGetLocalizeResource (TMP_FontAsset old_font, string local_name, string material_name, out TMP_FontAsset font_asset, out Material material)
        {
            if (string.IsNullOrEmpty(local_name))
                local_name = _current_local_name;

            var font_name = _context.ParseFontNameFunc(old_font);
            if (string.IsNullOrEmpty(font_name))
            {
                font_asset = null;
                material = null;
                return false;
            }

            var result = false;
            font_asset = null;
            if (_font_cache.TryGetValue(local_name, out var font_asset_dict) && font_asset_dict.TryGetValue(font_name, out font_asset))
            {
                result = true;
            }
            else if (_context.RequireLocalizeFontFunc != null && _context.RequireLocalizeFontFunc(old_font, local_name, out font_asset))
            {
                result = true;
                if (font_asset_dict == null)
                {
                    font_asset_dict = new Dictionary<string, TMP_FontAsset>();
                    _font_cache[local_name] = font_asset_dict;
                }
                font_asset_dict[font_name] = font_asset;
            }
            else
            {
                CSFramework.Logger.Warning($"LocalizeManager.TryGetLocalizeResource Error {local_name}");
            }

            material = null;
            if (result && !string.IsNullOrEmpty(material_name) && !material_name.Equals("Material")/*TODO*/)
            {
                if (!_material_cache.TryGetValue(local_name, out var material_dict))
                {
                    material_dict = new Dictionary<string, Material>();
                    _material_cache[local_name] = material_dict;
                }

                if (material_dict.TryGetValue(material_name, out material))
                {
                    result |= true;
                }
                else if (_context.RequireLocalizeMaterialFunc != null && _context.RequireLocalizeMaterialFunc(old_font, local_name, material_name, out material))
                {
                    result |= true;
                    material_dict[material_name] = material;
                }
                else
                {
                    CSFramework.Logger.Warning($"LocalizeManager.TryGetLocalizeResource Error {local_name}, {material_name}");
                    material = null;
                    result = false;
                }
            }
            else
            {
                material = null;
            }

            return result;
        }

        private string get_local_string (Dictionary<string, string> localize_data, string key)
        {
            if (!string.IsNullOrEmpty(Context.LocalizeKeyPrefix) && key.StartsWith(Context.LocalizeKeyPrefix, StringComparison.InvariantCulture))
                key = key.Substring(Context.LocalizeKeyPrefix.Length);

            if (localize_data.TryGetValue(key, out var value))
            {
                var match_result_collection = Regex.Matches(value, "%{(.+?)}");
                foreach (Match match_result in match_result_collection)
                {
                    var match_key = match_result.Groups[1].Value;
                    value = value.Replace("%{" + match_key + "}", get_local_string(localize_data, match_key));
                }
                return value;
            }
            else
            {
                return key;
            }
        }

        public void AutoSelectLocalName ()
        {
            // todo read storage
            SetLocalName(get_local_name(get_system_language()));
        }

        private string get_system_language ()
        {
            return SystemLanguageToLocalizeName(Application.systemLanguage);
        }

        public static string SystemLanguageToLocalizeName (SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.Afrikaans: return "af";
                case SystemLanguage.Arabic: return "ar";
                case SystemLanguage.Basque: return "eu";
                case SystemLanguage.Belarusian: return "be";
                case SystemLanguage.Bulgarian: return "bg";
                case SystemLanguage.Catalan: return "ca";
                case SystemLanguage.Chinese: return "zh";
                case SystemLanguage.ChineseSimplified: return "zh";
                case SystemLanguage.ChineseTraditional: return "zht";
                case SystemLanguage.Czech: return "cs";
                case SystemLanguage.Danish: return "da";
                case SystemLanguage.Dutch: return "nl";
                case SystemLanguage.English: return "en";
                case SystemLanguage.Estonian: return "et";
                case SystemLanguage.Faroese: return "fo";
                case SystemLanguage.Finnish: return "fi";
                case SystemLanguage.French: return "fr";
                case SystemLanguage.German: return "de";
                case SystemLanguage.Greek: return "el";
                case SystemLanguage.Hebrew: return "he";
                case SystemLanguage.Icelandic: return "is";
                case SystemLanguage.Indonesian: return "id";
                case SystemLanguage.Japanese: return "jp";
                case SystemLanguage.Korean: return "kr";
                case SystemLanguage.Latvian: return "lv";
                case SystemLanguage.Lithuanian: return "lt";
                case SystemLanguage.Norwegian: return "no";
                case SystemLanguage.Polish: return "pl";
                case SystemLanguage.Portuguese: return "pt";
                case SystemLanguage.Romanian: return "ro";
                case SystemLanguage.Russian: return "ru";
                case SystemLanguage.SerboCroatian: return "hr";
                case SystemLanguage.Slovak: return "sk";
                case SystemLanguage.Slovenian: return "sl";
                case SystemLanguage.Spanish: return "es";
                case SystemLanguage.Swedish: return "sv";
                case SystemLanguage.Thai: return "th";
                case SystemLanguage.Turkish: return "tr";
                case SystemLanguage.Ukrainian: return "uk";
                case SystemLanguage.Vietnamese: return "vi";
                case SystemLanguage.Hungarian: return "hu";
                case SystemLanguage.Italian: return "it";
                case SystemLanguage.Unknown: return "en";
            }

            return "en";
        }

        private string get_local_name (string str_language)
        {
            if (_context.SupportLocalNameList.Contains(str_language))
                return str_language;

            return "en";
        }
    }
}
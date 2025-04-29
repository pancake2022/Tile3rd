using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework.Editor
{
    public class SpriteAtlasConfigEditor
    {
        private static double _last_check_time;
        private static bool _dirty;

        [InitializeOnLoadMethod]
        private static void start_watch ()
        {
            EditorApplication.projectChanged += on_project_changed;
            EditorApplication.update += update;
        }

        private static void on_project_changed ()
        {
            _dirty = true;
        }

        private static void update ()
        {
            var dt = EditorApplication.timeSinceStartup - _last_check_time;
            if (dt >= 1.0f)
            {
                if (_dirty)
                {
                    var asset_version_file = Resources.Load<AssetVersionFile>(Environment.AssetVersionFilePath);
                    if (!asset_version_file)
                    {
                        EditorUtils.CreateAsset<AssetVersionFile>(Environment.AssetVersionFilePath);
                        asset_version_file = Resources.Load<AssetVersionFile>(Environment.AssetVersionFilePath);
                        EditorGUIUtility.PingObject(asset_version_file);
                    }
                    var sprite_atlas_config_path = Utils.GetEditorExtraResourcesPath(asset_version_file.SpriteAtlasConfigPath) + ".asset";
                    var sprite_atlas_config = AssetDatabase.LoadAssetAtPath<SpriteAtlasConfig>(sprite_atlas_config_path);
                    if (sprite_atlas_config)
                    {
                        if (sprite_atlas_config.CheckUpdateFromFile())
                        {
                            EditorUtility.SetDirty(sprite_atlas_config);
                            AssetDatabase.SaveAssets();
                        }
                    }
                    _dirty = false;
                }
                _last_check_time = EditorApplication.timeSinceStartup;
            }
        }
    }
}
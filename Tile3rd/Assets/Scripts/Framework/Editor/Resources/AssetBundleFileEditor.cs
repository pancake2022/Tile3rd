using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSFramework.Editor
{
    public class AssetBundleFileEditor : EditorWindow
    {
        [MenuItem("CSFramework/AssetBundle/Open Asset Bundle File Editor")]
        public static void OpenAssetBundleFileEditor ()
        {
            var window = EditorWindow.GetWindow<AssetBundleFileEditor>();
            window.Show();
            window.position = new Rect(500, 200, 1000, 500);
            window.titleContent = new GUIContent("Asset Bundle File Editor");
        }

        [MenuItem("CSFramework/AssetBundle/CreateSpriteAtlasConfig")]
        public static void CreateSpriteAtlasConfig ()
        {
            var asset_version_file = Resources.Load<AssetVersionFile>(Environment.AssetVersionFilePath);
            if (asset_version_file)
            {
                var sprite_atlas_config_full_path = Utils.GetEditorExtraResourcesPath(asset_version_file.SpriteAtlasConfigPath);
                EditorUtils.CreateAssetWithFullPath<SpriteAtlasConfig>(sprite_atlas_config_full_path);
                var sprite_atlas_config = AssetDatabase.LoadAssetAtPath<SpriteAtlasConfig>(sprite_atlas_config_full_path);
                EditorGUIUtility.PingObject(sprite_atlas_config);
            }
            else
            {
                CSFramework.Logger.Error($"Not found AssetVersionFile");
            }
        }
        private string _asset_root_path;
        private AssetVersionFileTemporary _temp_file = null;
        private Vector2 _directory_tree_scroll_position = Vector2.zero;

        public void Start ()
        {
            _asset_root_path = Environment.ExtraResourcesPath;
        }

        public void OnDestroy() 
        {
            _temp_file = null;
        }

        private void load_or_create_version_file ()
        {
            var asset_version_file = Resources.Load<AssetVersionFile>(Environment.AssetVersionFilePath);
            if (!asset_version_file)
            {
                EditorUtils.CreateAsset<AssetVersionFile>(Environment.AssetVersionFilePath);
                asset_version_file = Resources.Load<AssetVersionFile>(Environment.AssetVersionFilePath);
                EditorGUIUtility.PingObject(asset_version_file);
            }
            _temp_file = new AssetVersionFileTemporary(asset_version_file);
        }

        public void OnGUI() 
        {
            draw_path("Asset Root Folder", ref _asset_root_path);

            draw_directory_tree();

            if (GUILayout.Button("Load Or Create Asset Version File"))
            {
                load_or_create_version_file();
            }

            if (GUILayout.Button("Apply To Asset Version File"))
            {
                if (_temp_file == null)
                {
                    EditorUtility.DisplayDialog("Error", "Must Load Or Create Asset Version File First", "ok");
                }
                else
                {
                    _temp_file.OverwriteFile();
                    EditorUtility.SetDirty(_temp_file.File);
                    AssetDatabase.SaveAssets();
                    EditorUtility.DisplayDialog("Info", "Apply Success", "ok");
                }
            }
        }

        private void draw_directory_tree ()
        {
            using (var v = new EditorGUILayout.VerticalScope())
            {
                using (var s = new EditorGUILayout.ScrollViewScope(_directory_tree_scroll_position))
                {
                    _directory_tree_scroll_position = s.scrollPosition;
                    if (_temp_file != null)
                        draw_item(_temp_file.Root, 0);
                }
            }
        }

        private void draw_item (AssetVersionFileTemporary.ItemTemporary item, int offset)
        {
            GUILayout.BeginHorizontal();
            var str_offset = item.Item.IsPackAssetBundle ? "+" : "-";
            GUILayout.Label(str_offset, GUILayout.ExpandWidth(false));
            GUILayout.Space(offset * 16);
            var display_name = string.IsNullOrEmpty(item.Item.Path) ? string.Format("Root: {0}", Environment.ExtraResourcesPath) : item.Item.Path;
            GUILayout.Label(display_name, GUILayout.ExpandWidth(false));
            GUILayout.Space(16);
            item.Item.IsPackAssetBundle = GUILayout.Toggle(item.Item.IsPackAssetBundle, "Pack", GUILayout.ExpandWidth(false));
            if (item.Item.IsPackAssetBundle)
            {
                item.Item.InPackage = GUILayout.Toggle(item.Item.InPackage, "初始包", GUILayout.ExpandWidth(false));
            }
            else if (item.ChildDict.Count > 0)
            {
                var old_value = item.Item.IsPackAllChild;
                item.Item.IsPackAllChild = GUILayout.Toggle(item.Item.IsPackAllChild, "PackAllChild", GUILayout.ExpandWidth(false));
                if (!old_value && item.Item.IsPackAllChild)
                {
                    foreach (var child in item.ChildDict.Values)
                        child.Item.IsPackAssetBundle = true;
                }
            }
            item.Item.IsUploadRawResource = GUILayout.Toggle(item.Item.IsUploadRawResource, "上传原始资源", GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            if (!item.Item.IsPackAssetBundle)
            {
                if (item.ChildDict.Count > 0)
                    GUILayout.Space(2);
                foreach (var child in item.ChildDict.Values)
                    draw_item(child, offset + 1);
                if (item.ChildDict.Count > 0)
                    GUILayout.Space(2);
            }
        }

        private void draw_path (string path_name, ref string path)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(path_name + ": ");
            GUILayout.Label(path);
            if (GUILayout.Button("Select"))
            {
                var root = Application.dataPath + "/";
                var full_path = root + path;
                var result = EditorUtility.OpenFolderPanel("Choose " + path_name, full_path, "");
                if (!string.IsNullOrEmpty(result))
                {
                    path = result.Replace(root, "");
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
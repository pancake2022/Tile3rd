using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using CSFramework;
using Newtonsoft.Json;

[Serializable]
public class T4Cell
{
    [SerializeField]
    public List<int> Pos;
    [SerializeField]
    public int CustomType;
}

[Serializable]
public class T4Layer
{
    [SerializeField]
    public List<int> p;
    [SerializeField]
    public List<List<int>> t;
    [SerializeField]
    public List<T4Cell> TileList;
}

[Serializable]
public class T4Panel
{
    [SerializeField]
    public List<int> p;
    [SerializeField]
    public List<T4Layer> l;
}

public static class M3EditorUtils
{
    [MenuItem("M3/配置/配置转换")]
    public static void ConvertJson ()
    {
        var target_root = Utils.GetEditorExtraResourcesPath("Config/M3/Panel/");
        var t4json_file_list = Utils.GetFileList(Utils.GetEditorExtraResourcesPath("TempResources/t4json"));
        t4json_file_list.QuickEach((i, file) => 
        {
            if (file.Extension.ToLower() == ".json")
            {
                var pure_name = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                Utils.SafeCall(() => 
                {
                    T4Panel t4panel = null;
                    using (var sr = file.OpenText())
                    {
                        var str = sr.ReadToEnd();
                        t4panel = JsonConvert.DeserializeObject<T4Panel>(str);
                    }

                    var m3panel = new M3Panel();
                    if (int.TryParse(pure_name, out var id))
                        m3panel.ID = id;
                    m3panel.Desc = pure_name;
                    t4panel.l.QuickEach((index, l) =>
                    {
                        var layer = new M3Layer();
                        layer.Index = index;

                        if (l.TileList != null)
                        {
                            l.TileList.QuickEach((tile_index, tile) => 
                            {
                                var cell = new M3Cell();
                                cell.Type = Mathf.Max(0, tile.CustomType);
                                cell.IsBF = cell.Type > M3Const.CellTypeRandom;
                                cell.X = tile.Pos[0] / 50 + M3Const.LayerSize / 2;
                                cell.Y = tile.Pos[1] / 50 + M3Const.LayerSize / 2 + 1;
                                layer.CellList.Add(cell);
                            });
                        }
                        
                        if (l.t != null)
                        {
                            l.t.QuickEach((tile_index, tile) => 
                            {
                                var cell = new M3Cell();
                                cell.X = tile[0] / 50 + M3Const.LayerSize / 2;
                                cell.Y = tile[1] / 50 + M3Const.LayerSize / 2 + 1;
                                layer.CellList.Add(cell);
                            });
                        }
                        m3panel.LayerList.Add(layer);
                    });
                    var file_path = Utils.GetEditorExtraResourcesPath($"{M3Const.M3PanelConfigPath}/{file.Name}");
                    Utils.SaveFile(file_path, JsonUtility.ToJson(m3panel));
                });
            }
        });
        AssetDatabase.Refresh();
    }

    [MenuItem("M3/配置/应用LevelCSV配置")]
    public static void ApplyLevelCSV ()
    {
        var csv = AssetDatabase.LoadAssetAtPath<TextAsset>(Utils.GetEditorExtraResourcesPath("TempResources/t4level.csv"));
        if (csv)
        {
            var item_list = csv.text.Split('\n');
            for (var i = 0; i < item_list.Length; ++i)
            {
                var item = item_list[i];
                if (!string.IsNullOrEmpty(item))
                {
                    var data_list = item.Split(',');
                    if (data_list.Length >= 3 && 
                        int.TryParse(data_list[0], out var level_id) && 
                        int.TryParse(data_list[1], out var layout_id) && 
                        int.TryParse(data_list[2], out var difficulty_level))
                    {
                        var panel_json_path = Utils.GetEditorExtraResourcesPath($"Config/M3/Panel/{layout_id}.json");
                        var panel_json = AssetDatabase.LoadAssetAtPath<TextAsset>(panel_json_path);
                        if (panel_json)
                        {
                            var panel = JsonUtility.FromJson<M3Panel>(panel_json.text);
                            panel.ReferenceLevelID = level_id;
                            panel.DifficultyLevel = difficulty_level;
                            panel.RandomAllCellType();
                            Utils.SaveFile(panel_json_path, JsonUtility.ToJson(panel));
                        }
                        else
                        {
                            CSFramework.Logger.Warning($"not found json file: {panel_json_path}");
                        }
                    }
                }
            }
        }
        else
        {
            CSFramework.Logger.Warning("not found csv file");
        }
        AssetDatabase.Refresh();
    }
}
using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class M3Editor : WindowUI
{
    public static new string DefaultPrefabPath = "M3Editor/M3Editor";

    private InputField _save_as_path;

    private Text _message_text;
    private Text _error_text;

    public PanelUI PanelUI;
    public LayerUI LayerUI;
    public CellEditUI CellEditUI;
    public CellEditControlUI CellEditControlUI;

    public List<M3Panel> AllPanelList;

    protected override void on_create()
    {
        Property.UseCommonAnimation = false;
        Property.PlayOpenCloseSound = false;

        register_button("TopMenu/NewPanelButton", on_new_panel_clicked);
        register_button("TopMenu/SavePanelButton", on_save_panel_clicked);
        register_button("TopMenu/DeletePanelButton", on_delete_panel_clicked);
        register_button("TopMenu/SaveAsGroup/SaveAsButton", on_save_as_clicked);
        _save_as_path = find_component<InputField>("TopMenu/SaveAsGroup/SaveAsPath");
        _message_text = find_component<Text>("TopMenu/MessageGroup/MessageText");
        _error_text = find_component<Text>("TopMenu/MessageGroup/ErrorText");
        register_button("TopMenu/StartGameButton", on_start_game_clicked);

        PanelUI = create_ui<PanelUI>("PanelUI").Init(this);
        LayerUI = create_ui<LayerUI>("LayerUI").Init(this);
        CellEditUI = create_ui<CellEditUI>("CellEditUI").Init(this);
        CellEditControlUI = create_ui<CellEditControlUI>("CellEditControlUI").Init(this);
    }

    protected override void on_open()
    {
        AllPanelList = new List<M3Panel>();
        var file_list = Utils.GetExtraResourcesList(M3Const.M3PanelConfigPath, include_sub_dir: false);
        foreach (var file in file_list)
        {
            if (file.Extension.ToLower() == ".json")
            {
                M3Panel m3_panel = null;
                
                try
                {
                    using (var sr = file.OpenText())
                    {
                        var str = sr.ReadToEnd();
                        m3_panel = JsonUtility.FromJson<M3Panel>(str);
                    }
                }
                catch (Exception e)
                {
                    CSFramework.Logger.Error(e);
                }

                if (m3_panel != null)
                    AllPanelList.Add(m3_panel);
            }
        }
        AllPanelList.Sort((a, b) => a.ID.CompareTo(b.ID));

        PanelUI.InitPanelList();
        Debug.Log("加载布局");
    }

    private void on_new_panel_clicked ()
    {
        var max_panel_id = AllPanelList.Count > 0 ? AllPanelList[AllPanelList.Count - 1].ID : 0;
        var new_panel = new M3Panel
        {
            ID = max_panel_id + 1,
        };
        AllPanelList.Add(new_panel);
        PanelUI.SelectPanel(new_panel);
    }

    private void on_save_panel_clicked ()
    {
        var panel = PanelUI.CurrentPanel;
        if (panel == null)
        {
            ShowError($"当前布局为空");
        }
        else if (!CellEditUI.IsBrushStatusPass)
        {
            ShowError($"保存失败! 请检查布局【{panel.DispalyName}】的牌面数量");
        }
        else
        {
            if (panel.PanelType == M3PanelType.RandomSeed)
                panel.RefreshPanelData(); // force refresh panel data

            if (CellEditUI.InEditing)
                CellEditControlUI.OnEditModeChanged(false);

            var file_path = Utils.GetEditorExtraResourcesPath($"{M3Const.M3PanelConfigPath}/{panel.ID}.json");
            Utils.SaveFile(file_path, JsonUtility.ToJson(panel), true);
            ShowMessage($"布局【{panel.DispalyName}】保存成功");
            panel.IsDirty.Value = false;
        }
    }

    private void on_delete_panel_clicked ()
    {
        var panel = PanelUI.CurrentPanel;
        if (panel == null)
        {
            ShowError($"当前布局为空");
        }
        else
        {
            if (AllPanelList.Remove(panel))
            {
                var file_path = Utils.GetEditorExtraResourcesPath($"{M3Const.M3PanelConfigPath}/{panel.ID}.json");
                System.IO.File.Delete(file_path);
                ShowMessage($"布局【{panel.DispalyName}】删除成功");
                PanelUI.InitPanelList();
            }
            else
            {
                ShowMessage($"布局【{panel.DispalyName}】删除失败，未在当前列表");
            }
        }
    }

    private void on_save_as_clicked ()
    {

    }

    private void on_start_game_clicked ()
    {
        var panel = PanelUI.CurrentPanel;
        if (panel == null)
        {
            ShowError($"当前布局为空");
        }
        else
        {
            _ui_manager.OpenWindow<M3EditorGameWindow>().Init(panel);
        }
    }

    public void ShowMessage (string message)
    {
        _message_text.gameObject.SetActive(true);
        _error_text.gameObject.SetActive(false);
        _message_text.text = message;
    }

    public void ShowError (string message)
    {
        _message_text.gameObject.SetActive(false);
        _error_text.gameObject.SetActive(true);
        _error_text.text = message;
    }
}
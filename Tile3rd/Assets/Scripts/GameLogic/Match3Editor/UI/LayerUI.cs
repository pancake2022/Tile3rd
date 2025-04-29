using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class LayerUI : BaseUI
{
    public class LayerButton : BaseUI
    {
        public M3Layer Layer;

        public Text Text;
        public RectTransform SelectedMark;
        public Toggle VisibleToggle;

        public Action<LayerButton> ClickCallback;
        public Action<LayerButton, bool> VisibleCallback;

        protected override void on_create()
        {
            Text = find_component<Text>("Text");
            SelectedMark = find_component<RectTransform>("SelectedMark");
            VisibleToggle = register_toggle("VisibleToggle", on_visible_toggle);
            register_button(on_clicked);
        }

        public LayerButton Init (M3Layer layer, Action<LayerButton> click_callback, Action<LayerButton, bool> visible_callback)
        {
            Layer = layer;
            Text.text = layer.Index.ToString();
            name = layer.Index.ToString();
            ClickCallback = click_callback;
            VisibleCallback = visible_callback;
            Show();
            SetSelected(false);
            return this;
        }

        private void on_clicked ()
        {
            ClickCallback?.Invoke(this);
        }

        private void on_visible_toggle (bool value)
        {
            VisibleCallback?.Invoke(this, value);
        }

        public void SetSelected (bool value)
        {
            SelectedMark.SetActive(value);
        }
    }

    public M3Editor Editor;
    public M3Panel CurrentPanel;
    public List<LayerButton> LayerButtonList;
    public M3Layer CurrentLayer;

    private RectTransform _layer_list_content_rt;
    private GameObject _button_template;

    protected override void on_create()
    {
        _layer_list_content_rt = find_component<RectTransform>("LayerListScrollView/Viewport/Content");
        _button_template = find_component<RectTransform>("ButtonTemplate", _layer_list_content_rt).gameObject;
        _button_template.SetActive(false);

        register_button("LayerControlGroup/NewLayerButton", on_new_layer_clicked);
        register_button("LayerControlGroup/DeleteTopLayerButton", on_delete_top_layer_clicked);
        register_button("LayerControlGroup/DeleteSelectedLayerButton", on_delete_selected_layer_clicked);
        register_button("LayerControlGroup/UpSelectedLayer", on_up_selected_layer_clicked);
        register_button("LayerControlGroup/DownSelectedLayer", on_down_selected_layer_clicked);

        LayerButtonList = new List<LayerButton>();
        CurrentLayer = null;
    }

    public LayerUI Init (M3Editor editor)
    {
        Editor = editor;
        return this;
    }

    public void ClearLayerButtonList ()
    {
        foreach (var layer_button in LayerButtonList)
            destroy_ui(layer_button);
        LayerButtonList.Clear();
    }

    public void OnRefreshLayerList ()
    {
        Editor.CellEditUI.OnRefreshLayerList();

        ClearLayerButtonList();

        if (CurrentPanel != null && CurrentPanel.LayerList != null)
        {
            for (var i = 0; i < CurrentPanel.LayerList.Count; ++i)
            {
                var layer = CurrentPanel.LayerList[i];
                layer.Index = i;
                var layer_button = create_ui<LayerButton>(_button_template, _layer_list_content_rt);
                layer_button.Init(layer, l => on_selected_layer(l.Layer), on_layer_visible);
                LayerButtonList.Add(layer_button);
            }
        }
        sort_layer_button();
        on_selected_layer(null);
    }

    private void sort_layer_button ()
    {
        foreach (var layer_button in LayerButtonList)
            layer_button.transform.SetAsFirstSibling();
    }

    private void on_selected_layer (M3Layer layer)
    {
        if (CurrentLayer != layer)
        {
            var current_panel_button = LayerButtonList.Find(a => a.Layer == CurrentLayer);
            if (current_panel_button)
                current_panel_button.SetSelected(false);

            CurrentLayer = layer;
            current_panel_button = LayerButtonList.Find(a => a.Layer == CurrentLayer);
            if (current_panel_button)
                current_panel_button.SetSelected(true);

            Editor.CellEditUI.SelectLayer(CurrentLayer);
        }
    }

    private void on_layer_visible (LayerButton layer_button, bool visible)
    {
        Editor.CellEditUI.SetLayerVisible(layer_button.Layer, visible);
    }

    public void SetEditMode (bool value)
    {
        foreach (var layer_button in LayerButtonList)
            layer_button.VisibleToggle.SetIsOnWithoutNotify(!value);
    }

    public void SelectPanel (M3Panel panel)
    {
        if (CurrentPanel != panel)
        {
            CurrentPanel = panel;

            OnRefreshLayerList();
        }
    }

    private void on_new_layer_clicked ()
    {
        if (CurrentPanel == null)
        {
            Editor.ShowError("需要先选中一个布局");
            return;
        }

        if (CurrentPanel.LayerList == null)
            CurrentPanel.LayerList = new List<M3Layer>();

        var new_layer = new M3Layer
        {
            Index = CurrentPanel.LayerList.Count,
        };
        CurrentPanel.LayerList.Add(new_layer);
        OnRefreshLayerList();
        on_selected_layer(new_layer);
        CurrentPanel.IsDirty.Value = true;
    }

    private void on_delete_top_layer_clicked ()
    {
        if (CurrentPanel == null)
        {
            Editor.ShowError("需要先选中一个布局");
            return;
        }

        if (CurrentPanel.LayerList == null || CurrentPanel.LayerList.Count == 0)
        {
            Editor.ShowError("当前布局没有层");
            return;
        }

        Editor.ShowMessage($"删除层【{CurrentPanel.LayerList.Count}】成功");
        CurrentPanel.LayerList.RemoveAt(CurrentPanel.LayerList.Count - 1);
        OnRefreshLayerList();
        CurrentPanel.IsDirty.Value = true;
    }

    private void on_delete_selected_layer_clicked ()
    {
        if (CurrentPanel == null)
        {
            Editor.ShowError("需要先选中一个布局");
            return;
        }

        if (CurrentLayer == null)
        {
            Editor.ShowError("需要先选中一层");
            return;
        }
        
        foreach (var layer in CurrentPanel.LayerList)
        {
            if (layer.Index > CurrentLayer.Index)
                layer.IndexDown();
        }
        Editor.ShowMessage($"删除层【{CurrentLayer.Index}】成功");
        CurrentPanel.LayerList.Remove(CurrentLayer);
        OnRefreshLayerList();
        CurrentPanel.IsDirty.Value = true;
    }

    private void on_up_selected_layer_clicked ()
    {
        if (CurrentPanel == null)
        {
            Editor.ShowError("需要先选中一个布局");
            return;
        }

        if (CurrentLayer == null)
        {
            Editor.ShowError("需要先选中一层");
            return;
        }
        
        if (CurrentLayer.Index == CurrentPanel.LayerList.Count - 1)
        {
            Editor.ShowError("选中层已经是最上层");
            return;
        }
        
        var layer = CurrentLayer;
        CurrentLayer.IndexUp();
        LayerButtonList[CurrentLayer.Index].Layer.IndexDown();
        CurrentPanel.LayerList.Sort((a, b) => a.Index.CompareTo(b.Index));
        OnRefreshLayerList();
        on_selected_layer(layer);
        CurrentPanel.IsDirty.Value = true;
    }

    private void on_down_selected_layer_clicked ()
    {
        if (CurrentPanel == null)
        {
            Editor.ShowError("需要先选中一个布局");
            return;
        }

        if (CurrentLayer == null)
        {
            Editor.ShowError("需要先选中一层");
            return;
        }
        
        if (CurrentLayer.Index == 0)
        {
            Editor.ShowError("选中层已经是最下层");
            return;
        }
        
        var layer = CurrentLayer;
        CurrentLayer.IndexDown();
        LayerButtonList[CurrentLayer.Index].Layer.IndexUp();
        CurrentPanel.LayerList.Sort((a, b) => a.Index.CompareTo(b.Index));
        OnRefreshLayerList();
        on_selected_layer(layer);
        CurrentPanel.IsDirty.Value = true;
    }
}

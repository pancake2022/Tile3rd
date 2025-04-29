using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class CellEditUI : BaseUI
{
    public class CellUI : BaseUI
    {
        public M3Cell Cell;
        public Image Image;
        public RectTransform BrushFixedSignRT;
        public bool IsEdit;

        protected override void on_create()
        {
            Image = find_component<Image>("CellBG");
            BrushFixedSignRT = find_component<RectTransform>("BrushFixedSign");
        }

        public CellUI Init (M3Cell cell, bool is_edit)
        {
            Cell = cell;
            IsEdit = is_edit;
            RefreshImage();
            if (IsEdit)
                SetHighlight(false, M3Const.CellTypeEmpty);
            return this;
        }

        public void RefreshImage ()
        {
            if (IsEdit)
            {
                Show();
                Image.sprite = find_sprite("M3Tile01", Cell.Type == M3Const.CellTypeEmpty ? "e00" : "e" + Cell.Type.ToString("D2"));
                BrushFixedSignRT.SetActive(!Cell.IsBF);
            }
            else
            {
                if (Cell.Type == M3Const.CellTypeEmpty)
                {
                    Hide();
                }
                else
                {
                    Show();
                    Image.sprite = find_sprite("M3Tile01", "e" + Cell.Type.ToString("D2"));
                    BrushFixedSignRT.SetActive(!Cell.IsBF);
                }
            }
        }

        public CellUI RefreshPosition ()
        {
            var rt = transform as RectTransform;
            rt.anchoredPosition = new Vector2(Cell.X * rt.rect.size.x * 0.5f, Cell.Y * rt.rect.size.y * 0.5f);
            name = $"({Cell.X},{Cell.Y})";
            return this;
        }

        public void SetHighlight (bool value, int high_light_type)
        {
            if (Cell.Type == M3Const.CellTypeEmpty)
            {
                var color = Image.color;
                color.a = value ? 0.6f : 0.2f;
                Image.color = color;
            }
            else
            {
                var color = Image.color;
                color.a = 1.0f;
                Image.color = color;
            }

            if (value && high_light_type != M3Const.CellTypeEmpty)
                Image.sprite = find_sprite("M3Tile01", "e" + high_light_type.ToString("D2"));
        }

        public CellUI Brush (int type, bool force_write)
        {
            if (Cell.Type != type || force_write)
            {
                Cell.Type = type;
                Cell.IsBF = type != M3Const.CellTypeRandom;
                Debug.Log("1=" + Cell.Type);
            }
            else
            {
                Cell.Type = M3Const.CellTypeEmpty;
                Cell.IsBF = false;
                Debug.Log("2");
            }

            Image.sprite = find_sprite("M3Tile01", Cell.Type == M3Const.CellTypeEmpty ? "e00" : "e" + Cell.Type.ToString("D2"));
            BrushFixedSignRT.SetActive(!Cell.IsBF);
            Debug.Log("3");
            return this;
        }
    }

    public class LayerUI : BaseUI
    {
        public M3Layer Layer;
        public CellUI[,] CellArray;
        private GameObject _cell_template;
        private RectTransform _content_rt;

        protected override void on_create()
        {
            _content_rt = find_component<RectTransform>("Content");
        }

        public LayerUI InitEdit (GameObject cell_template)
        {
            CellArray = new CellUI[M3Const.LayerSize + 1, M3Const.LayerSize + 1];
            _cell_template = cell_template;

            M3Layer.Foreach(false, (x, y) => 
            {
                var cell_ui = create_ui<CellUI>(_cell_template, _content_rt).Init(new M3Cell
                {
                    Type = M3Const.CellTypeEmpty,
                    X = x,
                    Y = y,
                }, true).RefreshPosition();
                CellArray[x, y] = cell_ui;
            });
            
            M3Layer.Foreach(true, (x, y) => 
            {
                var cell_ui = create_ui<CellUI>(_cell_template, _content_rt).Init(new M3Cell
                {
                    Type = M3Const.CellTypeEmpty,
                    X = x,
                    Y = y,
                }, true).RefreshPosition();
                CellArray[x, y] = cell_ui;
            });

            name = "EditingLayer";
            return this;
        }

        public void SyncFromLayer (LayerUI layer_ui)
        {
            if (layer_ui == null)
            {
                M3Layer.Foreach((x, y) => CellArray[x, y].Hide());
                return;
            }

            M3Layer.Foreach(!layer_ui.Layer.IsOffset, (x, y) => 
            {
                CellArray[x, y].Hide();
            });

            M3Layer.Foreach(layer_ui.Layer.IsOffset, (x, y) => 
            {
                var cell_ui = CellArray[x, y];
                cell_ui.Show();
                cell_ui.Brush(M3Const.CellTypeEmpty, true);
                cell_ui.SetHighlight(false, M3Const.CellTypeEmpty);
            });

            foreach (var cell in layer_ui.Layer.CellList)
            {
                var cell_ui = CellArray[cell.X, cell.Y];
                cell_ui.Brush(cell.Type, true);
                cell_ui.SetHighlight(true, M3Const.CellTypeEmpty);
            }

            (transform as RectTransform).anchoredPosition = (layer_ui.transform as RectTransform).anchoredPosition;
        }

        public void SyncFromEditLayer (LayerUI edit_layer)
        {
            Layer.CellList = new List<M3Cell>();
            M3Layer.Foreach(Layer.IsOffset, (x, y) => 
            {
                var cell_ui = edit_layer.CellArray[x, y];
                if (cell_ui.Cell.Type != M3Const.CellTypeEmpty)
                    Layer.CellList.Add(new M3Cell(cell_ui.Cell));
            });

            Refresh(Layer);
        }

        private void sort_cell_ui ()
        {
            M3Layer.Foreach(Layer.IsOffset, (x, y) => 
            {
                var cell_ui = CellArray[x, y];
                if (cell_ui)
                    cell_ui.transform.SetAsLastSibling();
            });
        }

        private CellUI ensure_get_cell_ui (M3Cell cell)
        {
            var cell_ui = CellArray[cell.X, cell.Y];
            if (cell_ui == null)
            {
                CellArray[cell.X, cell.Y] = create_ui<CellUI>(_cell_template, _content_rt).Init(cell, false).RefreshPosition();
            }
            else
            {
                cell_ui.Init(cell, false);
            }
            return cell_ui;
        }

        public LayerUI Init (GameObject cell_template)
        {
            CellArray = new CellUI[M3Const.LayerSize + 1, M3Const.LayerSize + 1];
            _cell_template = cell_template;
            Layer = null;
            return this;
        }

        public LayerUI Refresh (M3Layer layer)
        {
            Layer = layer;
            foreach (var cell_ui in CellArray)
            {
                if (cell_ui)
                    cell_ui.Hide();
            }
            if (Layer != null)
            {
                foreach (var cell in Layer.CellList)
                    ensure_get_cell_ui(cell);

                name = layer.Index.ToString();

                // sort
                sort_cell_ui();
            }
            return this;
        }
    }

    public class BrushUI : BaseUI
    {
        public RectTransform SelectedMark;
        public Image BrushImage;
        public Text Text;
        public int Type;
        public Action<int> SelectedCallback;

        protected override void on_create()
        {
            SelectedMark = find_component<RectTransform>("SelectedMark");
            BrushImage = find_component<Image>("BrushImage");
            Text = find_component<Text>("Text");
            register_button(on_clicked);
        }

        private void on_clicked ()
        {
            SelectedCallback?.Invoke(Type);
        }

        public BrushUI Init (int type, Action<int> callback)
        {
            Type = type;
            SelectedCallback = callback;
            BrushImage.sprite = find_sprite("M3Tile01", "e" + type.ToString("D2"));
            Text.text = "x0";
            SetSelected(false);
            return this;
        }

        public BrushUI Refresh (int count, bool pass)
        {
            Text.text = $"x{count}";
            Text.color = pass ? Color.black : Color.red;
            return this;
        }

        public void SetSelected (bool value)
        {
            SelectedMark.SetActive(value);
        }
    }

    public M3Editor Editor;
    public List<LayerUI> LayerUIList;
    public LayerUI CurrentEditLayer;
    public bool IsBrushStatusPass;
    public bool InEditing;

    private RectTransform _content_rt;
    private GameObject _layer_template;
    private GameObject _cell_template;
    private RectTransform _brush_content_rt;
    private GameObject _brush_template;
    private List<BrushUI> _brush_list;
    private int _selected_brush_type;
    private LayerUI _edit_layer;
    private CellUI _current_cell;

    protected override void on_create()
    {
        _content_rt = find_component<RectTransform>("Mask/Content");
        _layer_template = find_component<RectTransform>("LayerTemplate", _content_rt).gameObject;
        _layer_template.SetActive(false);
        _cell_template = find_component<RectTransform>("CellTemplate", _content_rt).gameObject;
        _cell_template.SetActive(false);

        _brush_content_rt = find_component<RectTransform>("BrushScrollView/Viewport/Content");
        _brush_template = find_component<RectTransform>("BrushTemplate", _brush_content_rt).gameObject;
        _brush_template.SetActive(false);
        _brush_list = new List<BrushUI>();
        for (var i = 0; i <= M3Const.CellTypeCount; ++i)
        {
            var brush = create_ui<BrushUI>(_brush_template, _brush_content_rt).Init(i, on_brush_clicked);
            brush.Show();
            _brush_list.Add(brush);
        }
        _selected_brush_type = 0;
        _brush_list[_selected_brush_type].SetSelected(true);

        _edit_layer = create_ui<LayerUI>(_layer_template, _content_rt).InitEdit(_cell_template);
        _current_cell = null;
        InEditing = false;

        LayerUIList = new List<LayerUI>();
    }

    private void on_brush_clicked (int type)
    {
        _brush_list[_selected_brush_type].SetSelected(false);
        _selected_brush_type = type;
        _brush_list[_selected_brush_type].SetSelected(true);
    }

    private void refresh_brush_status ()
    {
        var dict = new Dictionary<int, int>();
        if (InEditing && CurrentEditLayer != null)
        {
            foreach (var layer in Editor.LayerUI.CurrentPanel.LayerList)
            {
                if (layer != CurrentEditLayer.Layer)
                    append_brush_count(dict, layer);
            }

            M3Layer.Foreach(CurrentEditLayer.Layer.IsOffset, (x, y) => 
            {
                var cell = _edit_layer.CellArray[x, y].Cell;
                dict[cell.Type] = dict.EnsureGet(cell.Type) + 1;
            });
        }
        else if (Editor.LayerUI.CurrentPanel != null)
        {
            foreach (var layer in Editor.LayerUI.CurrentPanel.LayerList)
                append_brush_count(dict, layer);
        }

        IsBrushStatusPass = true;
        foreach (var brush_ui in _brush_list)
        {
            var count = dict.EnsureGet(brush_ui.Type);
            var pass = count % 3 == 0;
            brush_ui.Refresh(count, pass);
            IsBrushStatusPass &= pass;
        }
    }

    private void append_brush_count (Dictionary<int, int> dict, M3Layer layer)
    {
        foreach (var cell in layer.CellList)
            dict[cell.Type] = dict.EnsureGet(cell.Type) + 1;
    }

    public CellEditUI Init (M3Editor editor)
    {
        Editor = editor;
        return this;
    }

    public void RefreshLayerUIList (M3Panel panel)
    {
        var panel_layer_count = panel.LayerList != null ? panel.LayerList.Count : 0;
        var count = Mathf.Max(panel_layer_count, LayerUIList.Count);
        for (var i = 0; i < count; ++i)
        {
            if (i < panel_layer_count)
            {
                if (i >= LayerUIList.Count)
                {
                    var ui = create_ui<LayerUI>(_layer_template, _content_rt).Init(_cell_template);
                    ui.transform.SetAsLastSibling();
                    LayerUIList.Add(ui);
                }

                var layer = panel.LayerList[i];
                var layer_ui = LayerUIList[i];
                layer_ui.Show();
                layer_ui.Refresh(layer);
                // (layer_ui.transform as RectTransform).anchoredPosition = new Vector2(-12 * layer.Index, 12 * layer.Index);
            }
            else
            {
                LayerUIList[i].Hide();
            }
        }
    }

    public void RefreshLayer (M3Layer layer)
    {
        var layer_ui = LayerUIList.Find(a => a.Layer == layer);
        if (layer_ui)
        {
            layer_ui.Refresh(layer);
            if (layer_ui == CurrentEditLayer)
                _edit_layer.SyncFromLayer(CurrentEditLayer);
        }
    }

    public void OnRefreshLayerList ()
    {
        RefreshLayerUIList(Editor.LayerUI.CurrentPanel);
        refresh_brush_status();
    }

    public void SetLayerVisible (M3Layer layer, bool visible)
    {
        var layer_ui = LayerUIList.Find(a => a.Layer == layer);
        if (layer_ui)
        {
            if (visible)
                layer_ui.Show();
            else
                layer_ui.Hide();
        }
    }

    public void SelectLayer (M3Layer layer)
    {
        if (InEditing)
        {
            if (CurrentEditLayer)
                CurrentEditLayer.SyncFromEditLayer(_edit_layer);
            CurrentEditLayer = LayerUIList.Find(a => a.Layer == layer);
            _edit_layer.SyncFromLayer(CurrentEditLayer);
        }
    }

    public void SetEditMode (bool value)
    {
        InEditing = value;
        _current_cell = null;
        if (InEditing)
        {
            foreach (var layer_ui in LayerUIList)
                layer_ui.Hide();
            
            CurrentEditLayer = LayerUIList.Find(a => a.Layer == Editor.LayerUI.CurrentLayer);
            _edit_layer.SyncFromLayer(CurrentEditLayer);
            _edit_layer.Show();
        }
        else
        {
            if (CurrentEditLayer)
                CurrentEditLayer.SyncFromEditLayer(_edit_layer);
            _edit_layer.Hide();

            RefreshLayerUIList(Editor.LayerUI.CurrentPanel);
        }
    }

    private void Update ()
    {
        if (InEditing && CurrentEditLayer)
        {
            var camera = _ui_manager.Framework.Context.UICamera;
            CellUI current_cell = null;
            M3Layer.Foreach(CurrentEditLayer.Layer.IsOffset, (x, y) => 
            {
                var cell = _edit_layer.CellArray[x, y];
                if (!cell.gameObject.activeSelf) return true;

                if (RectTransformUtility.RectangleContainsScreenPoint(cell.transform as RectTransform, Input.mousePosition, camera))
                {
                    current_cell = cell;
                    return true;
                }
                return false;
            });

            if (_current_cell != current_cell)
            {
                if (_current_cell)
                {
                    _current_cell.SetHighlight(false, M3Const.CellTypeEmpty);
                    _current_cell.RefreshImage();
                }

                _current_cell = current_cell;
            }

            if (_current_cell)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _current_cell.Brush(_selected_brush_type, false);
                    refresh_brush_status();
                    Editor.LayerUI.CurrentPanel.IsDirty.Value = true;
                }

                _current_cell.SetHighlight(true, _selected_brush_type);
            }
        }
    }
}

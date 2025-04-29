using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PanelUI : BaseUI
{
    public class PanelButton : BaseUI
    {
        public M3Panel Panel;

        public Text Text;
        public RectTransform SelectedMark;
        public Action<PanelButton> ClickCalback;

        protected override void on_create()
        {
            Text = find_component<Text>("Text");
            SelectedMark = find_component<RectTransform>("SelectedMark");
            register_button(on_clicked);
        }

        private void on_clicked ()
        {
            ClickCalback?.Invoke(this);
        }

        protected override void on_destroy()
        {
            if (Panel != null)
                Panel.IsDirty.OnValueChanged += on_refresh_dirty;
        }

        public PanelButton Init (M3Panel panel, Action<PanelButton> click_callback)
        {
            Panel = panel;
            ClickCalback = click_callback;
            Show();
            SetSelected(false);
            Panel.IsDirty.OnValueChanged += on_refresh_dirty;
            on_refresh_dirty(Panel.IsDirty.Value);
            return this;
        }

        private void on_refresh_dirty (bool is_dirty)
        {
            var prefix = is_dirty ? "*" : "";
            Text.text = $"{prefix}{Panel.DispalyName}";
        }

        public void SetSelected (bool value)
        {
            SelectedMark.SetActive(value);
        }
    }
    
    public int PerPagePanelCount = 100;
    public M3Editor Editor;
    public List<PanelButton> PanelButtonList;
    public M3Panel CurrentPanel;
    public int CurrentPageIndex;
    public int MaxPageIndex => (Editor.AllPanelList.Count - 1) / PerPagePanelCount;
    private RectTransform _panel_button_list_content_rt;
    private GameObject _panel_button_template;
    private InputField _page_navigation_input;

    private Button _left_button;
    private Button _right_button;
    private Button _left_end_button;
    private Button _right_end_button;

    protected override void on_create()
    {
        _panel_button_list_content_rt = find_component<RectTransform>("PanelListScrollView/Viewport/Content");
        _panel_button_template = find_component<RectTransform>("ButtonTemplate", _panel_button_list_content_rt).gameObject;
        _panel_button_template.SetActive(false);

        _page_navigation_input = find_component<InputField>("PageNavigation/PageNavigation");
        _page_navigation_input.onEndEdit.RemoveListener(on_page_input);
        _page_navigation_input.onEndEdit.AddListener(on_page_input);

        _left_button = register_button("PageNavigation/LeftButton", on_left_clicked);
        _right_button = register_button("PageNavigation/RightButton", on_right_clicked);
        _left_end_button = register_button("PageController/LeftEndButton", on_left_end_clicked);
        _right_end_button = register_button("PageController/RightEndButton", on_right_end_clicked);

        PanelButtonList = new List<PanelButton>();
        CurrentPanel = null;
    }

    public PanelUI Init (M3Editor editor)
    {
        Editor = editor;
        return this;
    }

    public void ClearPanelButtonList ()
    {
        foreach (var panel_button in PanelButtonList)
            destroy_ui(panel_button);
        PanelButtonList.Clear();
    }

    public void RefreshPanelButtonList ()
    {
        // clear
        ClearPanelButtonList();

        var start_index = CurrentPageIndex * PerPagePanelCount;
        var count = Mathf.Min(Editor.AllPanelList.Count - start_index, PerPagePanelCount);
        var panel_list = Editor.AllPanelList.GetRange(start_index, count);
        // create
        foreach (var panel in panel_list)
        {
            var panel_button = create_ui<PanelButton>(_panel_button_template, _panel_button_list_content_rt);
            panel_button.Init(panel, p => on_panel_selected(p.Panel));
            PanelButtonList.Add(panel_button);
        }
    }

    public PanelUI InitPanelList ()
    {
        CurrentPanel = null;
        CurrentPageIndex = 0;

        RefreshPanelButtonList();
        refresh_page_button_status();
        return this;
    }

    public void SelectPanel (M3Panel panel)
    {
        var index = Editor.AllPanelList.IndexOf(panel);
        CurrentPageIndex = index / PerPagePanelCount;
        RefreshPanelButtonList();
        on_panel_selected(panel);
    }

    public void on_panel_selected (M3Panel panel)
    {
        if (CurrentPanel != panel)
        {
            var current_panel_button = PanelButtonList.Find(a => a.Panel == CurrentPanel);
            if (current_panel_button)
                current_panel_button.SetSelected(false);

            CurrentPanel = panel;
            current_panel_button = PanelButtonList.Find(a => a.Panel == CurrentPanel);
            if (current_panel_button)
                current_panel_button.SetSelected(true);

            Editor.LayerUI.SelectPanel(CurrentPanel);
            Editor.CellEditControlUI.SelectPanel(CurrentPanel);
        }
    }

    private void on_page_input (string content)
    {
        if (int.TryParse(content, out var page_index))
        {
            CurrentPageIndex = Mathf.Clamp(page_index, 0, MaxPageIndex);
            RefreshPanelButtonList();
            refresh_page_button_status();
        }
        else
        {
            Editor.ShowError("请输入正确的页码");
        }
    }

    private void on_left_clicked ()
    {
        --CurrentPageIndex;
        RefreshPanelButtonList();
        refresh_page_button_status();
    }

    private void on_right_clicked ()
    {
        ++CurrentPageIndex;
        RefreshPanelButtonList();
        refresh_page_button_status();
    }

    private void on_left_end_clicked ()
    {
        CurrentPageIndex = 0;
        RefreshPanelButtonList();
        refresh_page_button_status();
    }

    private void on_right_end_clicked ()
    {
        CurrentPageIndex = MaxPageIndex;
        RefreshPanelButtonList();
        refresh_page_button_status();
    }

    private void refresh_page_button_status ()
    {
        var left = CurrentPageIndex > 0;
        _left_button.interactable = left;
        _left_end_button.interactable = left;

        var right = CurrentPageIndex < MaxPageIndex;
        _right_button.interactable = right;
        _right_end_button.interactable = right;

        (_page_navigation_input.placeholder as Text).text = $"{CurrentPageIndex + 1}/{MaxPageIndex + 1}";
        _page_navigation_input.text = "";
    }
}

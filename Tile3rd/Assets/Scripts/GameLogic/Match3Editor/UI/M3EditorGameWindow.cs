using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class M3EditorGameWindow : WindowUI
{
    public static new string DefaultPrefabPath = "M3Editor/M3EditorGameWindow";

    private M3GamePanelUI _panel_ui;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");

        _panel_ui = create_ui<M3GamePanelUI>("Panel");
        register_button("Close", Close);
    }

    public M3EditorGameWindow Init(M3Panel panel)
    {
        _panel_ui.Init(panel);
        return this;
    }
}
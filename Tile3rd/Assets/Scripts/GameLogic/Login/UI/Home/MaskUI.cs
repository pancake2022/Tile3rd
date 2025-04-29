using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MaskUI : WindowUI
{
    public static new string DefaultPrefabPath = "Panel/UI_Panel_mask";

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
    }
}
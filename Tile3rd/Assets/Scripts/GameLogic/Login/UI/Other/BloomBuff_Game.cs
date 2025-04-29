using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BloomBuff_Game : WindowUI
{
    public GameItemGroupUI ItemGroupUI;

    public BloomBuff_Game Init(GameItemGroupUI itemGroupUI)//PanelUI的初始化
    {
        ItemGroupUI = itemGroupUI;
        return this;
    }
    protected override void on_create()
    {
        //道具默认不显示
        var item = find_component<RectTransform>("Panel");
        for (int i = 0; i < item.childCount; i++)
        {
            var pic = item.transform.GetChild(i);
            pic.SetActive(false);
        }

        //道具显示
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        for (int i = 0; i < tile2storage.BloomBuffTimes; i++)
        {
            var pic = item.transform.GetChild(i);
            pic.SetActive(true);
        }
    }
}

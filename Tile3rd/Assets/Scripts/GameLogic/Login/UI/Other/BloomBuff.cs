using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BloomBuff : WindowUI
{
    public HomeUI Home;

    public BloomBuff Init(HomeUI home)//PanelUI的初始化
    {
        Home = home;
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
        for (int i = 0; i < GameConfigManager.Tile2Storage.BloomBuffTimes; i++)
        {
            var pic = item.transform.GetChild(i);
            pic.SetActive(true);
        }
    }
}

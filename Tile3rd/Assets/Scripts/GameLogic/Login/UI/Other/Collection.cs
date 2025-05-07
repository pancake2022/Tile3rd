using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Collection : WindowUI
{
    public HomeUI Home;
    private int nextTileID;
    public CollectionConfig currentTile;

    public Collection Init(HomeUI home)//PanelUI的初始化
    {
        Home = home;
        return this;
    }
    protected override void on_create()
    {
        register_button("Panel/icon", on_clicked);
        register_button("Panel/defaulticon", on_default_clicked);
        ShowInit();
    }

    public void ShowInit()
    {
        var collectionlist = GameConfigManager.GameConfigGroup.CollectionConfigList;
        var iconshow = find_component<RectTransform>("Panel/icon");
        var defaulticon = find_component<RectTransform>("Panel/defaulticon");
        var slidersshow = find_component<RectTransform>("Panel/slider");
        defaulticon.SetActive(false);
        iconshow.SetActive(false);
        slidersshow.SetActive(false);

        if (!GameConfigManager.Tile2Storage.TileUnlock.Values.Contains(false))
            defaulticon.SetActive(true);
        else
        {
            if (collectionlist.Any (item => item.Type == 2 && !GameConfigManager.Tile2Storage.TileUnlock[item.ID]))
            {
                currentTile = collectionlist.Where(item => item.Type == 2 && !GameConfigManager.Tile2Storage.TileUnlock[item.ID])  // 条件过滤
            .OrderBy(item => item.ID).FirstOrDefault();// 按 ID 升序排序

                iconshow.SetActive(true);
                slidersshow.SetActive(true);
                IconShow();
                SliderShow();
            }
            else
            {
                currentTile = collectionlist.Where(item => !GameConfigManager.Tile2Storage.TileUnlock[item.ID])  // 条件过滤
            .OrderBy(item => item.ID).FirstOrDefault();
                iconshow.SetActive(true);
                IconShow();
            }
        }
    }
    private void IconShow()
    {
        var icon = find_component<Image>("Panel/icon");
        icon.sprite = _ui_manager.FindSprite($"{currentTile.IconPack}", $"{currentTile.Icon}", true);
    }
    private void SliderShow()
    {
        int count = GameConfigManager.Tile2Storage.TileSingleUnlock
            .Where(item => item.Key >= currentTile.ID * 100 && item.Key <= currentTile.ID * 100 + currentTile.UnlockCount && item.Value)
            .Count();
        var slider = find_component<Slider>("Panel/slider");
        var slidertext = find_component<Text>("Panel/slider/Fill Area/Text");
        slider.value = count;
        slider.maxValue = currentTile.UnlockCount;
        slidertext.text = $"{slider.value}/{slider.maxValue}";
    }
    private void on_clicked()
    {
        _ui_manager.OpenWindow<CollectionUI>();
    }
    private void on_default_clicked()
    {
        _ui_manager.OpenWindow<CollectionUI>();
    }
}

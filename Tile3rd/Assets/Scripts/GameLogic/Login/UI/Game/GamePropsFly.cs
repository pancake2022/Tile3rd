using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePropsFly : WindowUI
{
    public static new string DefaultPrefabPath = "Game/Game_PropsFly";
    public GameUI Game;
    private Vector3 flyPosition;

    public GamePropsFly Init(GameUI game)
    {
        Game = game;
        return this;
    }

    public GamePropsFly StartFly()
    {
        FlyItemShow();
        return this;
    }

    public void FlyItemShow()
    {
        //广告礼包 - 消除
        if (GameConfigManager.ShareDataGlobalConfig._bundle_type_id == 3)
        {
            Game.gameItemGroupUI.GetStartPositon_Remove();
            flyPosition = Game.gameItemGroupUI.startPosition;
            ItemShow("M3Reward", "icon_game_delete", GameConfigManager.GlobalConfig.RV_Reward_Remove);
        }
        //广告礼包 - 回退
        if (GameConfigManager.ShareDataGlobalConfig._bundle_type_id == 4)
        {
            Game.gameItemGroupUI.GetStartPositon_Recall();
            flyPosition = Game.gameItemGroupUI.startPosition;
            ItemShow("M3Reward", "icon_game_back", GameConfigManager.GlobalConfig.RV_Reward_Recall);
        }
        //广告礼包 - 绽放
        if (GameConfigManager.ShareDataGlobalConfig._bundle_type_id == 5)
        {
            Game.gameItemGroupUI.GetStartPositon_Bloom();
            flyPosition = Game.gameItemGroupUI.startPosition;
            ItemShow("M3Reward", "icon_game_bloom", GameConfigManager.GlobalConfig.RV_Reward_Bloom);
        }
        transform.localPosition = flyPosition;
    }
    private void ItemShow(string itemAtlas, string spritename, int count)
    {
        var item = find_component<RectTransform>("Image/item_1");
        item.transform.SetActive(true);
        var itempic = find_component<Image>("Image/item_1");
        itempic.sprite = _ui_manager.FindSprite(itemAtlas, spritename, true);
        var itemtext = find_component<Text>("Image/item_1/Text");
        itemtext.text = "+" + count.ToString();
    }
}
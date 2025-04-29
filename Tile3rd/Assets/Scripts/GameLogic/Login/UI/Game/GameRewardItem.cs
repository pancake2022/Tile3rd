using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRewardItem : WindowUI
{
    public static new string DefaultPrefabPath = "Game/Game_Reward";
    public GameUI Game;
    public int game_reward_item_1 = 0;
    public bool BloomBuff;
    public int BloomTimes;

    public GameRewardItem Init(GameUI game)//PanelUI的初始化
    {
        Game = game;
        return this;
    }
    private void Update()//游戏更新
    {
        Reward_Item_Show();
    }
    public void Reward_Item_Show()
    {
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        var item_1 = find_component<RectTransform>("item_1");
        var item_2 = find_component<RectTransform>("item_2");
        var item_3 = find_component<RectTransform>("item_3");
        var text_item_1 = find_component<Text>("item_1/Text");
        var text_item_2 = find_component<Text>("item_2/Text");
        item_1.transform.SetActive(false);
        item_2.transform.SetActive(false);
        item_3.transform.SetActive(false);

        if (game_reward_item_1 >= 1)
            item_1.SetActive(true);
        if (Game.gameBG.bg_bloom_im.fillAmount == 1)
        {
            //如果是bloomAll状态，则显示3
            //否则显示2
            if (tile2storage.BloomAllTimes > 0)
                item_3.SetActive(true);
            else
                item_2.SetActive(true);
        }
            
        text_item_1.text = "+ " + game_reward_item_1.ToString();
        text_item_2.text = BloomTimes.ToString();
    }
}
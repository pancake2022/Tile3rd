using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameItemGroupUI : WindowUI
{
    public static new string DefaultPrefabPath = "Game/Game_ItemGroup";
    public M3GamePanelUI _panel_ui;
    public GameUI gameUI;
    public Vector3 startPosition;
    public BloomBuff_Game bloomBuff_Game;
    private Action<GameItemGroupUI> ClickCalback_Item;

    public GameItemGroupUI Init(GameUI game)
    {
        gameUI = game;
        gameUI.isPause = false;
        return this;
    }

    protected override void on_create()
    {
        //创建按钮
        register_button("item_recall/normal", on_recall_normal_clicked);
        register_button("item_eliminate/normal", on_eliminate_normal_clicked);
        register_button("item_flower/normal", on_flower_normal_clicked);

        //挂bloom状态的脚本
        var _bloom_buff = find_component<RectTransform>("item_flower/bloombuff");
        bloomBuff_Game = create_ui<BloomBuff_Game>("Game/Game_BloomBuff", _bloom_buff).Init(this);

        ItemGroupInit();
        ItemRefresh();
        BloomTipsInit();
    }
    private void ItemGroupInit()
    {
        var eliminate_lock = find_component<RectTransform>("item_eliminate/lock");
        var recall_lock = find_component<RectTransform>("item_recall/lock");
        var flower_lock = find_component<RectTransform>("item_flower/lock");
        var eliminate_normal = find_component<RectTransform>("item_eliminate/normal");
        var recall_normal = find_component<RectTransform>("item_recall/normal");
        var flower_normal = find_component<RectTransform>("item_flower/normal");
        var eliminate_num = find_component<Text>("item_eliminate/lock/Text");
        var recall_num = find_component<Text>("item_recall/lock/Text");
        var flower_num = find_component<Text>("item_flower/lock/Text");

        eliminate_lock.SetActive(false);
        recall_lock.SetActive(false);
        flower_lock.SetActive(false);
        eliminate_normal.SetActive(false);
        recall_normal.SetActive(false);
        flower_normal.SetActive(false);

        eliminate_num.text = "level " + GameConfigManager.GlobalConfig.Item_Remove_UnlockLevel.ToString();
        recall_num.text = "level " + GameConfigManager.GlobalConfig.Item_Recall_UnlockLevel.ToString();
        flower_num.text = "level 4";

        if (GameConfigManager.LevelStorage.LevelCount >= GameConfigManager.GlobalConfig.Item_Recall_UnlockLevel)
            recall_normal.SetActive(true);
        else
            recall_lock.transform.SetActive(true);

        if (GameConfigManager.LevelStorage.LevelCount >= GameConfigManager.GlobalConfig.Item_Remove_UnlockLevel)
            eliminate_normal.SetActive(true);
        else
            eliminate_lock.transform.SetActive(true);

        if (GameConfigManager.MakeOverStorage.CatQuestCondition[GameConfigManager.GlobalConfig.Item_Bloom_UnlockLevel] == 3) 
            flower_normal.SetActive(true);
        else
            flower_lock.transform.SetActive(true);

        level12give1bloom();
    }
    private void level12give1bloom()
    {
        if (GameConfigManager.LevelStorage.LevelCount == 12)
        {
            if (GameConfigManager.CommonStorage.Item_Bloom <= 0)
                GameConfigManager.CommonStorage.Item_Bloom++;
        }
    }
    //道具实时刷新
    public void ItemRefresh()
    {
        //数量实时显示
        var eliminate_num = find_component<Text>("item_eliminate/normal/Image/Text");
        var recall_num = find_component<Text>("item_recall/normal/Image/Text");
        var flower_num = find_component<Text>("item_flower/normal/Image/Text");
        recall_num.text = GameConfigManager.CommonStorage.Item_Recall.ToString();
        eliminate_num.text = GameConfigManager.CommonStorage.Item_Remove.ToString();
        flower_num.text = GameConfigManager.CommonStorage.Item_Bloom.ToString();
    }
    public void BloomTipsInit()
    {
        //flowertip的显示
        var bloomtip = find_component<RectTransform>("item_flower/normal/bloomtip");
        bloomtip.SetActive(false);
        if (GameConfigManager.LevelStorage.LevelCount >= 5)
        {
            if (GameConfigManager.LevelStorage.LevelCount != 11)
                bloomtip.SetActive(true);
        }
    }
    public void BloomTipsRefresh()
    {
        var bloomtip = find_component<RectTransform>("item_flower/normal/bloomtip");
        if (gameUI.gameRewardItem.BloomTimes <= 0)
            bloomtip.SetActive(true);
        else
            bloomtip.SetActive(false);
    }
    //normal - 回退
    private void on_recall_normal_clicked()
    {
        play_sound("sound_item_click");

        if (GameConfigManager.CommonStorage.Item_Recall >= 1)
        {
            if (gameUI._panel_ui.TryRevertGameOpt())
            {
                GameConfigManager.ShareDataGlobalConfig.itemlist[2]++;
                GameConfigManager.CommonStorage.Item_Recall--;
                GameConfigManager.ShareDataGlobalConfig._storybundle_check = true;
            }
            else
                StartCoroutine(WaitCheck_returntip());
        }
        else
        {
            if (gameUI._panel_ui.OptStack.Count == 0)//没有可以回退的就飘提示
                StartCoroutine(WaitCheck_returntip());
            else
            {
                GameConfigManager.ShareDataGlobalConfig._bundle_type_id = 4;
                _ui_manager.OpenWindow<OutItemRV>();
            }
        }
        ItemRefresh();
        ClickCalback_Item?.Invoke(this);
    }
    public IEnumerator WaitCheck_returntip()
    {
        play_sound("sound_item_click");
        var tip = find_component<RectTransform>("item_recall/tip");
        tip.transform.SetActive(true);
        yield return new WaitForSeconds(1f);
        tip.transform.SetActive(false);
    }

    //normal - 消除
    private void on_eliminate_normal_clicked()
    {
        play_sound("sound_item_click");
        if (gameUI.isPause == false)
        {
            if (GameConfigManager.CommonStorage.Item_Remove >= 1)
            {
                GameConfigManager.ShareDataGlobalConfig.itemlist[1]++;
                GameConfigManager.CommonStorage.Item_Remove--;
                StartCoroutine(WaitCheck_eliminate());
                GameConfigManager.ShareDataGlobalConfig._storybundle_check = true;
                gameUI.tileRandom.Condition_Count = 0;
            }
            else
            {
                GameConfigManager.ShareDataGlobalConfig._bundle_type_id = 3;
                _ui_manager.OpenWindow<OutItemRV>();
            }
        }
        ItemRefresh();
        ClickCalback_Item?.Invoke(this);
    }
    private IEnumerator WaitCheck_eliminate()
    {
        gameUI.isPause = true;
        eliminate();
        yield return new WaitForSeconds(1f);
        gameUI.isPause = false;
    }
    //消除道具的功能
    private void eliminate()
    {
        var eliminate_cell_ui = gameUI._panel_ui.get_eliminate_cell_ui();//list<M3CellUI>
        for (int i = 0; i < eliminate_cell_ui.Count; i++)//遍历list里的3个cell
        {
            var eliminate_cell_ui_i = eliminate_cell_ui[i];
            if (eliminate_cell_ui_i)
            {
                gameUI._panel_ui.HighLightCellUI.SetHighLight(true, eliminate_cell_ui_i.Cell);//就把牌各种设置为true,
                if (gameUI._panel_ui.CollectionUI.TryCollectCell(eliminate_cell_ui_i, gameUI._panel_ui.HighLightCellUI))//尝试收集并刷新ui
                    gameUI._panel_ui.refresh_cell_state();
            }
            else
                gameUI._panel_ui.HighLightCellUI.SetHighLight(false, null);//否则不行
        }
    }


    //normal - 小花
    private void on_flower_normal_clicked()
    {
        play_sound("sound_item_click");
        if (GameConfigManager.Tile2Storage.BloomAllTimes > 0)
        {
            StartCoroutine(WaitCheck_flowertip());
        }
        else
        {
            if (GameConfigManager.CommonStorage.Item_Bloom >= 1)
            {
                if (gameUI.isPause == false)
                {
                    GameConfigManager.ShareDataGlobalConfig.itemlist[3]++;
                    GameConfigManager.CommonStorage.Item_Bloom--;
                    FlowerBuff();
                    GameConfigManager.ShareDataGlobalConfig._storybundle_check = true;
                }
            }
            else
            {
                GameConfigManager.ShareDataGlobalConfig._bundle_type_id = 5;
                _ui_manager.OpenWindow<OutItemRV>();
            }
            ItemRefresh();
            BloomTipsRefresh();
            ClickCalback_Item?.Invoke(this);
        }
    }
    private IEnumerator WaitCheck_flowertip()
    {
        var tip2 = find_component<RectTransform>("item_flower/tip2");
        tip2.SetActive(true);
        yield return new WaitForSeconds(1f);
        tip2.SetActive(false);
    }
    private void FlowerBuff()
    {
        gameUI.SetItemBloomMusic();
        gameUI.gameRewardItem.BloomBuff = true;
        gameUI.gameRewardItem.BloomTimes = gameUI.gameRewardItem.BloomTimes + GameConfigManager.GlobalConfig.Bloom_Times_Item;
        var tip = find_component<RectTransform>("item_flower/tip");
        var tiptext = find_component<Text>("item_flower/tip/Image/Text");
        var anim = find_component<Animator>("item_flower/tip");
        tip.SetActive(true);
        tiptext.text = "+" + GameConfigManager.GlobalConfig.Bloom_Times_Item.ToString();
        anim.Play("Fly_tips", -1, 0f);
    }

    //复活判断
    public void ReviveCondition()
    {
        if (gameUI.isPause == false)
        {
            if (gameUI._panel_ui.CollectionUI.CollectedCellUIList.Count >= 7)
            {
                var matchcount = false;
                for (int i = 0; i < gameUI._panel_ui.CollectionUI.CollectedCellUIList.Count; i++)
                {
                    if (i >= 2)
                    {
                        var cell_1_type = gameUI._panel_ui.CollectionUI.CollectedCellUIList[i].Cell.Type;
                        var cell_2_type = gameUI._panel_ui.CollectionUI.CollectedCellUIList[i - 1].Cell.Type;
                        var cell_3_type = gameUI._panel_ui.CollectionUI.CollectedCellUIList[i - 2].Cell.Type;

                        if (cell_1_type == cell_2_type && cell_1_type == cell_3_type)//如果有3张相邻的牌一样
                            matchcount = true;
                    }
                }
                if (matchcount == false && gameUI._panel_ui.CollectionUI.isMatchPause == false) 
                    StartCoroutine(WaitOpen_reviveUI());
            }
        }
    }
    private IEnumerator WaitOpen_reviveUI()
    {
        gameUI.isPause = true;
        yield return new WaitForSeconds(0.6f);
        _ui_manager.OpenWindow<ReviveUI>();
    }
    //实施复活功能
    public void Revive()
    {
        StartCoroutine(WaitCheck_revive());
    }
    private IEnumerator WaitCheck_revive()
    {
        ReviveImplement();
        yield return new WaitForSeconds(0.8f);
        ReviveImplement();
        yield return new WaitForSeconds(0.8f);
        ReviveImplement();
        yield return new WaitForSeconds(0.8f);
        gameUI.isPause = false;
    }
    private void ReviveImplement()
    {
        gameUI.tileRandom.Condition_Count = 0;
        eliminate();
    }

    //拿到道具飞行位置
    public void GetStartPositon_Remove()
    {
        var remove_p = find_component<RectTransform>("item_eliminate");
        var v = remove_p.localPosition;
        v.x = remove_p.localPosition.x;
        v.y = -500;
        startPosition = v;
    }
    public void GetStartPositon_Recall()
    {
        var recall_p = find_component<RectTransform>("item_recall");
        var v = recall_p.localPosition;
        v.x = recall_p.localPosition.x;
        v.y = -500;
        startPosition = v;
    }
    public void GetStartPositon_Bloom()
    {
        var flower_p = find_component<RectTransform>("item_flower");
        var v = flower_p.localPosition;
        v.x = flower_p.localPosition.x;
        v.y = -500;
        startPosition = v;
    }
    public void CallBack_Item(Action<GameItemGroupUI> click_callback)
    {
        ClickCalback_Item = click_callback;
    }
}




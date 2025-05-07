using CSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBackground : BaseUI
{
    public GameUI gameUI;
    private Image bg_back_im;
    public Image bg_bloom_im;
    private RectTransform bg_back_rt;
    private RectTransform bg_bloom_rt;

    public GameBackground Init(GameUI game)
    {
        gameUI = game;
        return this;
    }
    protected override void on_create()
    {
        GameBGInit();
    }
    private void Update()//游戏更新
    {
        if (GameConfigManager.Tile2Storage.BloomAllTimes > 0)
        {
            if (GameConfigManager.Tile2Storage.BloomBuffTimes > 0)
                BloomBGShow(2);
            else
                BloomBGShow(1);
        }
        else
        {
            if (GameConfigManager.Tile2Storage.BloomBuffTimes > 0)
            {
                if (gameUI.gameRewardItem.BloomBuff)
                    BloomBGShow(2);
                else
                    BloomBGOff();
            }
            else
            {
                if (gameUI.gameRewardItem.BloomBuff)
                    BloomBGShow(1);
                else
                    BloomBGOff();
            }
        }
    }

    private void GameBGInit()
    {
        //bg_back_rt = find_component<RectTransform>("bg_back");
        bg_bloom_rt = find_component<RectTransform>("bg_bloom");
        bg_back_im = find_component<Image>("bg_back");
        bg_bloom_im = find_component<Image>("bg_bloom");
        //bg_back_rt.SetActive(true);
        bg_bloom_rt.SetActive(false);
        bg_bloom_im.fillAmount = 0;
    }

    public void BackGroundChange()
    {
        var alltile = GameConfigManager.GameConfigGroup.CollectionConfigList;
        var gamebg = alltile.Find(a => a.ID == GameConfigManager.Tile2Storage.CurrentTileID);
        bg_back_im.sprite = _ui_manager.FindSprite($"{gamebg.GameBGPack}", $"{gamebg.GameBG}", true);
    }
    public void BloomBGShow(int value)
    {
        if (value == 1)
            bg_bloom_im.sprite = _ui_manager.FindSprite($"M3BackGame", $"bg_201", true);
        if (value == 2)
            bg_bloom_im.sprite = _ui_manager.FindSprite($"M3BackGame", $"bg_202", true);

        bg_bloom_rt.SetActive(true);
        bg_bloom_im.fillOrigin = 2;

        //填充效果
        if (bg_bloom_im.fillAmount < 1)
        {
            bg_bloom_im.fillAmount = bg_bloom_im.fillAmount + (float)0.02;
            AlphaShow();
        }
    }
    public void BloomBGOff()
    {
        bg_bloom_im.fillOrigin = 1;
        if (bg_bloom_im.fillAmount > 0)
        {
            bg_bloom_im.fillAmount = bg_bloom_im.fillAmount - (float)0.02;
            AlphaOff();
        }
        else
            bg_bloom_rt.SetActive(false);
    }
    private void AlphaShow()
    {
        Color color = bg_bloom_im.color;
        if (color.a < 1)
        {
            color.a = color.a + 0.02f;
            bg_bloom_im.color = color;
        }
    }
    private void AlphaOff()
    {
        Color color = bg_bloom_im.color;
        if (color.a > 0)
        {
            color.a = color.a - 0.02f;
            bg_bloom_im.color = color;
        }
    }
}
using CSFramework;
using UnityEngine;
using UnityEngine.UI;

public class LoveLevel : BaseUI
{
    public HomeUI Home;
    private Slider levelExp;
    private Text expText;
    private Text levelText;
    private bool delayStart;
    public int delayCount;

    public LoveLevel Init(HomeUI home)//PanelUI的初始化
    {
        Home = home;
        return this;
    }
    protected override void on_create()
    {
        levelExp = find_component<Slider>("slider");
        expText = find_component<Text>("text");
        levelText = find_component<Text>("bubble/tip/image/Text");
        levelExp.maxValue = 100;

        if (GameConfigManager.Tile2Storage.LoveLevelExpUp > 0)
            GameConfigManager.ShareDataGlobalConfig._love_exp_pause = false;

        lovelevelInit();
    }
    private void Update()
    {
        levelExp.value = GameConfigManager.Tile2Storage.LoveLevelExp;
        expText.text = $"{GameConfigManager.Tile2Storage.LoveLevelExp / 10}/10";
        levelText.text = $"{ GameConfigManager.Tile2Storage.LoveLevelLevel}";
        
        if (GameConfigManager.ShareDataGlobalConfig._love_exp_pause == false)
        {
            if (GameConfigManager.Tile2Storage.LoveLevelExpUp > 0)
            {
                GameConfigManager.Tile2Storage.LoveLevelExpUp--;
                GameConfigManager.Tile2Storage.LoveLevelExp++;
            }
            order();
        }
        //延迟一些弹奖励窗口
        if(delayStart)
        {
            delayCount++;
            if (delayCount >= 10)
            {
                Home.makeOver.CurrentStoryCondition();
                _ui_manager.TryCloseWindow<MaskUI>();
                delayStart = false;
                delayCount = 0;
            }
        }
    }

    //弹窗顺序/1等级奖励/2story完成奖励
    private void order()
    {
        for (int i = 1; i <= 2; i++)
        {
            if (i == 1)
            {
                if (GameConfigManager.Tile2Storage.LoveLevelExp == 100)
                {
                    GameConfigManager.ShareDataGlobalConfig._love_exp_pause = true;
                    _ui_manager.OpenWindow<LoveLevelRewardUI>();
                    break;
                }
            }
            if (i == 2)
            {
                if (GameConfigManager.Tile2Storage.LoveLevelExpUp == 0)
                {
                    GameConfigManager.ShareDataGlobalConfig._love_exp_pause = true;
                    delayStart = true;
                }
            }
        }
    }
    private void lovelevelInit()
    {
        if (GameConfigManager.Tile2Storage.LoveLevelExp >= 100)
        {
            GameConfigManager.Tile2Storage.LoveLevelLevel++;
            GameConfigManager.Tile2Storage.LoveLevelExp = GameConfigManager.Tile2Storage.LoveLevelExp - 100;
        }
    }
}

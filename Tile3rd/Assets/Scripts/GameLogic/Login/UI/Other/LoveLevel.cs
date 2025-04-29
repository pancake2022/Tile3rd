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
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();

        levelExp = find_component<Slider>("slider");
        expText = find_component<Text>("text");
        levelText = find_component<Text>("bubble/tip/image/Text");
        levelExp.maxValue = 100;

        if (tile2storage.LoveLevelExpUp > 0)
            shareDataGlobalConfig._love_exp_pause = false;

        lovelevelInit();
    }
    private void Update()
    {
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        levelExp.value = tile2storage.LoveLevelExp;
        expText.text = $"{tile2storage.LoveLevelExp / 10}/10";
        levelText.text = $"{ tile2storage.LoveLevelLevel}";
        
        if (shareDataGlobalConfig._love_exp_pause == false)
        {
            if (tile2storage.LoveLevelExpUp > 0)
            {
                tile2storage.LoveLevelExpUp--;
                tile2storage.LoveLevelExp++;
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
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        for (int i = 1; i <= 2; i++)
        {
            if (i == 1)
            {
                if (tile2storage.LoveLevelExp == 100)
                {
                    shareDataGlobalConfig._love_exp_pause = true;
                    _ui_manager.OpenWindow<LoveLevelRewardUI>();
                    break;
                }
            }
            if (i == 2)
            {
                if (tile2storage.LoveLevelExpUp == 0)
                {
                    shareDataGlobalConfig._love_exp_pause = true;
                    delayStart = true;
                }
            }
        }
    }
    private void lovelevelInit()
    {
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        if (tile2storage.LoveLevelExp >= 100)
        {
            tile2storage.LoveLevelLevel++;
            tile2storage.LoveLevelExp = tile2storage.LoveLevelExp - 100;
        }
    }
}

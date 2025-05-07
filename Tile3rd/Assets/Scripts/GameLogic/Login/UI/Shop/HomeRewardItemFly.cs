using CSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HomeRewardItemFly : BaseUI
{
    public HomeUI Home;
    private SchedulerFloat _flower_scheduler = new SchedulerFloat().Init(1f);//花的飞行
    private SchedulerFloat _fly_scheduler = new SchedulerFloat().Init(0.7f);//心的飞行
    private Vector3 flyPosition;
    private Vector3 targetScale;
    private Vector3 flowerScale;
    public int flyType;
    private int currentdroptile;
    private int maxdroptile;
    private int lastdroptile;
    private int droptilemin;
    private int droptilemax;
    private int droptilecount;

    private List<ItemConfig> itemlist;
    private List<CollectionConfig> collectionlist;

    protected override void on_create()
    {
        itemlist = GameConfigManager.GameConfigGroup.ItemConfigList;
        collectionlist = GameConfigManager.GameConfigGroup.CollectionConfigList;

        var item = find_component<RectTransform>("Panel/Image/item");
        if (GameConfigManager.ShareDataGlobalConfig._home_fly == 2)
            item.SetActive(false);
        if (GameConfigManager.ShareDataGlobalConfig._home_fly == 4)
            item.SetActive(true);
    }

    public HomeRewardItemFly Init(HomeUI home)
    {
        Home = home;
        return this;
    }

    private void Update()//飞行轨迹
    {
        if (flyType == 1)
            FlowerFly();
        if (flyType == 4)
            HeartFly();
    }

    //开始飞行
    public HomeRewardItemFly StartFly()
    {
        var anim = find_component<Animator>("Panel");
        var image = find_component<RectTransform>("Panel/Image");

        //关卡宝箱
        if (GameConfigManager.ShareDataGlobalConfig._home_fly == 1)
        {
            Home.GetLevelChestPosition();
            flyPosition = Home.startPosition;
            FlyItemShow();
            anim.Play("Fly_tips");
        }
        //飞心
        if (GameConfigManager.ShareDataGlobalConfig._home_fly == 2)
        {
            Home.GetLoveLevelPosition();
            Home.makeOver.makeOver_CatImage.CatHeartPosition();
            flyPosition = Home.makeOver.makeOver_CatImage.localPosition;
            targetScale = new Vector3(0.7f, 0.7f);
            ItemShow("M3Reward", "icon_heart", true, 1);
            anim.Play("Fly_heart");
        }
        //飞小花
        if (GameConfigManager.ShareDataGlobalConfig._home_fly == 4)
        {
            anim.enabled = false;
            image.SetActive(true);
            Home.GetFlowerPosition();
            Home.makeOver.makeOver_Select.GetButtonPosition();
            flyPosition = Home.startPosition;
            flowerScale = new Vector3(1f, 1f);
            ItemShow("M3Reward", "icon_flower", false, 0);
            flyType = 1;
        }
        if (GameConfigManager.ShareDataGlobalConfig._home_fly == 5)
        {
            Home.GetDailyHintPosition();
            flyPosition = Home.startPosition;
            FlyHintRewardShow();
            anim.Play("Fly_tips");
        }
        transform.localPosition = flyPosition;
        return this;
    }

    private void FlowerFly()
    {
        var pre_percent = _flower_scheduler.Percent();
        if (_flower_scheduler.Tick(Time.deltaTime, false))
        {
            transform.localPosition = Home.makeOver.makeOver_Select.buttonPosition;
            transform.localScale = flowerScale;
            Home.ClearRewardFly();

            if (Home.makeOver.makeOver_Select.CurrentPanel.ImageType == 1)
            {
                Home.makeOver.makeOver_Select.SelectUIShow(true, false);
                Home.makeOver.makeOver_Image.SetMakeoverAnim(Home.makeOver.makeOver_Select.CurrentPanel);
            }
            if (Home.makeOver.makeOver_Select.CurrentPanel.ImageType == 2)
            {
                _ui_manager.OpenWindow<MakeOver_Notice>();
                Home.makeOver.makeOver_Select.SelectUIShow(true, false);
            }
        }
        else if (!_flower_scheduler.IsArrived())
        {
            var percent = _flower_scheduler.Percent();
            transform.localPosition = Utils.LerpByPrePercent(transform.localPosition, Home.makeOver.makeOver_Select.buttonPosition, pre_percent, percent);
            transform.localScale = Utils.LerpByPrePercent(transform.localScale, flowerScale, pre_percent, percent);
        }
    }
    public void HeartFly()
    {
        var pre_percent = _fly_scheduler.Percent();
        if (_fly_scheduler.Tick(Time.deltaTime, false))
        {
            transform.localPosition = Home.startPosition;
            transform.localScale = targetScale;
            GameConfigManager.ShareDataGlobalConfig._love_exp_pause = false;//需要处理
            Home.ClearRewardFly();
            Home.makeOver.MakeOverUI_SelectClose();

            ////任务不为空的情况下，刷新任务
            if (Home.catQuest != null)
                Home.catQuest.InitCatQuest();
            Home.storyIcon.StoryTipInit();

            //刷新dailytaskicon
            if (Home.dailyTask_icon != null)
                Home.dailyTask_icon.InitDailyTask_Icon();
            if (Home.dailyTask_hint != null)
                Home.dailyTask_hint.InitDailyTask_Hint();
        }
        else if (!_fly_scheduler.IsArrived())
        {
            var percent = _fly_scheduler.Percent();
            transform.localPosition = Utils.LerpByPrePercent(transform.localPosition, Home.startPosition, pre_percent, percent);
            transform.localScale = Utils.LerpByPrePercent(transform.localScale, targetScale, pre_percent, percent);
        }
    }
    //飞花/飞心
    private void ItemShow(string itemAtlas, string spriteName, bool textShow, int count)
    {
        var itempic = find_component<Image>("Panel/Image/item");
        var itemtext_rt = find_component<RectTransform>("Panel/Image/item/Text");
        var itemtext = find_component<Text>("Panel/Image/item/Text");
        itempic.sprite = _ui_manager.FindSprite(itemAtlas, spriteName, true);
        itemtext_rt.SetActive(textShow);
        itemtext.text = "+ " + count.ToString();
    }
    //每日任务hint奖励
    private void FlyHintRewardShow()
    {
        var currentitem = itemlist.Find(a => a.ID == Home.dailyTask_hint.currentTask.Task.RewardID);
        ItemShow($"{currentitem.Pack}", $"{currentitem.Icon}", true, Home.dailyTask_hint.currentTask.Task.RewardCount);
        StartCoroutine(WaitCheck_hintreward());
    }
    private IEnumerator WaitCheck_hintreward()
    {
        play_sound("sound_button_click");
        yield return new WaitForSeconds(2f);
        Home.DailyTaskInit();
    }
    //关卡宝箱奖励
    private void FlyItemShow()
    {
        if (GameConfigManager.ShareDataGlobalConfig._bundle_type_id == 5)
            unlockdrop();
    }
    //掉落解锁
    private void unlockdrop()
    {
        //解锁levelchest/和后面一关，仅掉落bloom
        if (GameConfigManager.LevelStorage.LevelCount < GameConfigManager.GlobalConfig.Unlock_Collection)
            BloomDrop();
        else
        {
            if (GameConfigManager.LevelStorage.LevelCount == GameConfigManager.GlobalConfig.Unlock_Collection)
                TileDrop();
            else
                Drop();
        }
    }
    //物品掉落的处理
    private void LifeDrop()
    {
        GameConfigManager.CommonStorage.Item_Life++;
        ItemShow("M3Reward", "icon_game_life", true, 1);
    }
    private void RemoveDrop()
    {
        GameConfigManager.CommonStorage.Item_Remove++;
        ItemShow("M3Reward", "icon_game_delete", true, 1);
    }
    private void RecallDrop()
    {
        GameConfigManager.CommonStorage.Item_Recall++;
        ItemShow("M3Reward", "icon_game_delete", true, 1);
    }
    private void BloomDrop()
    {
        GameConfigManager.CommonStorage.Item_Bloom = GameConfigManager.CommonStorage.Item_Bloom + 1;
        ItemShow("M3Reward", "icon_game_bloom", true, 1);
        //GetCurrentTile();
    }
    //牌掉落的处理
    private void GetCurrentTile()
    {
        //maxdroptile = tile2storage.TileUnlock.Count;
        var collectionlist = GameConfigManager.GameConfigGroup.CollectionConfigList;
        maxdroptile = collectionlist.Where(a => a.Type == 2).Max(a => a.ID);

        foreach (var item in collectionlist)
        {
            if (item.Type == 2)
            {
                if (GameConfigManager.Tile2Storage.TileUnlock[item.ID] == false)
                {
                    currentdroptile = item.ID;
                    lastdroptile = item.UnlockTile;
                    droptilemin = item.ID * 100;
                    droptilemax = item.ID * 100 + item.UnlockCount - 1;
                    droptilecount = item.UnlockCount;
                    break;
                }
            }
        }
    }
    private void TileDrop()
    {
        GetCurrentTile();
        var tile = collectionlist.Find(a => a.ID == currentdroptile);
        int num = 0;
        
        if (GameConfigManager.Tile2Storage.TileUnlock[lastdroptile] == true && GameConfigManager.Tile2Storage.TileUnlock[currentdroptile] == false)
        {
            foreach (var item in GameConfigManager.Tile2Storage.TileSingleUnlock)
            {
                if (GameConfigManager.Tile2Storage.TileSingleUnlock[droptilemax] == false)
                {
                    if (item.Key >= droptilemin) 
                    {
                        if (GameConfigManager.Tile2Storage.TileSingleUnlock[item.Key] == false)
                        {
                            num = item.Key - droptilemin;
                            GameConfigManager.Tile2Storage.TileSingleUnlock[item.Key] = true;
                            ItemShow($"{tile.TilePack}", $"e" + num.ToString("D2"), true, 1);
                            Home.levelChest.collectionunlock();
                            break;
                        }  
                    }
                }
                else
                {
                    GameConfigManager.ShareDataGlobalConfig._notice_id = 4;
                    _ui_manager.OpenWindow<NoticeUI>();
                    ItemShow($"{tile.TilePack}", $"e" + droptilecount.ToString("D2"), true, 1);
                    GameConfigManager.Tile2Storage.TileUnlock[currentdroptile] = true;
                    GameConfigManager.Tile2Storage.CurrentTileID = tile.ID;
                    Home.collection.ShowInit();
                }
            }
        }
    }
    //保底掉落
    private void guarddrop()
    {
        GetCurrentTile();
        int randomNum2 = Random.Range(1, 11);
        if (randomNum2 <= 8)
        {
            if (GameConfigManager.Tile2Storage.TileUnlock[maxdroptile] == false)
                TileDrop();
            else
                BloomDrop();
        }
        else
            BloomDrop();
    }

    //根据几率掉落
    private void Drop()
    {
        //局内使用过道具，则30%的概率返还1个该道具
        //其余则给保底奖励
        bool propUsed = false;
        int randomNum2 = Random.Range(1, 101);
        foreach (var item in GameConfigManager.Tile2Storage.LevelChestItemList)
        {
            if (item > 0)
                propUsed = true;
        }
        if (propUsed == true)
        {
            if (randomNum2 <= 30)
            {
                if (GameConfigManager.Tile2Storage.LevelChestItemList[0] > 0)
                    LifeDrop();
                if (GameConfigManager.Tile2Storage.LevelChestItemList[1] > 0)
                    RemoveDrop();
                if (GameConfigManager.Tile2Storage.LevelChestItemList[2] > 0)
                    RecallDrop();
                if (GameConfigManager.Tile2Storage.LevelChestItemList[3] > 0)
                    BloomDrop();
            }
            else
                guarddrop();
        }
        else
            guarddrop();
    }
}
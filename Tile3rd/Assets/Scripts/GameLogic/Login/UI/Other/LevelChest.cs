using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelChest : WindowUI
{
    public HomeUI Home;
    public bool _bloombuff_check;

    public LevelChest Init(HomeUI home)//PanelUI的初始化
    {
        Home = home;
        return this;
    }
    protected override void on_create()
    {
        register_button("normal", on_normal_clicked);
        register_button("ready_chest", on_chest_clicked);
        register_button("ready_bloom", on_bloom_clicked);

        ButtonInit();
    }
    public void ButtonInit()
    {
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];
        var levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        var sharestorage = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        var normal = find_component<RectTransform>("normal");
        var chest = find_component<RectTransform>("ready_chest");
        var bloom = find_component<RectTransform>("ready_bloom");

        normal.SetActive(false);
        chest.SetActive(false);
        bloom.SetActive(false);

        if (tile2storage.LevelChest_Process == 0)
        {
            if (levelStorage.LevelCount >= globalconfig.Unlock_BloomBundle)
            {
                if (tile2storage.BloomBuffTimes > 0)
                    normal.SetActive(true);
                else
                {
                    if (_bloombuff_check == false)
                        bloom.SetActive(true);
                    else
                        normal.SetActive(true);
                }
            }
            else
                normal.SetActive(true);
        }

        if (tile2storage.LevelChest_Process == 1)
        {
            if (levelStorage.LevelCount >= globalconfig.Unlock_BloomBundle)
            {
                if (tile2storage.BloomBuffTimes > 0)
                    chest.SetActive(true);
                else
                {
                    if (_bloombuff_check == false)
                        bloom.SetActive(true);
                    else
                        chest.SetActive(true);
                }
            }
            else
                chest.SetActive(true);
        }
        ////关箱状态
        //if (tile2storage.LevelChest_Process == 0)
        //{
        //    if (levelStorage.LevelCount < globalconfig.Unlock_BloomBundle)
        //        normal.SetActive(true);
        //    else
        //    {
        //        if (tile2storage.BloomBuffTimes > 0)
        //            normal.SetActive(true);
        //        else
        //            bloom.SetActive(true);
        //    }
        //}
        ////开箱状态
        //if (tile2storage.LevelChest_Process == 1)
        //{
        //    chest.SetActive(true);
        //}
    }
    private void on_normal_clicked()
    {
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        shareDataGlobalConfig._notice_id = 1;
        _ui_manager.OpenWindow<NoticeUI>();
    }
    private void on_chest_clicked()
    {
        play_sound("sound_chest_open");
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        shareDataGlobalConfig._bundle_type_id = 5;
        shareDataGlobalConfig._home_fly = 1;
        Home.home_rewarditemfly();
        tile2storage.LevelChest_Process = 0;
        ButtonInit();
    }
    private void on_bloom_clicked()
    {
        _ui_manager.OpenWindow<BundleBloomUI>();
        ButtonInit();
    }
    public void collectionunlock()
    {
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];
        var levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();

        if (levelStorage.LevelCount >= globalconfig.Unlock_Collection)
            Home.collection.ShowInit();
    }
}

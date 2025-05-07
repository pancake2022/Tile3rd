using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CollectionUI : WindowUI
{
    public class CollectionButton : BaseUI
    {
        public CollectionConfig collectionConfig;
        public Action<CollectionButton> ClickCalback;
        private Image back;
        private Image tile;
        private RectTransform tilelock;
        private RectTransform tileunlock;
        private Text desc;
        private Slider slider;
        private Image slider_con;
        private RectTransform button_use;
        private RectTransform button_select;
        private RectTransform tip;
        //private int truecount;

        protected override void on_create()
        {
            back = find_component<Image>("BG/image");
            tile = find_component<Image>("Tile/image");
            tilelock = find_component<RectTransform>("Condition/lock");
            tileunlock = find_component<RectTransform>("Condition/unlock");

            //lock
            register_button("Condition/lock/get", on_lock_clicked);
            desc = find_component<Text>("Condition/lock/desc/Text");
            slider = find_component<Slider>("Condition/lock/rate/slider");
            slider_con = find_component<Image>("Condition/lock/rate/slider/Icon");

            //unlock
            register_button("Condition/unlock/use", on_use_clicked);
            button_use = find_component<RectTransform>("Condition/unlock/use");
            button_select = find_component<RectTransform>("Condition/unlock/select");

            //tip
            tip = find_component<RectTransform>("Tip");
            tip.SetActive(false);
        }
        private void SetPanel()
        {
            back.sprite = _ui_manager.FindSprite($"{collectionConfig.BackPack}", $"{collectionConfig.Back}", true);
            tile.sprite = _ui_manager.FindSprite($"{collectionConfig.IconPack}", $"{collectionConfig.Icon}", true);
            desc.text = $"{collectionConfig.Describe}";
        }
        public void SetGray()
        {
            var mater1 = find_component<Coffee.UIExtensions.UIEffect>("BG/image");
            var mater2 = find_component<Coffee.UIExtensions.UIEffect>("Tile/image");
            mater1.enabled = true;
            mater2.enabled = true;
        }
        public void SetSliderIcon()
        {
            if (collectionConfig.Type == 1)
                slider_con.sprite = _ui_manager.FindSprite($"{collectionConfig.IconPack}", $"icon_heart", true);
            if (collectionConfig.Type == 2)
                slider_con.sprite = _ui_manager.FindSprite($"{collectionConfig.IconPack}", $"e00", true);
        }
        public void ConditionInit()
        {
            tilelock.SetActive(false);
            tileunlock.SetActive(false);

            if (GameConfigManager.Tile2Storage.TileUnlock[collectionConfig.ID])
                tileunlock.SetActive(true);
            else
            {
                tilelock.SetActive(true);
                SetGray();
            }
        }
        public void SetSlider(int current, int max)
        {
            var slidertext = find_component<Text>("Condition/lock/rate/slider/Fill Area/Text");
            
            if (collectionConfig.Type == 1)
            {
                slider.value = GameConfigManager.Tile2Storage.LoveLevelLevel;
                slider.maxValue = collectionConfig.UnlockLevel;
                slidertext.text = $"{GameConfigManager.Tile2Storage.LoveLevelLevel}/{collectionConfig.UnlockLevel}";
            }
            if (collectionConfig.Type == 2)
            {
                slider.value = current;//这个值等于true的数量
                slider.maxValue = max;
                slidertext.text = $"{slider.value}/{slider.maxValue}";
            }
        }
        public void SetTile(int tileID)
        {
            GameConfigManager.Tile2Storage.CurrentTileID = tileID;
        }
        public void SetSelected(bool markValue, bool buttonValue)
        {
            button_select.SetActive(markValue);
            button_use.SetActive(buttonValue);
        }
        private void on_lock_clicked()
        {
            play_sound("sound_button_click");
            if (collectionConfig.Type == 2)
            {
                ShowTip();
            }
            if (collectionConfig.ID == 101)
            {
                //sign解锁
                if (GameConfigManager.LevelStorage.LevelCount >= GameConfigManager.GlobalConfig.Unlock_Sign)
                {
                    _ui_manager.OpenWindow<SignUI>();
                    _ui_manager.TryCloseWindow<CollectionUI>();
                }
                else
                    ShowTip();
            }
        }
        async void ShowTip()
        {
            tip.SetActive(true);
            await Task.Delay(TimeSpan.FromSeconds(1));
            tip.SetActive(false);
        }
        private void on_use_clicked()
        {
            ClickCalback?.Invoke(this);
        }
        public CollectionButton Init(CollectionConfig collection, Action<CollectionButton> click_callback)
        {
            collectionConfig = collection;
            ClickCalback = click_callback;
            Show();
            SetPanel();
            return this;
        }
    }

    public static new string DefaultPrefabPath = "Panel/UI_Panel_collection";
    public CollectionButton collection_button;
    public List<CollectionButton> collectionButtonList;
    public CollectionConfig CurrentPanel;
    private RectTransform collectionButton_rt;
    private GameObject collectionButton_temp;
    private bool alltileunlock;
    private int counttrue;

    protected override void on_create()
    {
        Property.CommonAnimationTransform = transform.Find("Panel");
        _ui_manager.Framework.AudioManager.PlaySound("sound_panel_opening");
        register_button("Panel/UI_Top/Button_close", on_close_clicked);

        collectionButton_rt = find_component<RectTransform>("Panel/Scroll View/Viewport/Content/UI_Middle");
        collectionButton_temp = find_component<RectTransform>("CollectionTemplate", collectionButton_rt).gameObject;
        collectionButton_temp.SetActive(false);
        collectionButtonList = new List<CollectionButton>();
        CollectionInit();
        BGSet();
    }
    private void BGSet()
    {
        var storylist = GameConfigManager.GameConfigGroup.StoryConfigList;
        var back = find_component<RectTransform>("BG");
        var currentstory = storylist.Find(a => a.ID == GameConfigManager.MakeOverStorage.CurrentStoryID);
        var background = create_ui<HomeBackground>($"MakeOverLevels/00_bg/{currentstory.HomeBack}", back);
        background.Init();
    }
    public CollectionUI CollectionInit()
    {
        RefreshCollectionButtonList();
        return this;
    }
    public void ClearCollectionButtonList()
    {
        foreach (var collection_button in collectionButtonList)
            destroy_ui(collection_button);
        collectionButtonList.Clear();
    }
    public void RefreshCollectionButtonList()
    {
        var collectionlist = GameConfigManager.GameConfigGroup.CollectionConfigList;
        ClearCollectionButtonList();
        foreach (var collection in collectionlist)
        {
            collection_button = create_ui<CollectionButton>(collectionButton_temp, collectionButton_rt);
            collection_button.Init(collection, p => on_panel_selected(p.collectionConfig));
            collectionButtonList.Add(collection_button);
            collection_button.ConditionInit();
            collection_button.SetSliderIcon();

            if (collection.ID == GameConfigManager.Tile2Storage.CurrentTileID)
                collection_button.SetSelected(true, false);
            else
                collection_button.SetSelected(false, true);

            int testmin = collection.ID * 100;
            int testmax = collection.ID * 100 + collection.UnlockCount;
            int counttrue = 0;
            foreach (var item in GameConfigManager.Tile2Storage.TileSingleUnlock) 
            {
                if (item.Key > testmin && item.Key <= testmax && item.Value == true) 
                    counttrue++;
            }
            collection_button.SetSlider(counttrue, collection.UnlockCount);
        }
    }

    public void on_panel_selected(CollectionConfig collectionConfig)
    {
        play_sound("sound_button_click");
        if (CurrentPanel != collectionConfig)
        {
            var current_collection_button = collectionButtonList.Find(a => a.collectionConfig == CurrentPanel);
            if (current_collection_button)
                current_collection_button.SetSelected(false, true);

            CurrentPanel = collectionConfig;
            current_collection_button = collectionButtonList.Find(a => a.collectionConfig == CurrentPanel);
            
            if (current_collection_button)
            {
                current_collection_button.SetSelected(true, false);
                current_collection_button.SetTile(CurrentPanel.ID);
            }
            RefreshCollectionButtonList();
        }
    }
    private void on_close_clicked()
    {
        play_sound("sound_panel_closing");
        Close();
    }
}
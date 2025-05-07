using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CatQuest : BaseUI
{
    public class QuestButton : BaseUI
    {
        public QuestConfig questConfig;
        public Action<QuestButton> ClickCalback;
        private RectTransform cat;
        private RectTransform bubble;
        private Image icon;
        private Text buttontext;

        protected override void on_create()
        {
            cat = find_component<RectTransform>("Cat");
            bubble = find_component<RectTransform>("Bubble");
            icon = find_component<Image>("Bubble/image/image");
            register_button("Bubble/Start/button", on_clicked);
            buttontext = find_component<Text>("Bubble/Start/button/Text");
        }
        public void on_clicked()
        {
            ClickCalback?.Invoke(this);
        }
        public void SetCatShow(bool value)
        {
            cat.SetActive(value);
        }
        public void SetButtonText()
        {
            buttontext.text = questConfig.ButtonDesc;
        }
        public void SetBubbleShow()
        {
            if (GameConfigManager.MakeOverStorage.CatQuestCondition[questConfig.ID] <= 1)
                bubble.SetActive(true);
            else
                bubble.SetActive(false);
        }
        public void SetBubblePosition(int value)
        {
            var bubbleposition = bubble.localPosition;
            if (value == 1) 
            {
                bubbleposition.x = 50;
                bubbleposition.y = 195;
                bubble.localPosition = bubbleposition;
            }
            else
            {
                bubbleposition.x = 130;
                bubbleposition.y = 150;
                bubble.localPosition = bubbleposition;
            }
        }
        public void SetImage(string imagePack, string imageIcon)
        {
            icon.sprite = _ui_manager.FindSprite($"{imagePack}", $"{imageIcon}", true);
        }
        public QuestButton Init(QuestConfig quest, Action<QuestButton> click_callback)
        {
            questConfig = quest;
            ClickCalback = click_callback;
            SetBubbleShow();
            SetButtonText();
            return this;
        }
    }

    public HomeUI Home;
    public QuestButton questButton;
    public int startButtonType;

    private List<QuestConfig> questlist;
    private QuestConfig currentQuest;
    private M3Panel questLevelPanel;

    public CatQuest Init(HomeUI home)
    {
        Home = home;
        return this;
    }
    protected override void on_create()
    {
        questlist = GameConfigManager.GameConfigGroup.QuestConfigList;
    }
    public CatQuest InitCatQuest()
    {
        RefreshCatQuest();
        return this;
    }
    public void RefreshCatQuest()
    {
        GetCurrentQuest();
        CatQuestActive(false);
        if (currentQuest != null)
        {
            questButton = create_ui<QuestButton>("Panel");
            questButton.Init(currentQuest, p => on_start_clicked(p.questConfig));
            GuideInit();
            GetIcon();
            GetQuestType();
        }
        else
            gameObject.SetActive(false);
    }
    private void GetCurrentQuest()
    {
        currentQuest = questlist.Find(a => a.StoryID == GameConfigManager.MakeOverStorage.CurrentStoryID
        && GameConfigManager.MakeOverStorage.CatQuestCondition[a.ID] <= 2);
        if (currentQuest != null)
            GameConfigManager.MakeOverStorage.CurrentQuest.ID = currentQuest.ID;
    }
    private void GetQuestType()
    {
        if (currentQuest.QuestType == 1)
            Type1();
        if (currentQuest.QuestType == 2)
            Type2();
        if (currentQuest.QuestType == 3)
            Type3();
        if (currentQuest.QuestType == 4)
            Type4();
    }
    private void Type1()
    {
        startButtonType = 1;
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 0)
        {
            gameObject.SetActive(false);
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[currentQuest.UnlockCondition] > 1)
                GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] = 1;
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 1)
        {
            gameObject.SetActive(true);
            questButton.SetCatShow(true);
            questButton.SetBubblePosition(1);
            CatQuestActive(true);
            Home.makeOver.makeOver_CatImage.catButton.CatShow(false);
            Guide();
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 2)
        {
            gameObject.SetActive(true);
            questButton.SetCatShow(false);
            _ui_manager.OpenWindow<CatQuestRewardUI>();
            CatQuestActive(true);
            Home.makeOver.makeOver_CatImage.catButton.CatShow(false);
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 3)
            gameObject.SetActive(false);
    }

    private void Type2()
    {
        startButtonType = 1;
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 0)
        {
            gameObject.SetActive(false);
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[currentQuest.UnlockCondition] > 1)
                GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] = 1;
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 1)
        {
            gameObject.SetActive(true);
            questButton.SetCatShow(false);
            questButton.SetBubblePosition(2);
            CatQuestActive(true);
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 2)
        {
            gameObject.SetActive(true);
            questButton.SetCatShow(false);
            questButton.SetBubblePosition(2);
            on_panel_selected(questButton.questConfig);
            CatQuestActive(true);
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 3)
            gameObject.SetActive(false);
    }

    private void Type3()
    {
        startButtonType = 2;
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 0)
        {
            gameObject.SetActive(false);
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[currentQuest.UnlockCondition] > 1)
                GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] = 1;
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 1)
        {
            gameObject.SetActive(true);
            questButton.SetCatShow(false);
            CatQuestActive(true);
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 2)
        {
            gameObject.SetActive(true);
            questButton.SetCatShow(false);
            on_panel_selected(questButton.questConfig);
            CatQuestActive(true);
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 3)
        {
            gameObject.SetActive(false);
            questButton.SetCatShow(false);
        }
    }

    private void Type4()
    {
        startButtonType = 3;
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 0)
        {
            gameObject.SetActive(false);
            if (GameConfigManager.MakeOverStorage.TouchPointCondition[currentQuest.UnlockCondition] > 1)
                GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] = 1;
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 1)
        {
            gameObject.SetActive(true);
            questButton.SetCatShow(false);
            CatQuestActive(true);
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 2)
        {
            gameObject.SetActive(true);
            questButton.SetCatShow(false);
            on_panel_selected(questButton.questConfig);
            CatQuestActive(true);
        }
        if (GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] == 3)
        {
            gameObject.SetActive(false);
            questButton.SetCatShow(false);
        }
    }

    private void on_start_clicked(QuestConfig data)
    {
        currentQuest = data;
        if (startButtonType == 1)
        {
            GameConfigManager.ShareDataGlobalConfig._level_condition = 2;

            GetQuesLevelPanel();
            _ui_manager.OpenWindow<GameUI>().Init(questLevelPanel);
            _ui_manager.TryCloseWindow<HomeUI>();
            play_sound("sound_button_click");
        }
        if (startButtonType == 2)
        {
            Home.HomePanelShow(false);
            Home.makeOver.CloseButtonShow(true);
            Home.makeOver.makeOver_Tips.SetTitle();
            Home.makeOver.makeOver_Tips.TipsInit(true);

            //控制猫的的点击-只有开startclick的时候点猫才弹界面
            Home.makeOver.makeOver_CatImage.isCatTouch = true;
            Home.makeOver.makeOver_Tips.ButtonInit(true);
            foreach (var item in Home.makeOver.CurrentStoryImageList)
            {
                if (item.TouchType != 3)
                    item.TouchType = 0;
            }
        }
        if (startButtonType == 3)
        {
            Home.HomePanelShow(false);
            Home.makeOver.CloseButtonShow(true);
            Home.makeOver.makeOver_Tips.SetTitle();
            Home.makeOver.makeOver_Tips.SetTips();
            Home.makeOver.makeOver_Tips.TipsInit(true);
            Home.makeOver.makeOver_Image.ImageChangeCondition();
            Home.makeOver.makeOver_CatImage.ClearCatButton();

            //控制图的点击-只有开startclick的时候点图才有效
            Home.makeOver.makeOver_Image.isQuestClick = true;
        }
    }
    public void on_panel_selected(QuestConfig questConfig)
    {
        GameConfigManager.MakeOverStorage.CatQuestCondition[currentQuest.ID] = 3;
        GameConfigManager.MakeOverStorage.CurrentStoryID = currentQuest.StoryID;
        Home.MakeOverInit();
        CurrentImageUnlock();
        RefreshCatQuest();
    }

    public void CurrentImageUnlock()
    {
        //打开selectUI
        Home.makeOver.MakeOverUI_OnSelect();
        Home.makeOver.makeOver_Select.LoveBarInit();

        var quest = questlist.Find(a => a.ID == GameConfigManager.MakeOverStorage.CurrentQuest.ID);
        var touch = Home.makeOver.CurrentStoryTouchList.Find(a => a.ImageIDList.Contains(quest.MakeOverImageID));

        if (touch != null)
        {
            foreach (var item in touch.ImageIDList)
            {
                var data = Home.makeOver.CurrentStoryImageList.Find(a => a.ID == item);
                if (data.ID == quest.MakeOverImageID)
                {
                    GameConfigManager.MakeOverStorage.ImageUse[data.ID] = true;
                    GameConfigManager.MakeOverStorage.ImageUnlock[data.ID] = true;
                    Home.makeOver.makeOver_Select.SetTouchUnlock(data);
                    GameConfigManager.Tile2Storage.LoveLevelExpUp = data.LoveExp;
                    Home.makeOver.makeOver_Select.GetCatID();
                    Home.makeOver.makeOver_Image.InitImageButtonList();
                    Home.makeOver.makeOver_Select.AnimType = 2;
                    Home.makeOver.makeOver_Select.SelectAnim();
                }
                else
                    GameConfigManager.MakeOverStorage.ImageUse[data.ID] = false;
            }
        }
    }

    public void GetIcon()
    {
        var all_image_config = GameConfigManager.GameConfigGroup.MakeOverConfigList;
        var current_image = all_image_config.Find(a => a.ID == currentQuest.MakeOverImageID);
        questButton.SetImage($"{current_image.Pack}", $"{current_image.Icon}");
    }
    //找到指定关卡
    private void GetQuesLevelPanel()
    {
        var levellist = GameConfigManager.GameConfigGroup.LevelConfigList;
        var Level = levellist.Find(a => a.ID == questButton.questConfig.LevelID);
        var panel_config_ta = _ui_manager.Framework.ResourcesManager.LoadResource<TextAsset>($"{M3Const.M3PanelConfigPath}/{Level.PanelID}");
        if (panel_config_ta != null)
        {
            try
            {
                questLevelPanel = JsonUtility.FromJson<M3Panel>(panel_config_ta.text);
                GameConfigManager.LevelStorage.CurrentPanel = questLevelPanel;
            }
            catch (Exception e)
            {
                CSFramework.Logger.Error(e);
            }
        }
    }
    private void GuideInit()
    {
        var guide = find_component<RectTransform>("Panel/guide");
        guide.SetActive(false);
    }
    private void Guide()
    {
        if (GameConfigManager.MakeOverStorage.TouchPointCondition[4] == 2
            && GameConfigManager.MakeOverStorage.TouchPointCondition[5] == 1)
            SetGuide();
    }
    async void SetGuide()
    {
        var guide = find_component<RectTransform>("Panel/guide");
        guide.SetActive(false);
        await Task.Delay(TimeSpan.FromSeconds(2));
        guide.SetActive(true);
    }
    public void CatQuestActive(bool value)
    {
        GameConfigManager.ShareDataGlobalConfig._is_catquest_active = value;
        if (Home.dailyTask_hint != null)
            Home.dailyTask_hint.InitDailyTask_Hint();
        if (Home.dailyTask_icon != null)
            Home.dailyTask_icon.InitDailyTask_Icon();
        Home.makeOver.makeOver_Touch.InitTouchButtonList();
    }
}

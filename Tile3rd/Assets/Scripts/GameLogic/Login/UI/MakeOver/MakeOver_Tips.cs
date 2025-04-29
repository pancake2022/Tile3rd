using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MakeOver_Tips : BaseUI
{
    public MakeOver makeOver;
    private RectTransform button;

    public MakeOver_Tips Init(MakeOver makeover)
    {
        makeOver = makeover;
        return this;
    }
    protected override void on_create()
    {
        TipsInit(false);
        ButtonInit(false);

        register_button("button", on_clicked);
    }
    public void TipsInit(bool value)
    {
        var tips1 = find_component<RectTransform>("text1");
        var tips2 = find_component<RectTransform>("text2");
        tips1.SetActive(value);
        tips2.SetActive(value);
    }
    public void ButtonInit(bool value)
    {
        var makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        button = find_component<RectTransform>("button");
        button.SetActive(value);

        var buttontext = find_component<Text>("button/Text");
        if (makeoverStorage.CurrentStoryID == 3)
            buttontext.text = "Hint";
        if (makeoverStorage.CurrentStoryID == 5)
            buttontext.text = "Finish";
        if (makeoverStorage.CurrentStoryID == 101)
            buttontext.text = "Finish";
    }
    public void SetTitle()
    {
        var makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        var tips = find_component<RectTransform>("text1");
        tips.SetActive(true);
        var text = find_component<Text>("text1");
        if (makeoverStorage.CurrentStoryID == 3)
            text.text = "Where is the cat?";
        if (makeoverStorage.CurrentStoryID == 5)
            text.text = "Dry the cat";
        if (makeoverStorage.CurrentStoryID == 101)
            text.text = "Wake the cat";
    }
    public void SetTips()
    {
        var makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        var text = find_component<Text>("text2");
        if (makeoverStorage.CurrentStoryID == 3)
        {
            if (makeOver.makeOver_Image.CurrentImage.ID == 321)
                text.text = "Not even a shadow";
            if (makeOver.makeOver_Image.CurrentImage.ID == 322)
                text.text = "Only pillows here";
            if (makeOver.makeOver_Image.CurrentImage.ID == 318)
                text.text = "The cage is empty";
        }
        if (makeoverStorage.CurrentStoryID == 5)
            text.text = "Tap the cat and dry its fur";
        if (makeoverStorage.CurrentStoryID == 101)
            text.text = "Tap the cat and wake it";
    }
    private void on_clicked()
    {
        var makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        if (makeoverStorage.CurrentStoryID == 3)
        {
            ADSManager.TriggerADSShow_Reward("FindCat_Story");
        }
        if (makeoverStorage.CurrentStoryID == 5)
        {
            _ui_manager.OpenWindow<CatQuestRewardUI>();
        }
        if (makeoverStorage.CurrentStoryID == 101)
        {
            _ui_manager.OpenWindow<CatQuestRewardUI>();
        }
    }
}
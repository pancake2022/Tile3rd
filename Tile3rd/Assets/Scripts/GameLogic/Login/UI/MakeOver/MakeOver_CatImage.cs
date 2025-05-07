using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MakeOver_CatImage : BaseUI
{
    public class CatButton : BaseUI
    {
        public MakeOverConfig CatData;
        public Action<CatButton> ClickCalback;

        protected override void on_create()
        {
            register_button("Panel/image", on_clicked);
        }
        public void on_clicked()
        {
            ClickCalback?.Invoke(this);
        }
        public void CatShow(bool value)
        {
            gameObject.SetActive(false);
            var cat = find_component<RectTransform>("Panel/image");
            cat.SetActive(value);
            gameObject.SetActive(true);
        }
        public void CatAnim()
        {
            CatShow(false);
            var catanim = find_component<Animator>("Panel");
            catanim.enabled = true;
            catanim.Play($"{CatData.CatAnim}");
        }
        public void CatDefaultAnim()
        {
            var catanim = find_component<Animator>("Panel");
            catanim.SetBool($"{CatData.CatDefaultAnim}", true);
        }
        public void CatQuestAnim()
        {
            var catanim = find_component<Animator>("Panel");
            catanim.SetBool($"blink", true);
        }
        public CatButton Init(MakeOverConfig catData, Action<CatButton> click_callback)
        {
            CatData = catData;
            ClickCalback = click_callback;
            return this;
        }
    }

    public CatButton catButton;
    public MakeOver makeOver;
    private List<MakeOverConfig> datalist;
    public MakeOverConfig CurrentCat;
    public Vector3 localPosition;
    public bool isCatTouch;

    public MakeOver_CatImage Init(MakeOver makeover)
    {
        makeOver = makeover;
        datalist = makeOver.CurrentStoryImageList;
        return this;
    }
    public MakeOver_CatImage InitCatButton()
    {
        RefreshCatButton();
        return this;
    }
    //挂猫
    private void RefreshCatButton()
    {
        ClearCatButton();
        var data = datalist.Find(a => a.CatPrefab != "null" && a.CatID == GameConfigManager.MakeOverStorage.CurrentCatID[a.StoryID]);
        if (data != null)
        {
            catButton = create_ui<CatButton>($"MakeOverLevels/{data.StoryID.ToString("D2")}/{data.CatPrefab}", $"image/CatPoint_{data.CatID}");
            catButton.CatShow(true);
            catButton.Init(data, p => on_cat_clicked(p.CatData));
        }
    }
    public void ClearCatButton()
    {
        if (catButton != null)
            destroy_ui(catButton);
    }
    public void CatHeartPosition()
    {
        var data = datalist.Find(a => a.CatPrefab != "null" && a.CatID == GameConfigManager.MakeOverStorage.CurrentCatID[a.StoryID]);
        var heart = find_component<RectTransform>($"image/CatPoint_{data.CatID}");
        var heartPos = heart.localPosition;
        heartPos.y += 120;
        localPosition = heartPos;
    }
    public void on_cat_clicked(MakeOverConfig data)
    {
        CurrentCat = data;
        if (isCatTouch == true)
        {
            if (GameConfigManager.MakeOverStorage.CurrentCatID[3] == 2)
                _ui_manager.OpenWindow<CatQuestRewardUI>();
        }
    }
    //story03找猫(广告用)
    public void Story03Cat()
    {
        catButton.CatQuestAnim();
    }
}

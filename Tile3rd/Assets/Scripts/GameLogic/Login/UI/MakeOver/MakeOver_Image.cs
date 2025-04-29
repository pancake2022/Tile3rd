using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MakeOver_Image : BaseUI
{
    public class ImageButton : BaseUI
    {
        public MakeOverConfig Data;
        public Action<ImageButton> ClickCalback;
        private MakeOverStorage makeoverStorage;

        protected override void on_create()
        {
            makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        }
        //登录游戏时默认显示
        private void ImageShow()
        {
            ShowIDList();
            HideIDList();
            gameObject.SetActive(false);
            if (makeoverStorage.ImageUse[Data.ID])
                gameObject.SetActive(true);
        }
        //showlist里的data只要有1个解锁了就显示指定图片
        private void ShowIDList()
        {
            Data.ShowIDList.ForEach(item =>
            {
                if (item > 0)
                {
                    if (makeoverStorage.ImageUnlock[item])
                    {
                        makeoverStorage.ImageUnlock[Data.ID] = true;
                        makeoverStorage.ImageUse[Data.ID] = true;
                    }
                }
            });
        }
        //hidelist里的data只要有1个解锁了就隐藏指定图片
        private void HideIDList()
        {
            Data.HideIDList.ForEach(item =>
            {
                if (item > 0)
                {
                    if (makeoverStorage.ImageUnlock[item])
                        makeoverStorage.ImageUse[Data.ID] = false;
                }
            });
        }
        private void SetColor()
        {
            var color = find_component<Image>("Panel/image");
            if (makeoverStorage.ImageUnlock[Data.ID])
                color.color = new Color32(255, 255, 255, 255);
            else
                color.color = new Color32(100, 100, 100, 180);
        }
        public void ChangeImageColor()
        {
            var color = find_component<Image>("Panel/image");
            color.color = new Color32(255, 255, 255, 255);
        }
        private void ButtonShow()
        {
            if (Data.TouchType != 0)
                register_button("Panel/image", on_clicked);
        }
        public void on_clicked()
        {
            ClickCalback?.Invoke(this);
        }
        //被选中时显示
        public void SetSelectShow(bool value)
        {
            gameObject.SetActive(value);
        }
        public void DefaultAnim()
        {
            var anim = find_component<Animator>("Panel");
            if (Data.MakeOverDefaultAnim != "null")
            {
                anim.enabled = true;
                anim.SetBool($"{Data.MakeOverDefaultAnim}", true);
            }
        }
        public void MakeoverAnim()
        {
            var anim = find_component<Animator>("Panel");
            anim.enabled = true;
            anim.Play($"{Data.MakeOverAnim}");
        }
        public ImageButton Init(MakeOverConfig data, Action<ImageButton> click_callback)
        {
            Data = data;
            ClickCalback = click_callback;
            ImageShow();
            SetColor();
            ButtonShow();
            return this;
        }
    }

    public MakeOver makeOver;
    public ImageButton imageButton;
    public List<ImageButton> imageButtonList;
    public MakeOverConfig CurrentImage;
    private RectTransform imagepoint;
    public bool fromImageClick;
    public bool isQuestClick;
    private MakeOverStorage makeoverStorage;
    private List<MakeOverConfig> datalist;
    private RectTransform imageLerp;
    private Vector3 defaultPosition;
    private Vector3 defaultScale;
    public int lerpCondition;//0.无状态/1.on/2.off

    public MakeOver_Image Init(MakeOver makeover)
    {
        makeOver = makeover;
        datalist = makeOver.CurrentStoryImageList;
        return this;
    }
    protected override void on_create()
    {
        makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
        imageButtonList = new List<ImageButton>();
        CurrentImage = null;

        imageLerp = find_component<RectTransform>($"image");
        defaultPosition = new Vector3(0, 0, 0);
        defaultScale = new Vector3(1, 1, 1);
    }
    private void Update()//飞行轨迹
    {
        if (lerpCondition == 1)
            ImageLerpOn();
        if (lerpCondition == 2)
            ImageLerpOff();
    }
    public MakeOver_Image InitImageButtonList()
    {
        RefreshImageButtonList();
        collider();
        return this;
    }
    //初始化image
    public void RefreshImageButtonList()
    {
        ClearImageButtonList();
        foreach (var data in datalist)
        {
            //创建buttonlist
            imageButton = create_ui<ImageButton>($"MakeOverLevels/{data.StoryID.ToString("D2")}/{data.ImagePrefab}", $"{data.ImagePath}/Panel");
            imageButton.Init(data, p => on_image_selected(p.Data));
            imageButtonList.Add(imageButton);
        }
    }
    public void ClearImageButtonList()
    {
        foreach (var image_button in imageButtonList)
            destroy_ui(image_button);
        imageButtonList.Clear();
    }
    private void ImageLerpOn()
    {
        imageButton = imageButtonList.Find(a => a.Data == makeOver.makeOver_Select.CurrentPanel);

        var finishPosition = new Vector3(imageButton.Data.EditPoint[0], imageButton.Data.EditPoint[1], 0f);
        var finishScale = new Vector3(imageButton.Data.EditScale, imageButton.Data.EditScale, 1f);

        if (imageLerp.localPosition == finishPosition)
            lerpCondition = 0;
        else
        {
            imageLerp.localPosition = Vector3.Lerp(imageLerp.localPosition, finishPosition, 3f * Time.deltaTime);
            imageLerp.localScale = Vector3.Lerp(imageLerp.localScale, finishScale, 3f * Time.deltaTime);
        }
    }
    public void ImageLerpOff()
    {
        if (imageLerp.localPosition == defaultPosition)
            lerpCondition = 0;
        else
        {
            imageLerp.localPosition = Vector3.Lerp(imageLerp.localPosition, defaultPosition, 3f * Time.deltaTime);
            imageLerp.localScale = Vector3.Lerp(imageLerp.localScale, defaultScale, 3f * Time.deltaTime);
        }
    }
    //select界面的image状态
    public void Select_ImageShow(bool value)
    {
        imageButton = imageButtonList.Find(a => a.Data == makeOver.makeOver_Select.CurrentPanel);
        imageButton.SetSelectShow(value);
        foreach (var imageButton in imageButtonList)
        {
            if (imageButton.Data.ShowIDList.Contains(makeOver.makeOver_Select.CurrentPanel.ID))
                imageButton.SetSelectShow(true);
        }
        imageButton = imageButtonList.Find(a => a.Data.ID == makeOver.makeOver_Select.CurrentPanel.SelectHideID);
        if (imageButton != null)
            imageButton.SetSelectShow(false);
    }
    //点图片的状态
    public void on_image_selected(MakeOverConfig data)
    {
        CurrentImage = data;
        fromImageClick = true;
        if (CurrentImage.TouchType == 1)//普通
        {
            var item = makeOver.CurrentStoryTouchList.Find(a => a.ImageIDList.Contains(CurrentImage.ID));
            makeOver.makeOver_Touch.on_touch_selected(item);
            makeOver.MakeOverUI_OnSelect();
            lerpCondition = 1;
        }
        if (CurrentImage.TouchType == 2)//看信
            _ui_manager.OpenWindow<MakeOver_Notice>();
        if (CurrentImage.TouchType == 3)//找猫
        {
            if (makeOver.makeOver_CatImage.isCatTouch)
                makeOver.makeOver_Tips.SetTips();
        }
        if (CurrentImage.TouchType == 4)//换图
        {
            if (makeOver.makeOver_Image.isQuestClick)
            {
                int max = 0;
                int current = 0;
                imageButton = imageButtonList.Find(a => a.Data == CurrentImage);
                imageButton.SetSelectShow(false);
                imageButton = imageButtonList.Find(a => a.Data.ID == CurrentImage.TouchShowID);
                imageButton.SetSelectShow(true);
                imageButton.ChangeImageColor();
                ImageChangeCondition();
            }
        }
    }
    public void ImageChangeCondition()
    {
        int max = 0;
        int current = 0;
        foreach (var imageButton in imageButtonList)
        {
            if (imageButton.Data.TouchShowID != 0)
            {
                max++;
                if (imageButton.gameObject.active == false)
                    current++;
            }
        }
        if (current == max)
            makeOver.makeOver_Tips.ButtonInit(true);
    }

    public void SetMakeoverAnim(MakeOverConfig data)
    {
        CurrentImage = data;
        var image_main = imageButtonList.Find(a => a.Data == CurrentImage);
        image_main.MakeoverAnim();
        foreach (var imagecopy in imageButtonList)
        {
            if (imagecopy.Data.ShowIDList.Contains(makeOver.makeOver_Select.CurrentPanel.ID))
            {
                if (imagecopy != null)
                    imagecopy.MakeoverAnim();
            }
        }
    }
    //story01的collider
    private void collider()
    {
        if (makeoverStorage.CurrentStoryID == 1)
        {
            var collider_01_1 = find_component<RectTransform>($"image/background/effect/Collider/point_1");
            var collider_01_2 = find_component<RectTransform>($"image/background/effect/Collider/point_2");
            var collider_01_3 = find_component<RectTransform>($"image/background/effect/Collider/point_3");
            var collider_01_4 = find_component<RectTransform>($"image/background/effect/Collider/point_4");
            var collider_01_5 = find_component<RectTransform>($"image/background/effect/Collider/point_5");
            var collider_01_6 = find_component<RectTransform>($"image/background/effect/Collider/point_6");
            collider_01_1.SetActive(false);
            collider_01_2.SetActive(false);
            collider_01_3.SetActive(false);
            collider_01_4.SetActive(false);
            collider_01_5.SetActive(false);
            collider_01_6.SetActive(false);

            if (makeoverStorage.ImageUse[2]) 
                collider_01_1.SetActive(true);
            if (makeoverStorage.ImageUse[3]) 
                collider_01_1.SetActive(true);
            if (makeoverStorage.ImageUse[4]) 
                collider_01_3.SetActive(true);
            if (makeoverStorage.ImageUse[5]) 
                collider_01_2.SetActive(true);
            if (makeoverStorage.ImageUse[6]) 
                collider_01_1.SetActive(true);
            if (makeoverStorage.ImageUse[8]) 
                collider_01_4.SetActive(true);
            if (makeoverStorage.ImageUse[9]) 
                collider_01_5.SetActive(true);
            if (makeoverStorage.ImageUse[11])
                collider_01_6.SetActive(true);
        }
    }
}

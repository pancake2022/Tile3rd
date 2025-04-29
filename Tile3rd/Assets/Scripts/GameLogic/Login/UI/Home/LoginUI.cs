using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginUI : WindowUI
{
    public static new string DefaultPrefabPath = "Home/Login";
    public VersionUpdate versionUpdate;

    protected override void on_create()
    {
        Property.UseCommonAnimation = false;
        Property.PlayOpenCloseSound = false;

        Version();
        BackGround();

        //初始化进度条
        var slider = find_component<Slider>("Panel/Slider");
        slider.value = 0;
    }

    private void Update()
    {
        var slider = find_component<Slider>("Panel/Slider");
        if (slider.value <= 100) 
            slider.value += 1;
        if (slider.value == 100) 
        {
            SetGameManager();
            _ui_manager.OpenWindow<HomeUI>();
            Close();
        }
    }
    private void BackGround()
    {
        var bg_image = find_component<Image>("BG");
        bg_image.sprite = _ui_manager.FindSprite("M3BackGame", "bg_101", true);
    }
    private void Version()
    {
        versionUpdate = create_ui<VersionUpdate>("Panel").Init(this);
    }
    private void SetGameManager()
    {
        Transform gameManagerRoot = _ui_manager._context.WindowUILayerDict[UILayer.GameManager];
        var ads = create_ui<ADSManager>(gameManagerRoot);
        var review = create_ui<GoogleReviewManager>(gameManagerRoot);
    }

}
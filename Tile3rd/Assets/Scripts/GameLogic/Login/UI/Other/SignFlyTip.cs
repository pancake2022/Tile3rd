using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SignFlyTip : BaseUI
{
    public SignConfig Data;
    private void IconInit()
    {
        var icon = find_component<Image>("Image");
        var Desc = find_component<RectTransform>("Image/Desc");
        var desc = find_component<Text>("Image/Desc");
        var Num = find_component<RectTransform>("Image/Num");
        var num = find_component<Text>("Image/Num");
        Desc.SetActive(false);
        Num.SetActive(false);

        icon.sprite = _ui_manager.FindSprite($"{Data.IconPack}", $"{Data.Icon}", true);
        if (Data.ID == 1 || Data.ID == 4)
        {
            Desc.SetActive(true);
            desc.text = $"Bloom\nAll";
        }
        if (Data.ID == 2)
        {
            Num.SetActive(true);
            num.text = $"x{Data.Reward_Num}";
        }
        if (Data.ID == 5)
        {
            Num.SetActive(true);
            num.text = $"x{GameConfigManager.Tile2Storage.SignCount * 10}";
        }
    }
    private void SetPosition()
    {
        var home_ui = _ui_manager.FindWindow<HomeUI>();
        var sign_ui = _ui_manager.FindWindow<SignUI>();
        var v = transform.localPosition;

        if (sign_ui != null)
        {
            v.x = sign_ui.flytip_positionX;
            v.y = sign_ui.flytip_positionY;
        }
        else
            v.x = home_ui.signIcon.flytip_positionX;
        transform.localPosition = v;
    }
    public SignFlyTip Init(SignConfig data)
    {
        Data = data;
        SetPosition();
        IconInit();
        //DelayRemove();
        return this;
    }
    //async void DelayRemove()
    //{
    //    await Task.Delay(TimeSpan.FromSeconds(1.5));
    //    destroy_ui(this);
    //}
}
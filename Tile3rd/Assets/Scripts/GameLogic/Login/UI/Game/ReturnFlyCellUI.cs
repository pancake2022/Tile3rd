using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnFlyCellUI : M3GameCellUI
{
    private SchedulerFloat _fly_scheduler = new SchedulerFloat().Init(0.3f);//飞行速度是0.5
    private Action _completed_callback;//行为完成回调
    private Action _completed_callback_2;//行为完成回调
    public M3GamePanelUI M3Panel;

    public ReturnFlyCellUI Init(M3GamePanelUI m3panel)
    {
        M3Panel = m3panel;
        return this;
    }

    public ReturnFlyCellUI Init(M3Cell cell, Action completed_callback, Action completed_callback_2)//初始化
    {
        var game_ui = _ui_manager.FindWindow<GameUI>();
        game_ui.GamePause();
        _ui_manager.OpenWindow<MaskUI>();
        _completed_callback = completed_callback;//完成回调
        _completed_callback_2 = completed_callback_2;
        RefreshCell(cell);//刷新
        return this;
    }

    public ReturnFlyCellUI StartFly()//开始飞行
    {
        transform.localScale = M3GameCollectionUI.return_start.transform.localScale;//点击的缩放
        transform.position = M3GameCollectionUI.return_start.transform.position;//点击的位置
        return this;
    }

    private void Update()//游戏更新
    {
        var pre_percent = _fly_scheduler.Percent();//前百分比为飞行百分比
        if (_fly_scheduler.Tick(Time.deltaTime, false) ) //飞行结束后
        {
            var game_ui = _ui_manager.FindWindow<GameUI>();
            game_ui.GameActiveDelay();

            transform.position = M3GamePanelUI.return_end.transform.position;
            transform.localScale = M3GamePanelUI.return_end.transform.localScale;//目标缩放
            M3GamePanelUI.return_end.SetState(M3CellState.TopInLayer);
            _completed_callback?.Invoke();//不调用？
            _completed_callback_2?.Invoke();
            destroy_ui(this);
            _ui_manager.TryCloseWindow<MaskUI>();
        }
        else if (!_fly_scheduler.IsArrived())//如果是在飞行中
        {
            var percent = _fly_scheduler.Percent();//百分比 = 飞行百分比
            transform.position = Utils.LerpByPrePercent(transform.position, M3GamePanelUI.return_end.transform.position, pre_percent, percent);//调用utils里的飞行
            transform.localScale = Utils.LerpByPrePercent(transform.localScale, M3GamePanelUI.return_end.transform.localScale, pre_percent, percent);//调用utils里的缩放
        }
    }
}
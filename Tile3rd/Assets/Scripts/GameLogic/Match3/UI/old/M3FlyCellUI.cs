//using CSFramework;
//using UnityEngine;
//using UnityEngine.UI;
//using System;
//using System.Collections.Generic;

//public class M3FlyCellUI : M3CellUI
//{
//    private SchedulerFloat _fly_scheduler = new SchedulerFloat().Init(0.3f);//飞行速度是0.5
//    public M3CollectionCellUI TargetM3CollectionCellUI;//目标是CollectionCellUI
//    private Action _completed_callback;//行为完成回调

//    public M3FlyCellUI Init (M3Cell cell, Action completed_callback)//初始化
//    {
//        _completed_callback = completed_callback;//完成回调
//        RefreshCell(cell);//刷新
//        return this;
//    }

//    public M3FlyCellUI StartFly(M3HighLightCellUI high_light_cell_ui, M3CollectionCellUI collection_cell_ui)//开始飞行
//    {
//        transform.localScale = high_light_cell_ui.transform.localScale;//点击的缩放
//        transform.localPosition = high_light_cell_ui.transform.localPosition;//点击的位置
//        TargetM3CollectionCellUI = collection_cell_ui;//目标位置
//        TargetM3CollectionCellUI.StartCollect(this);//开始收集
//        return this;
//    }

//    private void Update ()//游戏中刷新
//    {
//        var pre_percent = _fly_scheduler.Percent();//前百分比为飞行百分比
//        if (_fly_scheduler.Tick(Time.deltaTime, false))//如果是false
//        {
//            transform.position = TargetM3CollectionCellUI.transform.position;//目标位置
//            transform.localScale = TargetM3CollectionCellUI.transform.localScale;//目标缩放
//            TargetM3CollectionCellUI.CompleteCollect(this);//目标设置为完成收集
//            _completed_callback?.Invoke();//不调用？
//        }
//        else if (!_fly_scheduler.IsArrived())//如果是在飞行中
//        {
//            var percent = _fly_scheduler.Percent();//百分比 = 飞行百分比
//            transform.position = Utils.LerpByPrePercent(transform.position, TargetM3CollectionCellUI.transform.position, pre_percent, percent);//调用utils里的飞行
//            transform.localScale = Utils.LerpByPrePercent(transform.localScale, TargetM3CollectionCellUI.transform.localScale, pre_percent, percent);//调用utils里的缩放
//        }
//    }
//}
using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class M3GameCollectionCellUI : M3GameCellUI
{
    public M3GameCollectionUI CollectionUI { get; private set; }//收集ui
    public int IndexInCollection { get; private set; }//收集的索引
    public M3CellInCollectionState InCollectionState { get; private set; }//收集的状态
    public bool InAdjusting { get; private set; }//是否在调整中
    public bool IsTriggerMatch = false;//是否触发match

    private SchedulerFloat _adjust_scheduler = new SchedulerFloat().Init(0.2f);
    private Vector3 _adjust_target_position;//目标位置？

    private SchedulerFloat _match_waiting_scheduler = new SchedulerFloat().Init(0.2f);//等待match刷新速度0.2
    private SchedulerFloat _match_animation_scheduler = new SchedulerFloat().Init(0.2f);//match动画的刷新速度0.2
    public static readonly Vector3 MatchScale = new Vector3(1.3f, 1.3f, 1.3f);//match的缩放1.12

    public M3GameCollectionCellUI Init (M3GameCollectionUI collection_ui)//初始化
    {
        CollectionUI = collection_ui;
        return this;
    }

    public M3GameCollectionCellUI StartCollect (M3GameFlyCellUI fly_cell_ui)//开始收集
    {
        InCollectionState = M3CellInCollectionState.Collecting;
        IsTriggerMatch = false;
        gameObject.SetActive(false);
        transform.localScale = Vector3.one;
        RefreshCell(fly_cell_ui.Cell);
        return this;
    }

    public M3GameCollectionCellUI CompleteCollect (M3GameFlyCellUI fly_cell_ui)//完成收集
    {
        InCollectionState = M3CellInCollectionState.Collected;
        gameObject.SetActive(true);
        if (IsTriggerMatch)
        {
            CollectionUI.TriggerMatch();
        }
        return this;
    }

    public M3GameCollectionCellUI StartMatch ()//开始match
    {
        InCollectionState = M3CellInCollectionState.Matching;
        _match_waiting_scheduler.Reset();//重置match的等待
        _match_animation_scheduler.Reset();//重置match的动画
        return this;
    }

    public M3GameCollectionCellUI ChangeIndexInCollection (int index, List<Vector3> position_list, bool with_animation)//变换顺序
    {
        if (InCollectionState == M3CellInCollectionState.Matching)
            return this;

        IndexInCollection = index;//顺序id
        _adjust_target_position = position_list[index];//目标顺序位置
        if (with_animation)//如果有动画
        {
            _adjust_scheduler.Reset();//重置
            InAdjusting = true;//正在调整中
        }
        else
        {
            _adjust_scheduler.SetArrived();//设置为已到达
            transform.localPosition = _adjust_target_position;//目标顺序位置
            InCollectionState = M3CellInCollectionState.Collected;//状态设置为收集完毕
            InAdjusting = false;//不在调整中
        }
        return this;
    }

    public void SetInCollectionState (M3CellInCollectionState state)//设置在消除栏里的状态
    {
        InCollectionState = state;

        if (InCollectionState == M3CellInCollectionState.Collected)//已经收集完毕了则可跟tile互动
        {
            gameObject.SetActive(true);
            
            
        }
        else // if (InCollectionState == M3CellInCollectionState.None)//否则不可
        {
            gameObject.SetActive(false);
        }
    }
    
    public void CompleteMatch ()
    {
        InCollectionState = M3CellInCollectionState.None;//状态设置未none
        gameObject.SetActive(false);//不可交互
        CollectionUI.MatchCompleted(this);//match完成
    }
    //return专用
    public void ReturnCompleteMatch()
    {
        InCollectionState = M3CellInCollectionState.None;//状态设置未none
        gameObject.SetActive(false);//不可交互
        CollectionUI.ReturnMatchCompleted(this);//match完成
    }

    private void Update ()//游戏中刷新
    {
        if (InAdjusting)//在调整中
        {
            var pre_percent = _adjust_scheduler.Percent();//前百分比
            if (_adjust_scheduler.Tick(Time.deltaTime, false))//增量时间为否
            {
                transform.localPosition = _adjust_target_position;//位置就是位置
                InAdjusting = false;//不在调整中
            }
            else if (!_adjust_scheduler.IsArrived())//在调整中？
            {
                var percent = _adjust_scheduler.Percent();//百分比
                transform.localPosition = Utils.LerpByPrePercent(transform.localPosition, _adjust_target_position, pre_percent, percent);//位置就读utiles里的
            }
        }
        
        if (InCollectionState == M3CellInCollectionState.Matching)//match过程中
        {
            if (!_match_waiting_scheduler.Tick(Time.deltaTime, false))//等待时间未结束则等待
            {
                // waiting
                CollectionUI.isMatchPause = true;
            }
            else if (_match_animation_scheduler.Tick(Time.deltaTime, false))//match动画中
            {
                CompleteMatch();
            }
            else if (!_match_animation_scheduler.IsArrived())//match动画还未到达的情况
            {
                var percent = _match_animation_scheduler.Percent();//动画百分比
                var stage_1_percent = 0.32f;//阶段1的百分比为0.32
                if (percent < stage_1_percent)//如果动画百分比小于阶段1的百分比
                {
                    var anim_percent = percent / stage_1_percent;//anim百分比 = 动画百分比/0.32
                    transform.localScale = Vector3.Lerp(Vector3.one, MatchScale, anim_percent);//缩放影响
                }
                else
                {
                    var anim_percent = (percent - stage_1_percent) / (1 - stage_1_percent);//（动画百分比-0.32）/（1-0.32）
                    transform.localScale = Vector3.Lerp(MatchScale, Vector3.zero, anim_percent);//缩放影响
                }
            }
            else
            {
                warning($"M3GameCollectionCellUI[{IndexInCollection}][{Cell.X}, {Cell.Y}] condition error");//其他情况报错
                InCollectionState = M3CellInCollectionState.Collected; // 并把状态设置为收集完毕
            }
        }
    }
}
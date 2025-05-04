using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class M3GameCollectionUI :  BaseUI
{
    public M3GamePanelUI PanelUI { get; private set; }//panelUI
    public Stack<M3GameCollectionCellUI> EmptyCellStack { get; private set; }
    public List<M3GameCollectionCellUI> CollectedCellUIList { get; private set; }
    public Queue<List<M3GameCollectionCellUI>> PrepareMatchQueue { get; private set; }
    public List<Vector3> PositionList;//vector3的list
    public static M3GameCollectionCellUI return_start;
    public bool isMatchPause;//用于判断复活
    public int textcount;//用于判断飞花位置
    public int index;
    private int matchCompletedCounter = 0;

    private Action<M3GameCollectionUI> ClickCalback_Match;
    private Action<M3GameCollectionUI> ClickCalback_Collect;
    public int currentCellType;

    protected override void on_create()
    {
        EmptyCellStack = new Stack<M3GameCollectionCellUI>();
        CollectedCellUIList = new List<M3GameCollectionCellUI>();
        PrepareMatchQueue = new Queue<List<M3GameCollectionCellUI>>();
        PositionList = new List<Vector3>();

        //for (var i = 1; i <= M3Const.CollectionMaxCellCount; ++i)//遍历消除栏的所有空位，最大值是7
        for (var i = 1; i <= M3Const.CollectionExpandMaxCellCount; ++i)
        {
            var cell_ui = create_ui<M3GameCollectionCellUI>($"CollectedCellGroup/Cell_{i}").Init(this);
            push_cell_to_stack(cell_ui);
            PositionList.Add(cell_ui.transform.localPosition);
        }
    }

    private void push_cell_to_stack(M3GameCollectionCellUI cell_ui)
    {
        cell_ui.Hide();
        EmptyCellStack.Push(cell_ui);
    }

    public M3GameCollectionUI Init(M3GamePanelUI panel_ui)
    {
        PanelUI = panel_ui;
        return this;
    }

    public bool TryCollectCell(M3GameCellUI cell_ui, M3GameHighLightCellUI light_cell_ui)
    {
        if (EmptyCellStack.Count == 0)
            return false;
        cell_ui.SetState(M3CellState.Collected);
        PanelUI.RecordGameOpt(new GameOpt
        {
            Type = GameOptType.CollectCell,
            CollectedCellUI = cell_ui,
        });

        // todo append cell
        var last_same_collected_cell_index = -1;
        var same_collected_cell_count = 0;
        for (var i = 0; i < CollectedCellUIList.Count; ++i)
        {
            var cell_ui_i = CollectedCellUIList[i];
            if (cell_ui_i.Cell.Type == cell_ui.Cell.Type)
            {
                last_same_collected_cell_index = i;
                if (cell_ui_i.InCollectionState != M3CellInCollectionState.Matching)
                    ++same_collected_cell_count;
            }
            
        }

        if (last_same_collected_cell_index != -1)
        {
            for (var i = last_same_collected_cell_index + 1; i < CollectedCellUIList.Count; ++i)
                CollectedCellUIList[i].ChangeIndexInCollection(i + 1, PositionList, true);
            start_collect(cell_ui, light_cell_ui, last_same_collected_cell_index + 1);

            if (same_collected_cell_count == M3Const.CellMatchCount - 1)
            {
                var current_collected_cell = CollectedCellUIList[last_same_collected_cell_index + 1];
                PrepareMatchQueue.Enqueue(new List<M3GameCollectionCellUI>
                {
                    CollectedCellUIList[last_same_collected_cell_index - 1],
                    CollectedCellUIList[last_same_collected_cell_index],
                    current_collected_cell,
                });
                current_collected_cell.IsTriggerMatch = true;
            }
        }
        else
        {
            start_collect(cell_ui, light_cell_ui, CollectedCellUIList.Count);
        }
        //获得cell的type值
        currentCellType = cell_ui.Cell.Type;

        //新手引导
        ClickCalback_Collect?.Invoke(this);
        return true;
    }

    public void Reset()
    {
        for (var i = CollectedCellUIList.Count - 1; i >= 0; --i)
            push_cell_to_stack(CollectedCellUIList[i]);
        CollectedCellUIList.Clear();
    }

    //return专用
    public void RemoveCollectedCell(M3Cell cell)
    {
        //var cell_ui = CollectedCellUIList.Find(a => a.Cell == cell);
        //cell_ui?.CompleteMatch();
        return_start = CollectedCellUIList.Find(a => a.Cell == cell);
        return_start?.ReturnCompleteMatch();
    }
    public void ReturnMatchCompleted(M3GameCollectionCellUI cell_ui)
    {
        var index = CollectedCellUIList.IndexOf(cell_ui);
        if (index >= 0)
        {
            for (var i = index + 1; i < CollectedCellUIList.Count; ++i)
                CollectedCellUIList[i].ChangeIndexInCollection(i - 1, PositionList, true);

            CollectedCellUIList.RemoveAt(index);
            push_cell_to_stack(cell_ui);
        }
    }

    private void start_collect(M3GameCellUI cell_ui, M3GameHighLightCellUI light_cell_ui, int index)
    {
        var empty_cell = EmptyCellStack.Pop().ChangeIndexInCollection(index, PositionList, false);
        CollectedCellUIList.Insert(empty_cell.IndexInCollection, empty_cell);
        PanelUI.HighLightLayerUI.CreateFlyCellUI(cell_ui.Cell).StartFly(light_cell_ui, empty_cell);
        sort_collected_cell();
    }

    private void sort_collected_cell()
    {
        // sort
        foreach (var collected_ui in CollectedCellUIList)
            collected_ui.transform.SetAsLastSibling();
    }

    public void TriggerMatch()
    {
        if (PrepareMatchQueue.Count > 0)
        {
            var match_cell_list = PrepareMatchQueue.Dequeue();
            foreach (var match_cell in match_cell_list)
            {
                match_cell.StartMatch();

                //match时的celltype
                //Debug.Log(match_cell.Cell.Type);
                currentCellType = match_cell.Cell.Type;
            }
            PanelUI.RecordGameOpt(new GameOpt
            {
                Type = GameOptType.Eliminate,
                CellList = match_cell_list.ConvertAll<M3Cell>(a => a.Cell),
            });

            textcount = 0;
        }
    }

    public void MatchCompleted(M3GameCollectionCellUI cell_ui)
    {
        index = CollectedCellUIList.IndexOf(cell_ui);
        if (index >= 0)
        {
            for (var i = index + 1; i < CollectedCellUIList.Count; ++i)
                CollectedCellUIList[i].ChangeIndexInCollection(i - 1, PositionList, true);

            CollectedCellUIList.RemoveAt(index);
            push_cell_to_stack(cell_ui);
        }

        ////match的回调
        matchCompletedCounter++;

        if (matchCompletedCounter == 1)
        {
            ClickCalback_Match?.Invoke(this);
        }
        if (matchCompletedCounter == 3)
        {
            matchCompletedCounter = 0; // 或者保留为3不再触发，看你需求
        }
    }

    public void CallBack_Match(Action<M3GameCollectionUI> click_callback)
    {
        ClickCalback_Match = click_callback;
    }
    public void CallBack_Collect(Action<M3GameCollectionUI> click_callback)
    {
        ClickCalback_Collect = click_callback;
    }
}

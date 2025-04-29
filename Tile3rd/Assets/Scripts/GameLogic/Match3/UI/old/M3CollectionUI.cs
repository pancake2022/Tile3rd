//using CSFramework;
//using UnityEngine;
//using UnityEngine.UI;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;


//public class M3CollectionUI : BaseUI
//{
//    public M3PanelUI PanelUI { get; private set; }//panelUI
//    public Stack<M3CollectionCellUI> EmptyCellStack { get; private set; }
//    public List<M3CollectionCellUI> CollectedCellUIList { get; private set; }
//    public Queue<List<M3CollectionCellUI>> PrepareMatchQueue { get; private set; }
//    public List<Vector3> PositionList;//vector3的list
//    public int leftcell;
//    public bool isMatchPause;
//    public static M3CollectionCellUI return_start;
//    public int textcount;

//    protected override void on_create()
//    {
//        EmptyCellStack = new Stack<M3CollectionCellUI>();
//        CollectedCellUIList = new List<M3CollectionCellUI>();
//        PrepareMatchQueue = new Queue<List<M3CollectionCellUI>>();
//        PositionList = new List<Vector3>();

//        //for (var i = 1; i <= M3Const.CollectionMaxCellCount; ++i)//遍历消除栏的所有空位，最大值是7
//        for (var i = 1; i <= M3Const.CollectionExpandMaxCellCount; ++i)
//        {
//            var cell_ui = create_ui<M3CollectionCellUI>($"CollectedCellGroup/Cell_{i}").Init(this);
//            push_cell_to_stack(cell_ui);
//            PositionList.Add(cell_ui.transform.localPosition);
//        }
//    }

//    private void push_cell_to_stack(M3CollectionCellUI cell_ui)
//    {
//        cell_ui.Hide();
//        EmptyCellStack.Push(cell_ui);
//    }

//    public M3CollectionUI Init(M3PanelUI panel_ui)
//    {
//        PanelUI = panel_ui;
//        return this;
//    }

//    public bool TryCollectCell(M3CellUI cell_ui, M3HighLightCellUI light_cell_ui)
//    {
//        if (EmptyCellStack.Count == 0)
//            return false;
//        cell_ui.SetState(M3CellState.Collected);
//        PanelUI.RecordGameOpt(new GameOpt
//        {
//            Type = GameOptType.CollectCell,
//            CollectedCellUI = cell_ui,
//        });

//        // todo append cell
//        var last_same_collected_cell_index = -1;
//        var same_collected_cell_count = 0;
//        for (var i = 0; i < CollectedCellUIList.Count; ++i)
//        {
//            var cell_ui_i = CollectedCellUIList[i];
//            if (cell_ui_i.Cell.Type == cell_ui.Cell.Type)
//            {
//                last_same_collected_cell_index = i;
//                if (cell_ui_i.InCollectionState != M3CellInCollectionState.Matching)
//                    ++same_collected_cell_count;
//            }
//        }

//        if (last_same_collected_cell_index != -1)
//        {
//            for (var i = last_same_collected_cell_index + 1; i < CollectedCellUIList.Count; ++i)
//                CollectedCellUIList[i].ChangeIndexInCollection(i + 1, PositionList, true);
//            start_collect(cell_ui, light_cell_ui, last_same_collected_cell_index + 1);

//            if (same_collected_cell_count == M3Const.CellMatchCount - 1)
//            {
//                var current_collected_cell = CollectedCellUIList[last_same_collected_cell_index + 1];
//                PrepareMatchQueue.Enqueue(new List<M3CollectionCellUI>
//                {
//                    CollectedCellUIList[last_same_collected_cell_index - 1],
//                    CollectedCellUIList[last_same_collected_cell_index],
//                    current_collected_cell,
//                });
//                current_collected_cell.IsTriggerMatch = true;
//            }
//        }
//        else
//        {
//            start_collect(cell_ui, light_cell_ui, CollectedCellUIList.Count);

//        }
//        //新手引导
//        var game_ui = _ui_manager.FindWindow<GameUI>();
//        game_ui.gameGuideUI.NewBeGuide_FirstStep();
//        return true;
//    }

//    public void Reset()
//    {
//        for (var i = CollectedCellUIList.Count - 1; i >= 0; --i)
//            push_cell_to_stack(CollectedCellUIList[i]);
//        CollectedCellUIList.Clear();
//    }

//    //return专用？
//    public void RemoveCollectedCell(M3Cell cell)
//    {
//        //var cell_ui = CollectedCellUIList.Find(a => a.Cell == cell);
//        //cell_ui?.CompleteMatch();
//        return_start = CollectedCellUIList.Find(a => a.Cell == cell);
//        //return_start?.CompleteMatch();
//        return_start?.ReturnCompleteMatch();
//    }

//    private void start_collect(M3CellUI cell_ui, M3HighLightCellUI light_cell_ui, int index)
//    {
//        var empty_cell = EmptyCellStack.Pop().ChangeIndexInCollection(index, PositionList, false);
//        CollectedCellUIList.Insert(empty_cell.IndexInCollection, empty_cell);
//        PanelUI.HighLightLayerUI.CreateFlyCellUI(cell_ui.Cell).StartFly(light_cell_ui, empty_cell);
//        sort_collected_cell();
//    }

//    private void sort_collected_cell()
//    {
//        // sort
//        foreach (var collected_ui in CollectedCellUIList)
//            collected_ui.transform.SetAsLastSibling();
//    }

//    public void TriggerMatch()
//    {

//        if (PrepareMatchQueue.Count > 0)
//        {
//            var match_cell_list = PrepareMatchQueue.Dequeue();
//            foreach (var match_cell in match_cell_list)
//                match_cell.StartMatch();

//            PanelUI.RecordGameOpt(new GameOpt
//            {
//                Type = GameOptType.Eliminate,
//                CellList = match_cell_list.ConvertAll<M3Cell>(a => a.Cell),
//            });

//            textcount = 0;
//        }
//    }

//    public void MatchCompleted(M3CollectionCellUI cell_ui)
//    {
//        var index = CollectedCellUIList.IndexOf(cell_ui);

//        //记录特效起始位置
//        var game_ui = _ui_manager.FindWindow<GameUI>();

//        if (index >= 0)
//        {
//            for (var i = index + 1; i < CollectedCellUIList.Count; ++i)
//                CollectedCellUIList[i].ChangeIndexInCollection(i - 1, PositionList, true);

//            CollectedCellUIList.RemoveAt(index);
//            push_cell_to_stack(cell_ui);
//        }

//        //match成功就去掉一张牌
//        //leftcell = leftcell - 1;
//        leftcell--;
//        play_sound("sound_tile_break");
//        isMatchPause = false;

//        //消除牌时显示特效
//        textcount++;
//        if (textcount == 1)
//        {
//            game_ui.rewardfly_positionX = Convert.ToInt32(CollectedCellUIList[index].transform.localPosition.x);
//            game_ui.rewardfly_positionY = Convert.ToInt32(this.transform.localPosition.y);
//            game_ui.game_rewarditemfly();
//        }

//        ////新手引导
//        game_ui.gameGuideUI.NewBeGuide_Match();
//        game_ui.gameItemGroupUI.BloomTipsRefresh();
//    }

//    //return专用
//    public void ReturnMatchCompleted(M3CollectionCellUI cell_ui)
//    {
//        var index = CollectedCellUIList.IndexOf(cell_ui);
//        if (index >= 0)
//        {
//            for (var i = index + 1; i < CollectedCellUIList.Count; ++i)
//                CollectedCellUIList[i].ChangeIndexInCollection(i - 1, PositionList, true);

//            CollectedCellUIList.RemoveAt(index);
//            push_cell_to_stack(cell_ui);
//        }
//    }
//}
//using CSFramework;
//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections.Generic;
//using System.Collections;


///// <summary>
///// 游戏操作步骤类型
///// </summary>
//public enum GameOptType
//{
//    None,
//    /// <summary>
//    /// 收集牌
//    /// </summary>
//    CollectCell,
//    /// <summary>
//    /// 消除牌
//    /// </summary>
//    Eliminate,
//}

///// <summary>
///// 游戏操作步骤
///// </summary>
//public class GameOpt
//{
//    public GameOptType Type;
//    public M3CellUI CollectedCellUI;
//    public List<M3Cell> CellList;
//}

//public class M3PanelUI : BaseUI
//{
//    public M3Panel Panel;
//    public M3CollectionUI CollectionUI;//声明消除栏
//    public M3LayerUI[] LayerUIArray;//声明整体布局（layer集合）
//    public M3LayerUI HighLightLayerUI;//声明单独的layer
//    public M3HighLightCellUI HighLightCellUI;//声明可交互cell
//    private RectTransform _layer_group_rt;
//    public Stack<GameOpt> OptStack = new Stack<GameOpt>();
//    public static M3CellUI return_end;
//    //public M3CellUI return_end;
//    public ReturnFlyCellUI returnfly;

//    protected override void on_create()
//    {
//        _layer_group_rt = find_component<RectTransform>("LayerGroup");//找到LayerGroup
//        CollectionUI = create_ui<M3CollectionUI>("Collection").Init(this);//创建消除
//    }


//    public M3PanelUI Init(M3Panel panel)//初始化游戏界面
//    {
//        Panel = panel;
//        LayerUIArray = new M3LayerUI[panel.LayerList.Count];

//        foreach (var layer in panel.LayerList)//遍历所有的layer，创建并刷新所有layer
//        {
//            LayerUIArray[layer.Index] = create_ui<M3LayerUI>("M3/LayerTemplate", _layer_group_rt).Init(layer);
//        }

//        HighLightLayerUI = create_ui<M3LayerUI>("M3/LayerTemplate", _layer_group_rt);//生成layer
//        HighLightCellUI = create_ui<M3HighLightCellUI>("M3/CellTemplate", HighLightLayerUI.ContentRT);//生成cell
//        HighLightCellUI.SetHighLight(false, null);//默认不可交互

//        PanelScaleSet();
//        refresh_cell_state();//刷新cell状态
//        return this;
//    }
//    //屏幕适配制作在这里
//    private void PhoneScaleSet()
//    {
//        var makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
//        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
//        var levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();
//        var all_level = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().LevelConfigList;
//        var all_quest = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().QuestConfigList;
//        var rect = find_component<RectTransform>("LayerGroup");
//        var position = rect.localPosition;

//        //如果是任务关则默认
//        if (shareDataGlobalConfig._quest_level == true) 
//        {
//            var quest = all_quest.Find(a => a.ID == makeoverStorage.CurrentQuestID);
//            rect.localScale = new Vector3(quest.PanelScale, quest.PanelScale);
//            position.x = quest.PanelRect;
//            rect.localPosition = position;
//        }
//        //如果是普通关则读表
//        else
//        {
//            var level = all_level.Find(a => a.ID == levelStorage.CurrentLevel);
//            rect.localScale = new Vector3(level.PanelScale, level.PanelScale);
//            position.x = level.PanelRect;
//            rect.localPosition = position;
//        }
//    }
//    private void PadScaleSet()
//    {
//        var makeoverStorage = _ui_manager.Framework.StorageManager.Storage<MakeOverStorage>();
//        var shareDataGlobalConfig = _ui_manager.Framework.ShareDataManager.Data<ShareDataGlobalConfig>();
//        var levelStorage = _ui_manager.Framework.StorageManager.Storage<LevelStorage>();
//        var all_level = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().LevelConfigList;
//        var all_quest = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().QuestConfigList;
//        var rect = find_component<RectTransform>("LayerGroup");
//        var position = rect.localPosition;

//        //如果是任务关则默认
//        if (shareDataGlobalConfig._quest_level == true)
//        {
//            var quest = all_quest.Find(a => a.ID == makeoverStorage.CurrentQuestID);
//            rect.localScale = new Vector3(quest.PadScale, quest.PadScale);
//            position.x = quest.PanelRect;
//            rect.localPosition = position;
//        }
//        //如果是普通关则读表
//        else
//        {
//            var level = all_level.Find(a => a.ID == levelStorage.CurrentLevel);
//            rect.localScale = new Vector3(level.PadScale, level.PadScale);
//            position.x = level.PanelRect;
//            rect.localPosition = position;
//        }
//    }
//    private void PanelScaleSet()
//    {
//        var _testlayer = find_component<RectTransform>("LayerGroup");
//        if (((float)UnityEngine.Screen.width / (float)UnityEngine.Screen.height) >= 0.7)//ipad=3:4
//            PadScaleSet();
//        else
//            PhoneScaleSet();
//    }

//    /// <summary>
//    /// 记录操作
//    /// </summary>
//    /// <param name="opt"></param>
//    public void RecordGameOpt (GameOpt opt)
//    {
//        if (opt.Type == GameOptType.CollectCell)
//            OptStack.Push(opt);
//        else if (opt.Type == GameOptType.Eliminate)
//            // OptStack.Push(opt);
//            OptStack.Clear(); // 消除之后暂时清除所有的记录
//    }

//    /// <summary>
//    /// 回退操作
//    /// </summary>
//    public bool TryRevertGameOpt ()
//    {
//        if (OptStack.Count > 0)
//        {
//            var opt = OptStack.Pop();

//            if (opt.Type == GameOptType.CollectCell)
//            {
//                return_end = opt.CollectedCellUI;

//                // 还原收集栏的数据
//                CollectionUI.RemoveCollectedCell(return_end.Cell);

//                // 刷新盘面的状态
//                CreateReturnFlyCellUI(return_end.Cell).StartFly();
//            }
//            else if (opt.Type == GameOptType.Eliminate)
//            {
//            }
//            return true;
//        }
//        return false;
//    }
//    public M3PanelUI RetryReset()
//    {
//        OptStack.Clear();
//        CollectionUI.Reset();
//        CleanLayerUI();
//        Debug.Log(Panel.LayerList.Count);
//        return this;
//    }
//    //测试
//    private void CleanLayerUI()
//    {
//        foreach (var layer_ui in LayerUIArray)
//            destroy_ui(layer_ui);
//        //destroy_ui(HighLightLayerUI);
//        //Panel.LayerList.Clear();
//    }

//    public M3CellUI FindCellUI (M3Cell cell)
//    {
//        foreach (var layer_ui in LayerUIArray)
//        {
//            foreach (var cell_ui in layer_ui.CellUIArray)
//            {
//                if (cell_ui?.Cell == cell)
//                    return cell_ui;
//            }
//        }
//        return null;
//    }

//    public void refresh_cell_state(bool reset_collected = false)//刷新cell状态
//    {
//        var max_layer_index = LayerUIArray.Length - 1;//定义最大layer数量
//        foreach (var layer_ui in LayerUIArray)//遍历layer集合里的全部layer
//        {
//            var current_layer_index = layer_ui.Layer.Index;//定义当前layer并付初始值
//            if (current_layer_index == max_layer_index)//如果当前layer为最大layer
//            {
//                foreach (var cell_ui in layer_ui.CellUIArray)//遍历其中的所有cell
//                {
//                    if (cell_ui)//如果有
//                    {
//                        if (reset_collected || cell_ui.State != M3CellState.Collected)//如果是不是已收集状态
//                            cell_ui.SetState(M3CellState.TopInLayer);//则认为cell是在top层可互动
//                    }
//                }
//            }
//            else
//            {
//                foreach (var cell_ui in layer_ui.CellUIArray)//遍历全部cell
//                {
//                    if (cell_ui)//如果有
//                    {
//                        if (reset_collected || cell_ui.State != M3CellState.Collected)//如果不是已收集状态
//                        {
//                            if (CheckOverlap(layer_ui, cell_ui))//如果是重叠的
//                                cell_ui.SetState(M3CellState.DarkInLayer);//则牌不可交互
//                            else
//                                cell_ui.SetState(M3CellState.TopInLayer);//否则可交互
//                        }
//                    }
//                }
//            }
//        }
//    }

//    private bool CheckOverlap(M3LayerUI layer_ui, M3CellUI cell_ui)//检查牌是否重叠
//    {

//        for (var may_overlap_layer_index = LayerUIArray.Length - 1; may_overlap_layer_index > layer_ui.Layer.Index; --may_overlap_layer_index)
//        {
//            var may_overlap_layer = LayerUIArray[may_overlap_layer_index];
//            for (var offset_x = -1; offset_x <= 1; ++offset_x)
//            {
//                for (var offset_y = -1; offset_y <= 1; ++offset_y)
//                {
//                    var may_overlap_cell = may_overlap_layer.CellUIArray[cell_ui.Cell.X + offset_x, cell_ui.Cell.Y + offset_y];
//                    if (may_overlap_cell && may_overlap_cell.State <= M3CellState.DarkInLayer)

//                        return true;
//                }
//            }
//        }
//        return false;
//    }

//    private void Update()//游戏更新
//    {
//        //点击cell
//        if (Input.GetMouseButton(0))
//        {
//            var selected_cell_ui = get_selected_cell_ui();//声明收集cell
//            if (selected_cell_ui)//如果可收集
//            {
//                HighLightCellUI.SetHighLight(true, selected_cell_ui.Cell);//就把牌各种设置为true
//            }

//            else
//                HighLightCellUI.SetHighLight(false, null);//否则不行
//        }
//        else
//        {
//            HighLightCellUI.SetHighLight(false, null);//如果不可收集就各种不行
//        }

//        if (Input.GetMouseButtonUp(0))
//        {
//            var selected_cell_ui = get_selected_cell_ui();//声明收集cell
//            if (selected_cell_ui)//如果可收集
//            {
//                if (CollectionUI.TryCollectCell(selected_cell_ui, HighLightCellUI))//尝试收集并刷新ui
//                    refresh_cell_state();
//                play_sound("sound_tile_click");
//            }
//        }
//    }

//    private M3CellUI get_selected_cell_ui()//收集cell
//    {
//        var _game_ui = _ui_manager.FindWindow<GameUI>();
//        var camera = _ui_manager.Framework.Context.UICamera;

//        if (_game_ui.isPause == false)
//        {
//            for (var i = LayerUIArray.Length - 1; i >= 0; --i)
//            {
//                var layer = LayerUIArray[i];
//                foreach (var cell_ui in layer.CellUIArray)
//                {
//                    if (cell_ui && cell_ui.State == M3CellState.TopInLayer)
//                    {
//                        if (RectTransformUtility.RectangleContainsScreenPoint(cell_ui.transform as RectTransform, Input.mousePosition, camera))
//                            return cell_ui;
//                    }
//                }
//            }
//            return null;
//        }

//        else
//        {
//            return null;
//        }
//    }


//    //消除道具
//    public List<M3CellUI> get_eliminate_cell_ui()
//    {
//        var celllist = new List<M3CellUI>();//声明list
//        var A = 0;

//        //如果消除栏内有牌
//        if (CollectionUI.CollectedCellUIList.Count != 0)
//        {
//            //消除栏里仅有一张牌时
//            if (CollectionUI.CollectedCellUIList.Count == 1)
//            {
//                var cell_1 = CollectionUI.CollectedCellUIList.Count - 1;
//                var cell_ui_1 = CollectionUI.CollectedCellUIList[cell_1];

//                //遍历panel，并找到一张type一样的牌
//                for (var i = LayerUIArray.Length - 1; i >= 0; --i)
//                {
//                    var layer = LayerUIArray[i];
//                    foreach (var cell_ui in layer.CellUIArray)
//                    {
//                        if ((cell_ui && cell_ui.State == M3CellState.TopInLayer) || (cell_ui && cell_ui.State == M3CellState.DarkInLayer))
//                        {
//                            var celltype = cell_ui.Cell.Type;
//                            if (celltype == cell_ui_1.Cell.Type)
//                            {
//                                if (celllist.Count < 2)
//                                    celllist.Add(cell_ui);
//                            }
//                        }
//                    }
//                }
//                return celllist;
//            }

//            //消除栏里有2张或以上牌时
//            if (CollectionUI.CollectedCellUIList.Count > 1)
//            {
//                var cell_1 = CollectionUI.CollectedCellUIList.Count - 1;
//                var cell_ui_1 = CollectionUI.CollectedCellUIList[cell_1];
//                var cell_2 = CollectionUI.CollectedCellUIList.Count - 2;
//                var cell_ui_2 = CollectionUI.CollectedCellUIList[cell_2];

//                //相邻两张牌不一样
//                if (cell_ui_1.Cell.Type != cell_ui_2.Cell.Type)
//                {
//                    for (var i = LayerUIArray.Length - 1; i >= 0; --i)
//                    {
//                        var layer = LayerUIArray[i];
//                        foreach (var cell_ui in layer.CellUIArray)
//                        {
//                            if ((cell_ui && cell_ui.State == M3CellState.TopInLayer) || (cell_ui && cell_ui.State == M3CellState.DarkInLayer))
//                            {
//                                var celltype = cell_ui.Cell.Type;
//                                if (celltype == cell_ui_1.Cell.Type)
//                                {
//                                    if (celllist.Count < 2)
//                                        celllist.Add(cell_ui);
//                                }
//                            }
//                        }
//                    }
//                    return celllist;
//                }
//                //如果相邻两张牌一样
//                if (cell_ui_1.Cell.Type == cell_ui_2.Cell.Type)
//                {
//                    for (var i = LayerUIArray.Length - 1; i >= 0; --i)
//                    {
//                        var layer = LayerUIArray[i];
//                        foreach (var cell_ui in layer.CellUIArray)
//                        {
//                            if ((cell_ui && cell_ui.State == M3CellState.TopInLayer) || (cell_ui && cell_ui.State == M3CellState.DarkInLayer))
//                            {
//                                var celltype = cell_ui.Cell.Type;
//                                if (celltype == cell_ui_1.Cell.Type)
//                                {
//                                    if (celllist.Count < 1)
//                                        celllist.Add(cell_ui);
//                                }
//                            }
//                        }
//                    }
//                    return celllist;
//                }
//            }
//        }

//        //如果消除栏内没有牌
//        if (CollectionUI.CollectedCellUIList.Count == 0)
//        {
//            for (var i = LayerUIArray.Length - 1; i >= 0; --i)
//            {
//                var layer = LayerUIArray[i];
//                //尝试给array随机
//                foreach (var cell_ui in layer.CellUIArray)
//                {
//                    if ((cell_ui && cell_ui.State == M3CellState.TopInLayer) || (cell_ui && cell_ui.State == M3CellState.DarkInLayer))
//                    {
//                        var celltype = cell_ui.Cell.Type;//声明celltype
//                        if (A == 0)
//                        {
//                            A = celltype;
//                        }
//                        else//A不等于0
//                        {
//                            celltype = A;
//                        }

//                        if (cell_ui.Cell.Type == celltype)
//                        {
//                            if (celllist.Count < 3)
//                            {
//                                celllist.Add(cell_ui);
//                            }
//                        }
//                    }
//                }
//            }
//            return celllist;
//        }
//        return null;
//    }

//    public ReturnFlyCellUI CreateReturnFlyCellUI(M3Cell cell)//cell的飞行
//    {
//        returnfly = create_ui<ReturnFlyCellUI>("M3/CellTemplate", HighLightLayerUI.ContentRT);
//        returnfly.Init(cell, () => refresh_cell_state(), () => destroy_ui(returnfly));
//        return returnfly;
//    }
//}
using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GameTileRandom : BaseUI
{
    public GameUI Game;
    private List<M3GameCellUI> allCellList;
    private List<M3GameCellUI> currentCellList;
    private RectTransform Cat_Stay;
    private RectTransform Cat_Run;
    private RectTransform Change;
    private RectTransform cat1;
    private RectTransform cat2;
    public int Condition = 1;
    public int Condition_Count = 0;

    public GameTileRandom Init(GameUI game)//PanelUI的初始化
    {
        Game = game;
        return this;
    }
    protected override void on_create()
    {
        allCellList = new List<M3GameCellUI>();
        currentCellList = new List<M3GameCellUI>();
        InitShow();
        ConditionShow1();
    }
    private void InitShow()
    {
        Cat_Stay = find_component<RectTransform>("TileRandom_Cat_Stay");
        Cat_Run = find_component<RectTransform>("TileRandom_Cat_Run");
        Change = find_component<RectTransform>("TileRandom_Change");
        cat1 = find_component<RectTransform>("TileRandom_Cat_Stay/Cat1");
        cat2 = find_component<RectTransform>("TileRandom_Cat_Stay/Cat2");
        Cat_Stay.SetActive(false);
        Cat_Run.SetActive(false);
        Change.SetActive(false);
        cat1.SetActive(false);
        cat2.SetActive(false);
    }
    public void RandomTileChange()
    {
        if (Game.leftCell > 9)
            ChangeCondition();
    }
    private void RefreshTile()
    {
        foreach (var cell in currentCellList)
        {
            cell.ChangeBack();
        }
    }
    private void ListClear()
    {
        allCellList.Clear();
        currentCellList.Clear();
    }
    private void CreatAllList()
    {
        foreach (var layer in Game._panel_ui.LayerUIArray)
        {
            foreach (var cell in layer.CellUIArray)
            {
                if (cell != null)
                {
                    if (cell.State == M3CellState.TopInLayer)
                        allCellList.Add(cell);
                }
            }
        }
    }
    private void CreatCurrentList()
    {
        System.Random random = new System.Random();
        if (allCellList.Count + 1 >= 3)
        {
            int targetCount = random.Next(4, Math.Min(7, allCellList.Count + 1));
            while (currentCellList.Count < targetCount)
            {
                int randomIndex = random.Next(allCellList.Count);
                M3GameCellUI randomCell = allCellList[randomIndex];

                if (!currentCellList.Contains(randomCell))
                    currentCellList.Add(randomCell);
            }
        }
    }
    private void ChangeCondition()
    {
        //额外判断 - 复活和remove会把count置为0
        //额外判断 - retry会重制猫状态
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];
        if (Condition < 3)
        {
            Condition_Count++;
            if (Condition_Count >= 3)
            {
                Condition++;
                Condition_Count = 0;
                ChangeConditionShow();
            }
        }

        if (Condition == 3)
            TileChangeCount(2,4,100);
        if (Condition == 4)
            TileChangeCount(globalconfig.RandomTile_Count_Min, globalconfig.RandomTile_Count_Max, globalconfig.RandomTile_Count_Rate);
    }
    
    private void TileChangeCount(int valueMin, int valueMax, int valueRate)
    {
        var gameConfigGroup = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();
        var globalconfig = gameConfigGroup.GlobalConfigList[0];

        Condition_Count++;
        if (Condition_Count > valueMin && Condition_Count < valueMax)
        {
            System.Random random = new System.Random();
            int randomValue = random.Next(1, 100);
            if (randomValue <= valueRate) 
            {
                if (Condition < 4)
                    Condition++;
                Condition_Count = 0;
                ChangeConditionShow();
            }
        }
        if (Condition_Count == globalconfig.RandomTile_Count_Max)
        {
            if (Condition < 4)
                Condition++;
            Condition_Count = 0;
            ChangeConditionShow();
        }
    }
    public void ChangeConditionShow()
    {
        InitShow();
        if (Condition == 1)
            ConditionShow1();
        if (Condition == 2)
            ConditionShow2();
        if (Condition == 3)
            ConditionShow3();
        if (Condition == 4)
            ConditionShow4();
    }
    private void ConditionShow1()
    {
        //猫蹲着
        Cat_Stay.SetActive(true);
        cat1.SetActive(true);
    }
    private void ConditionShow2()
    {
        //猫站起来了
        Cat_Stay.SetActive(true);
        cat2.SetActive(true);
        var catanim = find_component<Animator>("TileRandom_Cat_Stay/Cat2/Panel");
        catanim.SetBool("stay", true);
    }
    async void ConditionShow3()
    {
        //猫跑了
        Cat_Run.SetActive(true);
    }
    async void ConditionShow4()
    {
        //猫横穿并翻牌
        Game.GamePause();
        Change.SetActive(true);
        await Task.Delay(TimeSpan.FromSeconds(2.5));

        RefreshTile();
        ListClear();
        CreatAllList();
        CreatCurrentList();

        foreach (var cell in currentCellList)
        {
            cell.ChangeTile();
        }
        Game.GameActiveDelay();
    }
}
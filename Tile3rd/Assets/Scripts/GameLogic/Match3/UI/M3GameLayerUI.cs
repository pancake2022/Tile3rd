using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class M3GameLayerUI : BaseUI
{
    public M3Layer Layer{ get; private set; }
    public M3GameCellUI[,] CellUIArray;//cell的集合
    public RectTransform ContentRT { get; private set; }//生成位置？
    public int max;

    protected override void on_create()
    {
        ContentRT = find_component<RectTransform>("Content");//找到Content
        CellUIArray = new M3GameCellUI[M3Const.LayerSize + 1, M3Const.LayerSize + 1];//实例化一个cell的集合
    }

    public M3GameLayerUI Init (M3Layer layer, bool is_editor = false)//初始化
    {
        Layer = layer;
        foreach (var cell in layer.CellList)//遍历全部cell
        {
            CellUIArray[cell.X, cell.Y] = create_ui<M3GameCellUI>("M3/CellTemplate", ContentRT)//cell集合的坐标
                .RefreshCell(cell, is_editor)
                .RefreshPosition();
        }
        return this;
    }

    public M3GameFlyCellUI CreateFlyCellUI (M3Cell cell)//cell的飞行
    {
        var cell_ui = create_ui<M3GameFlyCellUI>("M3/CellTemplate", ContentRT);
        cell_ui.Init(cell, () => destroy_ui(cell_ui) );
        return cell_ui;
    }
}
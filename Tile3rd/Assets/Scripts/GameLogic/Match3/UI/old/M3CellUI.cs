//using CSFramework;
//using UnityEngine;
//using UnityEngine.UI;
//using System;
//using System.Collections.Generic;

//public class M3CellUI : BaseUI
//{
//    public M3Cell Cell;
//    public Image Image;
//    public RectTransform BrushFixedSignRT;
//    public M3CellState State { get; private set; }

//    protected override void on_create()
//    {
//        Image = find_component<Image>("CellBG");
//        BrushFixedSignRT = find_component<RectTransform>("BrushFixedSign");
//    }

//    public M3CellUI RefreshCell (M3Cell cell, bool is_editor = false)
//    {
//        Cell = cell;
//        if (Cell != null)//如果cell不为空
//        {
//            TileSelect();//变换套牌花色
//            BrushFixedSignRT.SetActive(is_editor && !Cell.IsBF);
//        }
//        else
//        {
//            BrushFixedSignRT.SetActive(false);
//        }
//        return this;
//    }

//    public void ResetState ()
//    {
//        State = M3CellState.None;
//        RefreshCell(Cell);
//    }

//    public void SetState (M3CellState state)//cell的状态
//    {
//        State = state;

//        if (State == M3CellState.TopInLayer)//可点击
//        {
//            gameObject.SetActive(true);
//            Image.color = Color.white;
//        }
//        else if (State == M3CellState.DarkInLayer)//不可点击
//        {
//            gameObject.SetActive(true);
//            Image.color = Color.gray;
//        }
//        else if (State == M3CellState.Collected)//在收集器中
//        {
//            gameObject.SetActive(false);
//        }
//        else // if (State == M3CellState.None)
//        {
//            gameObject.SetActive(false);
//        }
//    }

//    public M3CellUI RefreshPosition ()//刷新位置
//    {
//        var rt = transform as RectTransform;
//        rt.anchoredPosition = new Vector2(Cell.X * rt.rect.size.x * 0.5f, Cell.Y * rt.rect.size.y * 0.5f);//偏移0.5
//        name = $"({Cell.X},{Cell.Y})";//名称就是（坐标）
//        return this;
//    }

//    private void TileSelect()
//    {
//        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
//        if (tile2storage.CurrentTileID == 1)
//            Image.sprite = find_sprite("M3Tile01", "e" + Cell.Type.ToString("D2"));//找图集
//        if (tile2storage.CurrentTileID == 2)
//            Image.sprite = find_sprite("M3Tile02", "e" + Cell.Type.ToString("D2"));//找图集
//        if (tile2storage.CurrentTileID == 3)
//            Image.sprite = find_sprite("M3Tile03", "e" + Cell.Type.ToString("D2"));//找图集
//    }
//}
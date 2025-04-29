using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class M3GameCellUI : BaseUI
{
    public M3Cell Cell;
    public Image Image;
    public RectTransform BrushFixedSignRT;
    public M3CellState State { get; private set; }

    protected override void on_create()
    {
        Image = find_component<Image>("CellBG");
        BrushFixedSignRT = find_component<RectTransform>("BrushFixedSign");
    }

    public M3GameCellUI RefreshCell (M3Cell cell, bool is_editor = false)
    {
        Cell = cell;
        if (Cell != null)//如果cell不为空
        {
            TileSelect();//变换套牌花色
            BrushFixedSignRT.SetActive(is_editor && !Cell.IsBF);
        }
        else
        {
            BrushFixedSignRT.SetActive(false);
        }
        return this;
    }

    private void ResetState ()
    {
        State = M3CellState.None;
        RefreshCell(Cell);
    }

    public void SetState (M3CellState state)//cell的状态
    {
        State = state;
        
        if (State == M3CellState.TopInLayer)//可点击
        {
            gameObject.SetActive(true);
            Image.color = Color.white;
        }
        else if (State == M3CellState.DarkInLayer)//不可点击
        {
            gameObject.SetActive(true);
            Image.color = Color.gray;
        }
        else if (State == M3CellState.Collected)//在收集器中
        {
            gameObject.SetActive(false);
        }
        else // if (State == M3CellState.None)
        {
            gameObject.SetActive(false);
        }
    }

    public M3GameCellUI RefreshPosition ()//刷新位置
    {
        var rt = transform as RectTransform;
        rt.anchoredPosition = new Vector2(Cell.X * rt.rect.size.x * 0.5f, Cell.Y * rt.rect.size.y * 0.5f);//偏移0.5
        name = $"({Cell.X},{Cell.Y})";//名称就是（坐标）
        return this;
    }

    private void TileSelect()
    {
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        var all_tile = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().CollectionConfigList;
        var currentTile = all_tile.Find(a => a.ID == tile2storage.CurrentTileID);
        Image.sprite = find_sprite($"{currentTile.TilePack}", "e" + Cell.Type.ToString("D2"));//找图集
    }
    public void ChangeTile()
    {
        //需要去找对应图集里的e00，否则会死机
        //从亮的tile里随机3个进行改变
        var tile2storage = _ui_manager.Framework.StorageManager.Storage<Tile2Storage>();
        var all_tile = _ui_manager.Framework.ConfigManager.SingleConfigGroup<GameConfigGroup>().CollectionConfigList;
        var currentTile = all_tile.Find(a => a.ID == tile2storage.CurrentTileID);
        if (State == M3CellState.TopInLayer)
            Image.sprite = find_sprite($"{currentTile.TilePack}", "e00");
    }
    public void ChangeBack()
    {
        RefreshCell(Cell, false);
        TileSelect();
    }
}
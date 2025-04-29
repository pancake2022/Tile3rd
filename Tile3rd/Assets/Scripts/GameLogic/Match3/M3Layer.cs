using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class M3Layer
{
    public int Index; // 当层的序列
    public bool IsOffset => Index % 2 != 0;
    public List<M3Cell> CellList = new List<M3Cell>(); // 当层的牌列表

    public M3Layer () {}
    public M3Layer (M3Layer layer)
    {
        Index = layer.Index;
        for (var i = 0; i < layer.CellList.Count; ++i)
            CellList.Add(new M3Cell(layer.CellList[i]));
    }

    public void IndexUp ()
    {
        ++Index;
        for (var i = CellList.Count - 1; i >= 0; --i)
        {
            var cell = CellList[i];
            ++cell.X;
            ++cell.Y;
            if (cell.X > M3Const.LayerSize || cell.Y > M3Const.LayerSize)
                CellList.RemoveAt(i);
        }
    }

    public void IndexDown ()
    {
        --Index;
        for (var i = CellList.Count - 1; i >= 0; --i)
        {
            var cell = CellList[i];
            --cell.X;
            --cell.Y;
            if (cell.X < 0 || cell.Y < 0)
                CellList.RemoveAt(i);
        }
    }

    public void MoveUp ()
    {
        for (var i = CellList.Count - 1; i >= 0; --i)
        {
            var cell = CellList[i];
            cell.Y += M3Const.SizeStep;
            if (cell.Y > M3Const.LayerSize)
                CellList.RemoveAt(i);
            
        }
    }

    public void MoveRight ()
    {
        for (var i = CellList.Count - 1; i >= 0; --i)
        {
            var cell = CellList[i];
            cell.X += M3Const.SizeStep;
            if (cell.X > M3Const.LayerSize)
                CellList.RemoveAt(i);
        }
    }

    public void MoveDown ()
    {
        for (var i = CellList.Count - 1; i >= 0; --i)
        {
            var cell = CellList[i];
            cell.Y -= M3Const.SizeStep;
            if (cell.Y < 0)
                CellList.RemoveAt(i);
        }
    }

    public void MoveLeft ()
    {
        for (var i = CellList.Count - 1; i >= 0; --i)
        {
            var cell = CellList[i];
            cell.X -= M3Const.SizeStep;
            if (cell.X < 0)
                CellList.RemoveAt(i);
        }
    }

    public static void Foreach (Action<int, int> cb)
    {
        for (var x = 0; x < M3Const.LayerSize; x += M3Const.SizeStep)
            for (var y = M3Const.LayerSize - 1; y >= 0; y -= M3Const.SizeStep)
                cb(x, y);

        for (var x = 1; x <= M3Const.LayerSize; x += M3Const.SizeStep)
            for (var y = M3Const.LayerSize; y > 0; y -= M3Const.SizeStep)
                cb(x, y);
    }

    public static void Foreach (bool IsOffset, Action<int, int> cb)
    {
        if (IsOffset)
        {
            for (var x = 1; x <= M3Const.LayerSize; x += M3Const.SizeStep)
                for (var y = M3Const.LayerSize; y > 0; y -= M3Const.SizeStep)
                    cb(x, y);
        }
        else
        {
            for (var x = 0; x < M3Const.LayerSize; x += M3Const.SizeStep)
                for (var y = M3Const.LayerSize - 1; y >= 0; y -= M3Const.SizeStep)
                    cb(x, y);
        }
    }

    public static void Foreach (bool IsOffset, Func<int, int, bool> cb)
    {
        if (IsOffset)
        {
            for (var x = 1; x <= M3Const.LayerSize; x += M3Const.SizeStep)
                for (var y = M3Const.LayerSize; y > 0; y -= M3Const.SizeStep)
                    if (cb(x, y))
                        return;
        }
        else
        {
            for (var x = 0; x < M3Const.LayerSize; x += M3Const.SizeStep)
                for (var y = M3Const.LayerSize - 1; y >= 0; y -= M3Const.SizeStep)
                    if (cb(x, y))
                        return;
        }
    }
}
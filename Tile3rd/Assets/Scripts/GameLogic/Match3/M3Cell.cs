using System;

public enum M3CellState
{
    None, // 未初始化
    TopInLayer, // 静止在层中，可点击
    DarkInLayer, // 静止在层中，不可点击
    Collected, // 静止在收集器中
}

public enum M3CellInCollectionState
{
    None,
    Collected,
    Collecting,
    Matching,
}

[Serializable]
public class M3Cell
{
    public int Type; // 花色，用来记录牌的花色
    public bool IsBF; // 花色是否被笔刷固定了 IsBF
    public int X; // X坐标
    public int Y; // Y坐标

    public M3Cell () {}
    public M3Cell (M3Cell cell)
    {
        Type = cell.Type;
        IsBF = cell.IsBF;
        X = cell.X;
        Y = cell.Y;
    }
}
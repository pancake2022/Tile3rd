public class M3Const
{
    public static readonly string M3ConfigPath = "Config/M3";
    public static readonly string M3PanelConfigPath = $"{M3ConfigPath}/Panel";
    public static readonly int CellMatchCount = 3;
    public static readonly int CellSize = 10;
    public static readonly int LayerSize = 20;
    public static readonly int SizeStep = 2;
    public static readonly int CellTypeCount = 15; // 牌面种类
    public static readonly int CellTypeEmpty = -1;
    public static readonly int CellTypeRandom = 0;
    public static readonly int CollectionMaxCellCount = 7; // 收集缓存区最大数量
    public static readonly int CollectionExpandMaxCellCount = 9; // 收集缓存区最大数量
}
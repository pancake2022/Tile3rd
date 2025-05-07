using UnityEngine;
using CSFramework;

public class GameConfigManager : WindowUI
{
    private static bool _initialized;
    public static CommonStorage CommonStorage { get; private set; }
    public static Tile2Storage Tile2Storage { get; private set; }
    public static MakeOverStorage MakeOverStorage { get; private set; }
    public static LevelStorage LevelStorage { get; private set; }
    public static ShareDataGlobalConfig ShareDataGlobalConfig { get; private set; }
    public static GameConfigGroup GameConfigGroup { get; private set; }
    public static GlobalConfig GlobalConfig => GameConfigGroup?.GlobalConfigList?[0];


    public static void Initialize(UIManager uiManager)
    {
        if (_initialized) return;

        var framework = uiManager.Framework;
        CommonStorage = framework.StorageManager.Storage<CommonStorage>();
        Tile2Storage = framework.StorageManager.Storage<Tile2Storage>();
        MakeOverStorage = framework.StorageManager.Storage<MakeOverStorage>();
        LevelStorage = framework.StorageManager.Storage<LevelStorage>();
        ShareDataGlobalConfig = framework.ShareDataManager.Data<ShareDataGlobalConfig>();
        GameConfigGroup = framework.ConfigManager.SingleConfigGroup<GameConfigGroup>();

        _initialized = true;
    }

    public static void GiveItem(int itemID, int itemNum)
    {
        var ItemConfigList = GameConfigGroup.ItemConfigList;
        if (CommonStorage == null || ItemConfigList == null) return;

        switch (itemID)
        {
            case 1:
                CommonStorage.Flower += itemNum;
                break;
            case 2:
                CommonStorage.Item_Remove += itemNum;
                break;
            case 3:
                CommonStorage.Item_Recall += itemNum;
                break;
            case 4:
                CommonStorage.Item_Bloom += itemNum;
                break;
            case 5:
                CommonStorage.Item_Life += itemNum;
                break;
            default:
                Debug.LogWarning($"未知的 itemID: {itemID}");
                break;
        }
    }
}
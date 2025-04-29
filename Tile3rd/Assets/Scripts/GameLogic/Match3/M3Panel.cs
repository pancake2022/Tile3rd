using System;
using System.Collections.Generic;
using CSFramework;

public enum M3PanelType
{
    RandomSeed = 0, // 使用种子随机，相当于固定关卡
    ForceRandom = 1, // 强制随机，相当于随机关卡
}

[Serializable]
public class M3Panel
{
    public int ID; // 唯一编号
    public string Desc; // 描述
    public int RandomSeed; // 随机种子
    public int DifficultyLevel; // 难度等级，对应有多少种牌色
    public int ReferenceLevelID; // 引用的关卡ID，TODO REMOVE
    public M3PanelType PanelType; // 关卡类型
    public List<M3Layer> LayerList = new List<M3Layer>();
    [NonSerialized]
    public LVBool IsDirty;

    public string DispalyName => string.IsNullOrEmpty(Desc) ? ID.ToString() : $"[{ID}]{Desc}";

    public M3Panel () {}
    public M3Panel (M3Panel panel)
    {
        ID = panel.ID;
        Desc = panel.Desc;
        RandomSeed = panel.RandomSeed;
        DifficultyLevel = panel.DifficultyLevel;
        PanelType = panel.PanelType;
        for (var i = 0; i < panel.LayerList.Count; ++i)
            LayerList.Add(new M3Layer(panel.LayerList[i]));
    }

    public void RefreshPanelData ()
    {
        if (PanelType == M3PanelType.RandomSeed)
        {
            RandomAllCellType(RandomSeed);
        }
        else // if (PanelType == M3PanelType.ForceRandom)
        {
            RandomAllCellType(save_random_seed: false);
        }
    }

    public void RandomAllCellType (int random_seed = 0, bool save_random_seed = false)
    {
        if (random_seed == 0)
            random_seed = new System.Random().Next(0, int.MaxValue);
        
        if (save_random_seed)
            RandomSeed = random_seed;

        var random = new System.Random(random_seed);
        var all_type_set = new HashSet<int>();
        var wait_random_cell_list = new List<M3Cell>();
        foreach (var layer in LayerList)
        {
            foreach (var cell in layer.CellList)
            {
                if (cell.IsBF && cell.Type > M3Const.CellTypeRandom)
                    all_type_set.Add(cell.Type);
                else
                    wait_random_cell_list.Add(cell);
            }
        }
        var all_type_list = new List<int>(all_type_set);
        var free_type_count = DifficultyLevel - all_type_list.Count;
        if (free_type_count < 0)
        {
            free_type_count = 0;
            CSFramework.Logger.Warning($"Panel[{DispalyName}] DifficultyLevel Error, DifficultyLevel({DifficultyLevel}) < TypeCount({all_type_list.Count})");
        }
        else if (free_type_count > M3Const.CellTypeCount - all_type_list.Count)
        {
            free_type_count = M3Const.CellTypeCount - all_type_list.Count;
            CSFramework.Logger.Warning($"Panel[{DispalyName}] DifficultyLevel Error, DifficultyLevel({DifficultyLevel}) > CellTypeCount({M3Const.CellTypeCount})");
        }

        // generate free type
        if (free_type_count > 0)
        {
            for (var type_i = 1; type_i <= M3Const.CellTypeCount; ++type_i)
            {
                if (!all_type_set.Contains(type_i))
                {
                    all_type_set.Add(type_i);
                    all_type_list.Insert(0, type_i);
                    --free_type_count;

                    if (free_type_count == 0)
                        break;
                }
            }
        }

        // shuffle random cell list
        wait_random_cell_list.Shuffle(random);

        for (var i = 0; i < wait_random_cell_list.Count; ++i)
        {
            var type_index = i / M3Const.CellMatchCount;
            type_index %= all_type_list.Count;
            wait_random_cell_list[i].Type = all_type_list[type_index];
        }

        IsDirty.Value = true;
    }
}
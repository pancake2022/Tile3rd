using CSFramework;

public enum CommonTag
{
    /// <summary>
    /// 新玩家标记
    /// </summary>
    NewPlayer = 0,
}

public static class CommonTagExtension
{
    public static bool IsCommonTagDirty (this CommonStorage storage, CommonTag tag)
    {
        if (storage.CommonTagDict.TryGetValue(tag.ToString(), out var tag_value))
            return tag_value != 0;
        return false;
    }

    public static void SetCommonTagDirty (this CommonStorage storage, CommonTag tag)
    {
        var key = tag.ToString();
        if (storage.CommonTagDict.TryGetValue(key, out var tag_value) && tag_value != 0)
            return;
        storage.CommonTagDict[key] = 1;
    }

    public static bool CheckCommonTagExecute (this CommonStorage storage, CommonTag tag, System.Action callback)
    {
        if (!storage.IsCommonTagDirty(tag))
        {
            callback?.Invoke();
            storage.SetCommonTagDirty(tag);
            return true;
        }
        return false;
    }

    public static bool CheckCommonTagValue (this CommonStorage storage, CommonTag tag, int match_value)
    {
        if (storage.CommonTagDict.TryGetValue(tag.ToString(), out var tag_value))
            return tag_value == match_value;
        return false;
    }

    public static void SetCommonTagValue (this CommonStorage storage, CommonTag tag, int tag_value)
    {
        storage.CommonTagDict[tag.ToString()] = tag_value;
    }
}
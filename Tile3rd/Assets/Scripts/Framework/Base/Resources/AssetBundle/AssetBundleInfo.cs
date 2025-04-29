namespace CSFramework
{
    [System.Serializable]
    public class AssetBundleInfo
    {
        public string Name;
        public string[] DependencyNameList;
        public string HashValue;
        public uint Crc;
    }
}
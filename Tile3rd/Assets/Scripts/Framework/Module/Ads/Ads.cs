namespace CSFramework
{
    public enum AdsType
    {
        None,
        Interstitial, // 插屏广告
        RewardedVideo, // 激励视频
        Banner, // Banner
    }

    public enum AdsPlatform
    {
        None,
        Android,
        iOS,
    }

    [System.Serializable]
    public class AdsConfigItem 
    {
        public AdsPlatform AdsPlatform;
        public AdsType AdsType;
        public string Key;
    }
}
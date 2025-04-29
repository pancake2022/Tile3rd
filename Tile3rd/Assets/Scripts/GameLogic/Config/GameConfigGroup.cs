using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class GameConfigGroup : ConfigGroup
    {
        public List<GlobalConfig> GlobalConfigList; // Global 
        public List<DailyTaskChainConfig> DailyTaskChainConfigList; // DailyTaskChain 
        public List<DailyTaskConfig> DailyTaskConfigList; // DailyTask 
        public List<LevelConfig> LevelConfigList; // Level 
        public List<ShopConfig> ShopConfigList; // Shop 
        public List<SignConfig> SignConfigList; // Sign 
        public List<BundleConfig> BundleConfigList; // Bundle 
        public List<ItemConfig> ItemConfigList; // Item 
        public List<TouchPointConfig> TouchPointConfigList; // TouchPoint 
        public List<MakeOverConfig> MakeOverConfigList; // MakeOver 
        public List<StoryConfig> StoryConfigList; // Story 
        public List<QuestConfig> QuestConfigList; // Quest 
        public List<CollectionConfig> CollectionConfigList; // Collection 
        public List<LoveLevelConfig> LoveLevelConfigList; // LoveLevel  
    }
}
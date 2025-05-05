using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class DailyTaskConfig : Config
    {
        public string FindCatPrefab; // 找猫的prefab 
        public int ID; // 编号 
        public int RewardCount; // 奖励数量 
        public int RewardID; // 奖励ID 
        public int RewardType; // 奖励类型 1. 道具奖励 
        public int TaskChain; // 任务链ID 
        public int TaskCount; // 任务计数 
        public int TaskType; // 任务类型 1. 连赢任务 2. 找猫任务  
    }
}
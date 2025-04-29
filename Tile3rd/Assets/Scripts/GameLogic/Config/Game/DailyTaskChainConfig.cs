using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class DailyTaskChainConfig : Config
    {
        public List<int> ChainList; // 包含的任务 
        public int ID; // 编号 
        public int MakeOverImageID; // 奖励家具ID 
        public int StoryID; // 所属Story 
        public int UnlockCD; // 解锁CD(秒) 
        public int UnlockTouchID; // 解锁touchID  
    }
}
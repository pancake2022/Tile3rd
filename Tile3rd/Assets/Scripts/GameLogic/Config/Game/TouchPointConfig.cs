using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class TouchPointConfig : Config
    {
        public string Desc; // 说明 
        public int ID; // 编号 
        public List<int> ImageIDList; // 包含图片 
        public int PrefabID; //  
        public int StoryID; // 剧情编号 
        public int Type; // 解锁类型 1.小花解锁 2.任务解锁 3.广告解锁 
        public List<int> Unlock; // 解锁条件  
    }
}
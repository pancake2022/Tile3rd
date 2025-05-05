using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class QuestConfig : Config
    {
        public string ButtonDesc; // 猫任务开始时按钮上的文字 
        public string DescFinish; // 猫任务结束时的描述 
        public string DescStart; // 猫任务的开始时的描述 
        public int ID; // 编号 
        public int LevelID; // 关卡ID 
        public int MakeOverImageID; // 家具的图片 
        public int QuestType; // 任务完成条件 1. 关卡（显示猫） 2. 关卡（不显示猫） 3. 找茬 4. 换图 
        public int StoryID; //  
        public int UnlockCondition; // 解锁条件 touchPointID 
        public int UnlockType; // 解锁类型 1. touchPoint  
    }
}
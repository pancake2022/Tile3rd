using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class StoryConfig : Config
    {
        public string Back; // 图片 
        public string HomeBack; // home背景 
        public int ID; // 编号 
        public int ImageRewardID; // 奖励 
        public int ImageRewardNum; // 奖励数量 
        public string Name; // 故事名字 
        public string Pack; // 资源包 
        public int TouchRewardID; // 奖励 
        public int TouchRewardNum; // 奖励数量 
        public int Type; // 类型 1. 剧情故事 2. 特殊解锁  
    }
}
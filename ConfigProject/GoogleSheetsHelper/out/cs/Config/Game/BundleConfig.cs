using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class BundleConfig : Config
    {
        public string Icon; // 图标 
        public int ID; // 编号 
        public int Item1ID; // 道具ID 
        public int Item1Num; // 道具数量 
        public int Item2ID; // 道具ID 
        public int Item2Num; // 道具数量 
        public int Item3ID; // 道具ID 
        public int Item3Num; // 道具数量 
        public string Name; // 礼包名字 
        public string Pack; // 资源包 
        public int ShowStoryID; // 显示的故事 
        public int Type; // 类型 1. 故事礼包 2. 签到礼包  
    }
}
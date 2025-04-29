using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class DailyTaskChianConfig : Config
    {
        public List<int> ChainList; // 包含的任务 
        public string ChianBG; // 图片背景 
        public string ChianIcon; // 图片 
        public string ChianPack; // 图片包名 
        public int ID; // 编号 
        public int UnlockCD; // 解锁CD-秒  
    }
}
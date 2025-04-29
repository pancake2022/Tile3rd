using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class LevelConfig : Config
    {
        public int ID; // 编号 
        public int PadRect; // 位置修正 
        public float PadScale; // 缩放 
        public int PanelID; // panel编号 
        public int PanelRect; // 位置修正 
        public float PanelScale; // 缩放 
        public int Type; // 0. 普通关卡 1. 翻牌关卡  
    }
}
using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class CollectionConfig : Config
    {
        public string Back; // 图片 
        public string BackPack; // 资源包 
        public string Describe; // 描述 
        public string GameBG; // 游戏背景图 
        public string GameBGPack; // 游戏背景资源包 
        public string Icon; // 图标 
        public string IconPack; // 资源包 
        public int ID; // 编号 
        public string TilePack; // 牌的图集 
        public int Type; // 牌收集方式 1. lovelevel等级 2. 牌的收集进度 3. 签到奖励 
        public int UnlockCount; // 解锁需要收集的牌数量 
        public int UnlockLevel; // 解锁等级 
        public int UnlockTile; // 解锁条件  
    }
}
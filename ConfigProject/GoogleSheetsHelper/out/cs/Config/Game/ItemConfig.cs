using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class ItemConfig : Config
    {
        public string Icon; // 图标 
        public int ID; // 编号 
        public string Pack; // 资源包 
        public int TileID; // 牌的编号 
        public int Type; // 道具类型 1. 道具 2. tile 3. buff  
    }
}
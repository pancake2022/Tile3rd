using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class GameLevelConfig : Config
    {
        public string BgImage; // 背景图片 
        public string BgPack; // 背景图集 
        public List<int> CustomerList; // 顾客 
        public int CustomerShowType; // 顾客出现规则 
        public string Desc; // 说明 
        public int ID; // 编号 
        public int PanelID; // 初始关卡ID 
        public List<int> RefreshPanelList; // 刷新的关卡List 
        public List<int> SCustomerList; // 特殊客人 
        public int Type; // 关卡类型  
    }
}
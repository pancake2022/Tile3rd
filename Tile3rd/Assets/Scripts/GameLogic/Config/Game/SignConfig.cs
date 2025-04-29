using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class SignConfig : Config
    {
        public string Desc; // 备注 
        public string Icon; // 图标 
        public string IconPack; // 资源包 
        public int ID; // 编号 
        public int LevelCD; // 赢的关卡次数 
        public int RefreshCD; // 刷新倒计时s 
        public int Reward_Num; // 奖励数量 
        public string Style; // 样式 
        public string UIIcon; // 图标 
        public int UnlcokCD; // 解锁倒计时s  
    }
}
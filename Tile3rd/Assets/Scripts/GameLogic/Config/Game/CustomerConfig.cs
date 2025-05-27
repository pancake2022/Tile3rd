using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class CustomerConfig : Config
    {
        public int ID; // 编号 
        public string Image; // 顾客图片 
        public int Like; // 完成奖励 
        public List<int> OrderList; // 订单 
        public string Pack; // 顾客图集 
        public List<int> RewardID; // 特殊完成奖励 
        public int Type; // 顾客类型 0.初始默认出场 1. 完成前序客人 2. 特殊规则（奖励） 
        public List<int> Unlock; // 解锁顺序  
    }
}
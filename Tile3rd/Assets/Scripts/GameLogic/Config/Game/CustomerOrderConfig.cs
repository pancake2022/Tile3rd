using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class CustomerOrderConfig : Config
    {
        public string Desc; // 说明 
        public int ID; // 编号 
        public string Image; // 订单图片 
        public int Num; // 订单数量 
        public string Pack; // 订单图库 
        public int Reward; // 单个价值 
        public int Type; // 订单类型  
    }
}
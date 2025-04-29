using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class ShopConfig : Config
    {
        public string AppID; // 苹果商店的id 
        public int BuyLimit; // 1.限购1次 2.不限购 
        public string GooglePlayID; // google商店的id 
        public string IconPath; // icon的路径 
        public int ID; // 编号 
        public string Name; // 名称 
        public float Price; // 价格（美元） 
        public int Type; // 1.消耗品 2.非消耗品 3.订阅  
    }
}
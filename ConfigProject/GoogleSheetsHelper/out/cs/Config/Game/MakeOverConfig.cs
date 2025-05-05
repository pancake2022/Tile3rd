using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class MakeOverConfig : Config
    {
        public int BuyPrice; // 价格 
        public int BuyType; // 获得类型 0. 初始 1. 金币 2. 广告 3. 猫任务 4. 每日任务 99.弃用 
        public string CatAnim; // 猫出现动画 
        public string CatDefaultAnim; // 猫的待机动画 
        public int CatID; // 猫的图片 
        public string CatPrefab; //  
        public bool CatSelectHide; // 选择的时候隐藏猫 
        public string Desc; // 说明 
        public List<int> EditPoint; // 装修显示的位置 
        public float EditScale; // 装修显示的缩放 
        public List<int> HideIDList; // 隐藏图片 
        public string Icon; // 图标 
        public int ID; // 编号 
        public bool ImageCount; // 奖励计数 
        public string ImagePath; // 图片路径 
        public string ImagePrefab; //  
        public int ImageType; // 1.普通 2.信件 
        public bool isUnlock; // 当前使用 
        public bool isUse; // 当前使用 
        public int LoveExp; // 好感度经验值 
        public string MakeOverAnim; // 装修动画名 
        public string MakeOverDefaultAnim; // 默认动画名 
        public string Pack; // 资源包 
        public int SecondPrice; // 第二价格 
        public int SelectHideID; // select时隐藏 
        public List<int> ShowIDList; // 显示图片 
        public int StoryID; //  
        public int TouchShowID; // touchtype4时显示的图片ID 
        public int TouchType; // 0.不可点 1.可点击 2. 点击后弹信件 3. 点击后弹提示 4. 点击后显示图片  
    }
}
using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class GlobalConfig : Config
    {
        public int M3_MaxTypeCount; // 最大花色数量 
        public int Version; // 版本 
        public int Story_Num_Last; // 上个版本的故事数量 
        public int Flower_Initial; // 鲜花的初始数量 
        public int Level_Initial; // 关卡的初始等级为1 
        public int Level_Loop_Min; // 关卡循环的最小等级 
        public int Level_Loop_Max; // 关卡循环的最大等级 
        public int Item_Remove_Initial; // 道具【消除】初始数量 
        public int Item_Recall_Initial; // 道具【回退】初始数量 
        public int Item_Bloom_Initial; // 道具【小花】初始数量 
        public int Item_Life_Initial; // 道具【复活】初始数量 
        public int Item_Recall_UnlockLevel; // 道具【回退】解锁关卡（达到） 
        public int Item_Remove_UnlockLevel; // 道具【消除】解锁关卡（达到） 
        public int Item_Bloom_UnlockLevel; // 道具【绽放】解锁关卡（任务） 
        public int Unlock_CatQuest; // 解锁 - 猫任务 
        public int Unlock_DailyTask; // 解锁 - DailyTask（任务） 
        public int Unlock_Revive_BloomBuff; // 解锁 - 复活给bloombuff 
        public int Unlock_NewBundle; // 解锁 - 新手礼包 
        public int Unlock_LevelChest; // 解锁 - 关卡宝箱 - 任务解锁 
        public int Unlock_Collection; // 解锁 - 牌收集 
        public int Unlock_StoryIcon; // 解锁 - storyIcon - 家具解锁 
        public int Unlock_BloomBundle; // 解锁 - 绽放礼包 
        public int Unlock_Sign; // 解锁 - 7日签到奖励 
        public int Unlock_GoogleReview; // 解锁 - googlereview等级 
        public int Unlock_Shop; // 解锁 - 付费礼包 
        public int RV_Reward_Remove; // 广告获得的【消除】数量 
        public int RV_Reward_Recall; // 广告获得的【回退】数量 
        public int RV_Reward_Bloom; // 广告获得的【绽放】数量 
        public int RV_Reward_Life; // 广告获得的【复活】数量 
        public int Bloom_Times_Match; // 绽放次数 - 消除 
        public int Bloom_Times_Life; // 绽放次数 - 复活 
        public int Bloom_Times_Item; // 绽放次数 - 道具 
        public int Bloom_Bunlde_BloomTimes; // 绽放礼包 - 绽放次数 
        public int Flower_Bloom_Rate; // 基础绽放几率 
        public int Flower_Bloom_Buff_Rate; // Buff绽放几率 
        public int Flower_Bloom_Min; // 绽放状态下获得花次数 - 最小值 
        public int Flower_Bloom_Max; // 绽放状态下获得花次数 - 最大值 
        public int Flower_Bloom_Normal_Min; // 关卡基础花 - 最小值 
        public int Flower_Bloom_Normal_Max; // 关卡基础花 - 最大值 
        public int Flower_Bloom_Buff_Min; // 绽放buff状态下获得花次数 - 最小值 
        public int Flower_Bloom_Buff_Max; // 绽放buff状态下获得花次数 - 最大值 
        public int Interstitial_UnlockLevel; // 插屏广告 - 解锁等级 
        public int Interstitial_CD_Initial; // 插屏广告CD间隔 - 初始值 
        public int Interstitial_CD_Level; // 插屏广告CD间隔 - 关卡 
        public int RandomTile_Count_Min; // 猫翻牌玩法 - 最小值 
        public int RandomTile_Count_Max; // 猫翻牌玩法 - 最大值 
        public int RandomTile_Count_Rate; // 猫翻牌玩法 - 翻牌几率 
        public int Shop_CD; // 商店刷新倒计时（秒）  
    }
}
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CSFramework
{
    [Serializable]
    public class Tile2Storage : Storage
    {
        // 版本更新
        [JsonProperty]
        int _version = 0;
        [JsonIgnore]
        public int Version
        {
            get
            {
                return _version;
            }
            set
            {
                if (_version != value)
                {
                    _version = value;
                    dirty(true);
                }
            }
        }

        // levelchest的状态
        [JsonProperty]
        int _levelchest_process = 0;
        [JsonIgnore]
        public int LevelChest_Process
        {
            get
            {
                return _levelchest_process;
            }
            set
            {
                if (_levelchest_process != value)
                {
                    _levelchest_process = value;
                    dirty(true);
                }
            }
        }
        // levelchest的物品list
        [JsonProperty]
        StorageList<int> _levelchest_item_list = new StorageList<int>();

        [JsonIgnore]
        public StorageList<int> LevelChestItemList
        {
            get
            {
                return _levelchest_item_list;
            }
            set
            {
                if (_levelchest_item_list != value)
                {
                    _levelchest_item_list = value;
                    dirty(true);
                }
            }
        }

        // lovelevel的等级
        [JsonProperty]
        int _lovelevel_level = 0;
        [JsonIgnore]
        public int LoveLevelLevel
        {
            get
            {
                return _lovelevel_level;
            }
            set
            {
                if (_lovelevel_level != value)
                {
                    _lovelevel_level = value;
                    dirty(true);
                }
            }
        }
        // lovelevel的exp进度
        [JsonProperty]
        int _lovelevel_exp = 0;
        [JsonIgnore]
        public int LoveLevelExp
        {
            get
            {
                return _lovelevel_exp;
            }
            set
            {
                if (_lovelevel_exp != value)
                {
                    _lovelevel_exp = value;
                    dirty(true);
                }
            }
        }
        // lovelevel的exp成长值
        [JsonProperty]
        int _lovelevel_expUp = 0;
        [JsonIgnore]
        public int LoveLevelExpUp
        {
            get
            {
                return _lovelevel_expUp;
            }
            set
            {
                if (_lovelevel_expUp != value)
                {
                    _lovelevel_expUp = value;
                    dirty(true);
                }
            }
        }

        // 换牌
        [JsonProperty]
        int _current_tile_id = 1;
        [JsonIgnore]
        public int CurrentTileID
        {
            get
            {
                return _current_tile_id;
            }
            set
            {
                if (_current_tile_id != value)
                {
                    _current_tile_id = value;
                    dirty(true);
                }
            }
        }
        //套牌的解锁状态
        [JsonProperty]
        StorageDictionary<int, bool> _tileunlock = new StorageDictionary<int, bool>();
        [JsonIgnore]
        public StorageDictionary<int, bool> TileUnlock
        {
            get
            {
                return _tileunlock;
            }
        }
        //套牌单张牌的解锁状态
        [JsonProperty]
        StorageDictionary<int, bool> _tile_single_unlock = new StorageDictionary<int, bool>();
        [JsonIgnore]
        public StorageDictionary<int, bool> TileSingleUnlock
        {
            get
            {
                return _tile_single_unlock;
            }
        }

        //故事礼包
        [JsonProperty]
        StorageDictionary<int, bool> _bundle_rv = new StorageDictionary<int, bool>();
        [JsonIgnore]
        public StorageDictionary<int, bool> BundleRV
        {
            get
            {
                return _bundle_rv;
            }
        }
        //绽放礼包的bloombuff
        [JsonProperty]
        int _bloom_buff_times = 0;
        [JsonIgnore]
        public int BloomBuffTimes
        {
            get
            {
                return _bloom_buff_times;
            }
            set
            {
                if (_bloom_buff_times != value)
                {
                    _bloom_buff_times = value;
                    dirty(true);
                }
            }
        }
        // 绽放礼包的首次免费
        [JsonProperty]
        bool _bloom_buff_first;
        [JsonIgnore]
        public bool BloomBuffFirst
        {
            get
            {
                return _bloom_buff_first;
            }
            set
            {
                if (_bloom_buff_first != value)
                {
                    _bloom_buff_first = value;
                    dirty(true);
                }
            }
        }
        // 每日任务链dailytaskchian的当前ID
        [JsonProperty]
        int _current_dailytaskchain_id = 0;
        [JsonIgnore]
        public int CurrentDailyTaskChainID
        {
            get
            {
                return _current_dailytaskchain_id;
            }
            set
            {
                if (_current_dailytaskchain_id != value)
                {
                    _current_dailytaskchain_id = value;
                    dirty(true);
                }
            }
        }
        // 每日任务链dailytaskchian的状态/0未解锁/1已解锁/2已完成未领奖/3已领奖
        [JsonProperty]
        StorageDictionary<int, int> _dailytaskchain_condition = new StorageDictionary<int, int>();
        [JsonIgnore]
        public StorageDictionary<int, int> DailyTaskChainCondition
        {
            get
            {
                return _dailytaskchain_condition;
            }
        }
        
        // 每日任务链dailytaskchian的记录下线时间
        [JsonProperty]
        DateTime _dailytaskchain_starttime;
        [JsonIgnore]
        public DateTime DailyTaskChainStartTime
        {
            get
            {
                return _dailytaskchain_starttime;
            }
            set
            {
                if (_dailytaskchain_starttime != value)
                {
                    _dailytaskchain_starttime = value;
                    dirty(true);
                }
            }
        }
        // 每日任务dailytask的当前ID
        [JsonProperty]
        int _current_dailytask_id = 0;
        [JsonIgnore]
        public int CurrentDailyTaskID
        {
            get
            {
                return _current_dailytask_id;
            }
            set
            {
                if (_current_dailytask_id != value)
                {
                    _current_dailytask_id = value;
                    dirty(true);
                }
            }
        }
        // 每日任务dailytask的状态/0未解锁/1已解锁/2已完成未领奖/3已领奖
        [JsonProperty]
        StorageDictionary<int, int> _dailytask_condition = new StorageDictionary<int, int>();
        [JsonIgnore]
        public StorageDictionary<int, int> DailyTaskCondition
        {
            get
            {
                return _dailytask_condition;
            }
        }
        // 连赢计数
        [JsonProperty]
        int _winstreak_count = 0;
        [JsonIgnore]
        public int WinStreakCount
        {
            get
            {
                return _winstreak_count;
            }
            set
            {
                if (_winstreak_count != value)
                {
                    _winstreak_count = value;
                    dirty(true);
                }
            }
        }
        // 判断连赢期间，退出游戏会清除连赢进度
        [JsonProperty]
        bool _winstreak_offgame = false;
        [JsonIgnore]
        public bool WinStreakOffGame
        {
            get
            {
                return _winstreak_offgame;
            }
            set
            {
                if (_winstreak_offgame != value)
                {
                    _winstreak_offgame = value;
                    dirty(true);
                }
            }
        }
        //找猫计数
        [JsonProperty]
        StorageDictionary<int, bool> _findcat_condition = new StorageDictionary<int, bool>();
        [JsonIgnore]
        public StorageDictionary<int, bool> FindCatCondition
        {
            get
            {
                return _findcat_condition;
            }
        }
        //找猫hint状态（0:初始/1:显示hint/2:hint完成）
        [JsonProperty]
        StorageDictionary<int, int> _findcathint_condition = new StorageDictionary<int, int>();
        [JsonIgnore]
        public StorageDictionary<int, int> FindCatHintCondition
        {
            get
            {
                return _findcathint_condition;
            }
        }
        //签到计数
        [JsonProperty]
        int _sign_count = 0;
        [JsonIgnore]
        public int SignCount
        {
            get
            {
                return _sign_count;
            }
            set
            {
                if (_sign_count != value)
                {
                    _sign_count = value;
                    dirty(true);
                }
            }
        }
        //签到状态
        [JsonProperty]
        StorageDictionary<int, int> _sign_condition = new StorageDictionary<int, int>();
        [JsonIgnore]
        public StorageDictionary<int, int> SignCondition
        {
            get
            {
                return _sign_condition;
            }
        }
        //签到倒计时
        [JsonProperty]
        StorageDictionary<int, DateTime> _sign_cd = new StorageDictionary<int, DateTime>();
        [JsonIgnore]
        public StorageDictionary<int, DateTime> SignCD
        {
            get
            {
                return _sign_cd;
            }
        }
        //签到的解锁
        [JsonProperty]
        bool _is_sign_unlock;
        [JsonIgnore]
        public bool IsSignUnlock
        {
            get
            {
                return _is_sign_unlock;
            }
            set
            {
                if (_is_sign_unlock != value)
                {
                    _is_sign_unlock = value;
                    dirty(true);
                }
            }
        }
        [JsonProperty]
        StorageDictionary<int, int> _sign_level_cd = new StorageDictionary<int, int>();
        [JsonIgnore]
        public StorageDictionary<int, int> SignLevelCD
        {
            get
            {
                return _sign_level_cd;
            }
        }
        //签到给的的bloomall
        [JsonProperty]
        int _bloom_all_times = 0;
        [JsonIgnore]
        public int BloomAllTimes
        {
            get
            {
                return _bloom_all_times;
            }
            set
            {
                if (_bloom_all_times != value)
                {
                    _bloom_all_times = value;
                    dirty(true);
                }
            }
        }
        // 去广告礼包是否购买
        [JsonProperty]
        bool _is_noADS;
        [JsonIgnore]
        public bool isnoADS
        {
            get
            {
                return _is_noADS;
            }
            set
            {
                if (_is_noADS != value)
                {
                    _is_noADS = value;
                    dirty(true);
                }
            }
        }
        // 商店礼包的刷新
        [JsonProperty]
        DateTime _shop_refreshCD;
        [JsonIgnore]
        public DateTime ShopRefreshCD
        {
            get
            {
                return _shop_refreshCD;
            }
            set
            {
                if (_shop_refreshCD != value)
                {
                    _shop_refreshCD = value;
                    dirty(true);
                }
            }
        }
        // 商店礼包是否首次解锁 - 用来判断CD开始时间
        [JsonProperty]
        bool _is_shop_unlock;
        [JsonIgnore]
        public bool isShopUnlock
        {
            get
            {
                return _is_shop_unlock;
            }
            set
            {
                if (_is_shop_unlock != value)
                {
                    _is_shop_unlock = value;
                    dirty(true);
                }
            }
        }
        // 商店当前礼包
        [JsonProperty]
        ShopConfig _current_shop;
        [JsonIgnore]
        public ShopConfig CurrentShop
        {
            get
            {
                return _current_shop;
            }
            set
            {
                if (_current_shop != value)
                {
                    _current_shop = value;
                    dirty(true);
                }
            }
        }
    }
}
using System;
using Newtonsoft.Json;

namespace CSFramework
{
    [Serializable]
    public class LevelStorage : Storage
    {

        // 当前关卡实际
        [JsonProperty]
        int _currentlevel = 0;
        [JsonIgnore]
        public int CurrentLevel
        {
            get
            {
                return _currentlevel;
            }
            set
            {
                if (_currentlevel != value)
                {
                    _currentlevel = value;
                    dirty(true);
                }
            }
        }

        // 当前关卡显示
        [JsonProperty]
        int _levelcount = 1;
        [JsonIgnore]
        public int LevelCount
        {
            get
            {
                return _levelcount;
            }
            set
            {
                if (_levelcount != value)
                {
                    _levelcount = value;
                    dirty(true);
                }
            }
        }

        // 当前panel
        [JsonProperty]
        M3Panel _current_panel;
        [JsonIgnore]
        public M3Panel CurrentPanel
        {
            get
            {
                return _current_panel;
            }
            set
            {
                if (_current_panel != value)
                {
                    _current_panel = value;
                    dirty(true);
                }
            }
        }

        // 当前GameLevel
        [JsonProperty]
        GameLevelConfig _current_gamelevel;
        [JsonIgnore]
        public GameLevelConfig Current_GameLevel
        {
            get
            {
                return _current_gamelevel;
            }
            set
            {
                if (_current_gamelevel != value)
                {
                    _current_gamelevel = value;
                    dirty(true);
                }
            }
        }

        // 当前GameLevel
        [JsonProperty]
        StorageDictionary<int, bool> _gamelevel_condition = new StorageDictionary<int, bool>();
        [JsonIgnore]
        public StorageDictionary<int, bool> GameLevel_Condition
        {
            get
            {
                return _gamelevel_condition;
            }
        }
        // Customer状态
        //[JsonProperty]
        //StorageDictionary<int, int> _customer_condition = new StorageDictionary<int, int>();
        //[JsonIgnore]
        //public StorageDictionary<int, int> Customer_Condition
        //{
        //    get
        //    {
        //        return _customer_condition;
        //    }
        //}

        // Order状态
        //[JsonProperty]
        //StorageDictionary<int, int> _order_condition = new StorageDictionary<int, int>();
        //[JsonIgnore]
        //public StorageDictionary<int, int> Order_Condition
        //{
        //    get
        //    {
        //        return _order_condition;
        //    }
        //}
    }
}
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
    }
}
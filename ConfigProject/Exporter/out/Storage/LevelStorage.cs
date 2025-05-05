using System;
using Newtonsoft.Json;

namespace CSFramework
{
    [Serializable]
    public class LevelStorage : Storage
    {
        
        // 当前关卡
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
       
       
    }
}
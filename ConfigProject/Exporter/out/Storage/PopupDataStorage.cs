using System;
using Newtonsoft.Json;

namespace CSFramework
{
    [Serializable]
    public class PopupDataStorage : Storage
    {
        
        // 状态
        [JsonProperty]
        int _state = 0;
        [JsonIgnore]
        public int State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    dirty();
                }
            }
        }
       
        // 上次弹出的时间戳
        [JsonProperty]
        ulong _lastpopuptimestamp = 0;
        [JsonIgnore]
        public ulong LastPopupTimestamp
        {
            get
            {
                return _lastpopuptimestamp;
            }
            set
            {
                if (_lastpopuptimestamp != value)
                {
                    _lastpopuptimestamp = value;
                    dirty();
                }
            }
        }
       
        // 弹出次数
        [JsonProperty]
        int _popuptimes = 0;
        [JsonIgnore]
        public int PopupTimes
        {
            get
            {
                return _popuptimes;
            }
            set
            {
                if (_popuptimes != value)
                {
                    _popuptimes = value;
                    dirty();
                }
            }
        }
       
       
    }
}
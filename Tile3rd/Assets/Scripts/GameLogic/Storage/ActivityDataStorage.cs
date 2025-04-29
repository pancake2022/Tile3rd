using System;
using Newtonsoft.Json;

namespace CSFramework
{
    [Serializable]
    public class ActivityDataStorage : Storage
    {
        
        // 开始时间戳
        [JsonProperty]
        ulong _starttimestamp = 0;
        [JsonIgnore]
        public ulong StartTimestamp
        {
            get
            {
                return _starttimestamp;
            }
            set
            {
                if (_starttimestamp != value)
                {
                    _starttimestamp = value;
                    dirty();
                }
            }
        }
       
        // 结束时间戳
        [JsonProperty]
        ulong _endtimestamp = 0;
        [JsonIgnore]
        public ulong EndTimestamp
        {
            get
            {
                return _endtimestamp;
            }
            set
            {
                if (_endtimestamp != value)
                {
                    _endtimestamp = value;
                    dirty();
                }
            }
        }
       
        // 状态
        [JsonProperty]
        int _status = 0;
        [JsonIgnore]
        public int Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    dirty();
                }
            }
        }
       
        // 上一次弹出的时间戳
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
       
        // 开启次数
        [JsonProperty]
        int _opentimes = 0;
        [JsonIgnore]
        public int OpenTimes
        {
            get
            {
                return _opentimes;
            }
            set
            {
                if (_opentimes != value)
                {
                    _opentimes = value;
                    dirty();
                }
            }
        }
       
        // 完成次数
        [JsonProperty]
        int _completedtimes = 0;
        [JsonIgnore]
        public int CompletedTimes
        {
            get
            {
                return _completedtimes;
            }
            set
            {
                if (_completedtimes != value)
                {
                    _completedtimes = value;
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
       
        // 进度值
        [JsonProperty]
        int _progress = 0;
        [JsonIgnore]
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    dirty();
                }
            }
        }
       
        // 动画进度值
        [JsonProperty]
        int _animateprogress = 0;
        [JsonIgnore]
        public int AnimateProgress
        {
            get
            {
                return _animateprogress;
            }
            set
            {
                if (_animateprogress != value)
                {
                    _animateprogress = value;
                    dirty();
                }
            }
        }
       
        // 奖励数据存储（通常在活动开启的时候生成）
        [JsonProperty]
        StorageList<int> _simplerewarddatalist = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> SimpleRewardDataList
        {
            get
            {
                return _simplerewarddatalist;
            }
        }
       
        // 子进度字典
        [JsonProperty]
        StorageDictionary<int, int> _subprogressdict = new StorageDictionary<int, int>();
        [JsonIgnore]
        public StorageDictionary<int, int> SubProgressDict
        {
            get
            {
                return _subprogressdict;
            }
        }
       
        // 子状态字典
        [JsonProperty]
        StorageDictionary<int, int> _substatusdict = new StorageDictionary<int, int>();
        [JsonIgnore]
        public StorageDictionary<int, int> SubStatusDict
        {
            get
            {
                return _substatusdict;
            }
        }
       
       
    }
}
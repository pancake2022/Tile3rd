using System;
using Newtonsoft.Json;

namespace CSFramework
{
    [Serializable]
    public class ActivityGroupDataStorage : Storage
    {
        
        // 活动数据字典
        [JsonProperty]
        StorageDictionary<int, ActivityDataStorage> _activitydatadict = new StorageDictionary<int, ActivityDataStorage>();
        [JsonIgnore]
        public StorageDictionary<int, ActivityDataStorage> ActivityDataDict
        {
            get
            {
                return _activitydatadict;
            }
        }
       
       
    }
}
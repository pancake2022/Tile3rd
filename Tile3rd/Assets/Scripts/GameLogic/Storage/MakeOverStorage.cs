using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CSFramework
{
    [Serializable]
    public class MakeOverStorage : Storage
    {
        // 是否使用
        [JsonProperty]
        StorageDictionary<int, bool> _imageuse = new StorageDictionary<int, bool>();
        [JsonIgnore]
        public StorageDictionary<int, bool> ImageUse
        {
            get
            {
                return _imageuse;
            }
        }
        // 是否解锁
        [JsonProperty]
        StorageDictionary<int, bool> _imageunlock = new StorageDictionary<int, bool>();
        [JsonIgnore]
        public StorageDictionary<int, bool> ImageUnlock
        {
            get
            {
                return _imageunlock;
            }
        }

        // TouchPoint解锁
        [JsonProperty]
        StorageDictionary<int, int> _touchpointcondition = new StorageDictionary<int, int>();
        [JsonIgnore]
        public StorageDictionary<int, int> TouchPointCondition
        {
            get
            {
                return _touchpointcondition;
            }
        }


        // 每个story的猫的唯一ID
        [JsonProperty]
        StorageDictionary<int, int> _current_catID = new StorageDictionary<int, int>();
        [JsonIgnore]
        public StorageDictionary<int, int> CurrentCatID
        {
            get
            {
                return _current_catID;
            }
        }
        // 当前story的编号
        [JsonProperty]
        int _current_storyid = 1;
        [JsonIgnore]
        public int CurrentStoryID
        {
            get
            {
                return _current_storyid;
            }
            set
            {
                if (_current_storyid != value)
                {
                    _current_storyid = value;
                    dirty(true);
                }
            }
        }
        // 已解锁的最大story
        [JsonProperty]
        int _unlockmax_storyid = 1;
        [JsonIgnore]
        public int UnlockMaxStoryID
        {
            get
            {
                return _unlockmax_storyid;
            }
            set
            {
                if (_unlockmax_storyid != value)
                {
                    _unlockmax_storyid = value;
                    dirty(true);
                }
            }
        }
        // story的状态/0未解锁/1已解锁/2touch（未领奖）/3touch（已领奖）image（未领奖）/4image完成（已领奖）
        [JsonProperty]
        StorageDictionary<int, int> _storycondition = new StorageDictionary<int, int>();
        [JsonIgnore]
        public StorageDictionary<int, int> StoryCondition
        {
            get
            {
                return _storycondition;
            }
        }
        // 每个story的引导
        [JsonProperty]
        StorageDictionary<int, int> _story_guide = new StorageDictionary<int, int>();//
        [JsonIgnore]
        public StorageDictionary<int, int> StoryGuide
        {
            get
            {
                return _story_guide;
            }
        }
        // catquest的id
        [JsonProperty]
        QuestConfig _current_quest;
        [JsonIgnore]
        public QuestConfig CurrentQuest
        {
            get
            {
                return _current_quest;
            }
            set
            {
                if (_current_quest != value)
                {
                    _current_quest = value;
                    dirty(true);
                }
            }
        }
        // catquest的状态/0未解锁/1已解锁/2已完成
        [JsonProperty]
        StorageDictionary<int, int> _catquest_condition = new StorageDictionary<int, int>();
        [JsonIgnore]
        public StorageDictionary<int, int> CatQuestCondition
        {
            get
            {
                return _catquest_condition;
            }
        }
    }
    
}
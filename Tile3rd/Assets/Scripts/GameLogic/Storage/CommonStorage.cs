using System;
using Newtonsoft.Json;

namespace CSFramework
{
    [Serializable]
    public class CommonStorage : Storage
    {
        
        // 唯一编号
        [JsonProperty]
        string _uuid = null;
        [JsonIgnore]
        public string UUID
        {
            get
            {
                return _uuid;
            }
            set
            {
                if (_uuid != value)
                {
                    _uuid = value;
                    dirty(true);
                }
            }
        }
       
        // 玩家ID
        [JsonProperty]
        ulong _playerid = 0;
        [JsonIgnore]
        public ulong PlayerID
        {
            get
            {
                return _playerid;
            }
            set
            {
                if (_playerid != value)
                {
                    _playerid = value;
                    dirty(true);
                }
            }
        }
       
        // 设备ID
        [JsonProperty]
        string _deviceid = null;
        [JsonIgnore]
        public string DeviceID
        {
            get
            {
                return _deviceid;
            }
            set
            {
                if (_deviceid != value)
                {
                    _deviceid = value;
                    dirty(true);
                }
            }
        }
       
        // 邮箱
        [JsonProperty]
        string _email = null;
        [JsonIgnore]
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    dirty(true);
                }
            }
        }
       
        // FBID
        [JsonProperty]
        string _facebookid = null;
        [JsonIgnore]
        public string FacebookID
        {
            get
            {
                return _facebookid;
            }
            set
            {
                if (_facebookid != value)
                {
                    _facebookid = value;
                    dirty(true);
                }
            }
        }
       
        // FBName
        [JsonProperty]
        string _facebookname = null;
        [JsonIgnore]
        public string FacebookName
        {
            get
            {
                return _facebookname;
            }
            set
            {
                if (_facebookname != value)
                {
                    _facebookname = value;
                    dirty();
                }
            }
        }
       
        // FBEmail
        [JsonProperty]
        string _facebookemail = null;
        [JsonIgnore]
        public string FacebookEmail
        {
            get
            {
                return _facebookemail;
            }
            set
            {
                if (_facebookemail != value)
                {
                    _facebookemail = value;
                    dirty();
                }
            }
        }
       
        // 名称
        [JsonProperty]
        string _nickname = null;
        [JsonIgnore]
        public string Nickname
        {
            get
            {
                return _nickname;
            }
            set
            {
                if (_nickname != value)
                {
                    _nickname = value;
                    dirty();
                }
            }
        }
       
        // 付费次数
        [JsonProperty]
        uint _revenuetimes = 0;
        [JsonIgnore]
        public uint RevenueTimes
        {
            get
            {
                return _revenuetimes;
            }
            set
            {
                if (_revenuetimes != value)
                {
                    _revenuetimes = value;
                    dirty(true);
                }
            }
        }
       
        // 付费额
        [JsonProperty]
        ulong _revenueusdcents = 0;
        [JsonIgnore]
        public ulong RevenueUSDCents
        {
            get
            {
                return _revenueusdcents;
            }
            set
            {
                if (_revenueusdcents != value)
                {
                    _revenueusdcents = value;
                    dirty(true);
                }
            }
        }
       
        // 上次付费的时间戳
        [JsonProperty]
        ulong _lastrevenuetimestamp = 0;
        [JsonIgnore]
        public ulong LastRevenueTimestamp
        {
            get
            {
                return _lastrevenuetimestamp;
            }
            set
            {
                if (_lastrevenuetimestamp != value)
                {
                    _lastrevenuetimestamp = value;
                    dirty(true);
                }
            }
        }
       
        // 安装时间戳
        [JsonProperty]
        ulong _installtimestamp = 0;
        [JsonIgnore]
        public ulong InstallTimestamp
        {
            get
            {
                return _installtimestamp;
            }
            set
            {
                if (_installtimestamp != value)
                {
                    _installtimestamp = value;
                    dirty();
                }
            }
        }
       
        // 保存时间戳
        [JsonProperty]
        ulong _savetimestamp = 0;
        [JsonIgnore]
        public ulong SaveTimestamp
        {
            get
            {
                return _savetimestamp;
            }
            set
            {
                if (_savetimestamp != value)
                {
                    _savetimestamp = value;
                    dirty();
                }
            }
        }
       
        // AdjustID
        [JsonProperty]
        string _adjustid = null;
        [JsonIgnore]
        public string AdjustID
        {
            get
            {
                return _adjustid;
            }
            set
            {
                if (_adjustid != value)
                {
                    _adjustid = value;
                    dirty(true);
                }
            }
        }
       
        // ADID
        [JsonProperty]
        string _adid = null;
        [JsonIgnore]
        public string ADID
        {
            get
            {
                return _adid;
            }
            set
            {
                if (_adid != value)
                {
                    _adid = value;
                    dirty();
                }
            }
        }
       
        // Idfa
        [JsonProperty]
        string _idfa = null;
        [JsonIgnore]
        public string Idfa
        {
            get
            {
                return _idfa;
            }
            set
            {
                if (_idfa != value)
                {
                    _idfa = value;
                    dirty();
                }
            }
        }
       
        // Idfv
        [JsonProperty]
        string _idfv = null;
        [JsonIgnore]
        public string Idfv
        {
            get
            {
                return _idfv;
            }
            set
            {
                if (_idfv != value)
                {
                    _idfv = value;
                    dirty();
                }
            }
        }
       
        // Gaid
        [JsonProperty]
        string _gaid = null;
        [JsonIgnore]
        public string Gaid
        {
            get
            {
                return _gaid;
            }
            set
            {
                if (_gaid != value)
                {
                    _gaid = value;
                    dirty();
                }
            }
        }
       
        // Platform
        [JsonProperty]
        int _platform = 0;
        [JsonIgnore]
        public int Platform
        {
            get
            {
                return _platform;
            }
            set
            {
                if (_platform != value)
                {
                    _platform = value;
                    dirty();
                }
            }
        }
       
        // 
        [JsonProperty]
        string _locale = null;
        [JsonIgnore]
        public string Locale
        {
            get
            {
                return _locale;
            }
            set
            {
                if (_locale != value)
                {
                    _locale = value;
                    dirty();
                }
            }
        }
       
        // 
        [JsonProperty]
        string _country = null;
        [JsonIgnore]
        public string Country
        {
            get
            {
                return _country;
            }
            set
            {
                if (_country != value)
                {
                    _country = value;
                    dirty();
                }
            }
        }
       
        // 
        [JsonProperty]
        string _region = null;
        [JsonIgnore]
        public string Region
        {
            get
            {
                return _region;
            }
            set
            {
                if (_region != value)
                {
                    _region = value;
                    dirty();
                }
            }
        }
       
        // 
        [JsonProperty]
        string _timezone = null;
        [JsonIgnore]
        public string TimeZone
        {
            get
            {
                return _timezone;
            }
            set
            {
                if (_timezone != value)
                {
                    _timezone = value;
                    dirty();
                }
            }
        }
       
        // 
        [JsonProperty]
        string _resversion = null;
        [JsonIgnore]
        public string ResVersion
        {
            get
            {
                return _resversion;
            }
            set
            {
                if (_resversion != value)
                {
                    _resversion = value;
                    dirty();
                }
            }
        }
       
        // 
        [JsonProperty]
        string _nativeversion = null;
        [JsonIgnore]
        public string NativeVersion
        {
            get
            {
                return _nativeversion;
            }
            set
            {
                if (_nativeversion != value)
                {
                    _nativeversion = value;
                    dirty();
                }
            }
        }
       
        // 
        [JsonProperty]
        string _invitecode = null;
        [JsonIgnore]
        public string InviteCode
        {
            get
            {
                return _invitecode;
            }
            set
            {
                if (_invitecode != value)
                {
                    _invitecode = value;
                    dirty();
                }
            }
        }
       
        // 
        [JsonProperty]
        string _firebaseinstanceid = null;
        [JsonIgnore]
        public string FirebaseInstanceID
        {
            get
            {
                return _firebaseinstanceid;
            }
            set
            {
                if (_firebaseinstanceid != value)
                {
                    _firebaseinstanceid = value;
                    dirty();
                }
            }
        }
       
        // 
        [JsonProperty]
        string _appleaccountid = null;
        [JsonIgnore]
        public string AppleAccountID
        {
            get
            {
                return _appleaccountid;
            }
            set
            {
                if (_appleaccountid != value)
                {
                    _appleaccountid = value;
                    dirty();
                }
            }
        }
       
        // 公用的标记字典
        [JsonProperty]
        StorageDictionary<string, int> _commontagdict = new StorageDictionary<string, int>();
        [JsonIgnore]
        public StorageDictionary<string, int> CommonTagDict
        {
            get
            {
                return _commontagdict;
            }
        }
       
        // 公用的弹出字典
        [JsonProperty]
        StorageDictionary<string, PopupDataStorage> _commonpopupdatadict = new StorageDictionary<string, PopupDataStorage>();
        [JsonIgnore]
        public StorageDictionary<string, PopupDataStorage> CommonPopupDataDict
        {
            get
            {
                return _commonpopupdatadict;
            }
        }
       
        // 活动集合数据字典
        [JsonProperty]
        StorageDictionary<string, ActivityGroupDataStorage> _activitygroupdatadict = new StorageDictionary<string, ActivityGroupDataStorage>();
        [JsonIgnore]
        public StorageDictionary<string, ActivityGroupDataStorage> ActivityGroupDataDict
        {
            get
            {
                return _activitygroupdatadict;
            }
        }
       
        // 是否开启声音
        [JsonProperty]
        bool _soundopen = true;
        [JsonIgnore]
        public bool SoundOpen
        {
            get
            {
                return _soundopen;
            }
            set
            {
                if (_soundopen != value)
                {
                    _soundopen = value;
                    dirty();
                }
            }
        }
       
        // 是否开启音乐
        [JsonProperty]
        bool _musicopen = true;
        [JsonIgnore]
        public bool MusicOpen
        {
            get
            {
                return _musicopen;
            }
            set
            {
                if (_musicopen != value)
                {
                    _musicopen = value;
                    dirty();
                }
            }
        }
       
        // AB测试参数字典
        [JsonProperty]
        StorageDictionary<string, string> _abtestparamdict = new StorageDictionary<string, string>();
        [JsonIgnore]
        public StorageDictionary<string, string> ABTestParamDict
        {
            get
            {
                return _abtestparamdict;
            }
        }

        // 金币
        [JsonProperty]
        int _coin = 0;
        [JsonIgnore]
        public int Coin
        {
            get
            {
                return _coin;
            }
            set
            {
                if (_coin != value)
                {
                    _coin = value;
                    dirty(true);
                }
            }
        }

        // 小花
        [JsonProperty]
        int _flower = 0;
        [JsonIgnore]
        public int Flower
        {
            get
            {
                return _flower;
            }
            set
            {
                if (_flower != value)
                {
                    _flower = value;
                    dirty(true);
                }
            }
        }

        // 道具【回退】
        [JsonProperty]
        int _item_recall = 0;
        [JsonIgnore]
        public int Item_Recall
        {
            get
            {
                return _item_recall;
            }
            set
            {
                if (_item_recall != value)
                {
                    _item_recall = value;
                    dirty(true);
                }
            }
        }

        // 道具【消除】
        [JsonProperty]
        int _item_remove = 0;
        [JsonIgnore]
        public int Item_Remove
        {
            get
            {
                return _item_remove;
            }
            set
            {
                if (_item_remove != value)
                {
                    _item_remove = value;
                    dirty(true);
                }
            }
        }
        // 道具【绽放】
        [JsonProperty]
        int _item_bloom = 0;
        [JsonIgnore]
        public int Item_Bloom
        {
            get
            {
                return _item_bloom;
            }
            set
            {
                if (_item_bloom != value)
                {
                    _item_bloom = value;
                    dirty(true);
                }
            }
        }

        // 道具【复活】
        [JsonProperty]
        int _item_life = 0;
        [JsonIgnore]
        public int Item_Life
        {
            get
            {
                return _item_life;
            }
            set
            {
                if (_item_life != value)
                {
                    _item_life = value;
                    dirty(true);
                }
            }
        }

        // 一次性的（去广告）
        [JsonProperty]
        bool _no_ads = false;
        [JsonIgnore]
        public bool No_Ads
        {
            get
            {
                return _no_ads;
            }
            set
            {
                if (_no_ads != value)
                {
                    _no_ads = value;
                    dirty(true);
                }
            }
        }

        // 一次性的（剧情金币奖励）
        [JsonProperty]
        int _story_reward_once = 0;
        [JsonIgnore]
        public int Story_Reward_Once
        {
            get
            {
                return _story_reward_once;
            }
            set
            {
                if (_story_reward_once != value)
                {
                    _story_reward_once = value;
                    dirty(true);
                }
            }
        }

        // home背景
        [JsonProperty]
        int _background_number = 1;
        [JsonIgnore]
        public int Background_Number
        {
            get
            {
                return _background_number;
            }
            set
            {
                if (_background_number != value)
                {
                    _background_number = value;
                    dirty(true);
                }
            }
        }

        // game背景
        [JsonProperty]
        int _game_background_number = 1;
        [JsonIgnore]
        public int Game_Background_Number
        {
            get
            {
                return _game_background_number;
            }
            set
            {
                if (_game_background_number != value)
                {
                    _game_background_number = value;
                    dirty(true);
                }
            }
        }

        // 是否显示过google评价
        [JsonProperty]
        int _android_reviewed;
        [JsonIgnore]
        public int Android_Reviewed
        {
            get
            {
                return _android_reviewed;
            }
            set
            {
                if (_android_reviewed != value)
                {
                    _android_reviewed = value;
                    dirty();
                }
            }
        }

        //// startime
        //[JsonProperty]
        //DateTime _start_time;
        //[JsonIgnore]
        //public DateTime Start_Time
        //{
        //    get
        //    {
        //        return _start_time;
        //    }
        //    set
        //    {
        //        if (_start_time != value)
        //        {
        //            _start_time = value;
        //            dirty(true);
        //        }
        //    }
        //}
        //// leftime
        //[JsonProperty]
        //int _left_time;
        //[JsonIgnore]
        //public int LeftTime
        //{
        //    get
        //    {
        //        return _left_time;
        //    }
        //    set
        //    {
        //        if (_left_time != value)
        //        {
        //            _left_time = value;
        //            dirty(true);
        //        }
        //    }
        //}
    }
}
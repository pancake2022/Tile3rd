using System.Collections;
using System.Collections.Generic;

namespace CSFramework
{
    public class ShareDataGlobalConfig : ShareData<ShareDataGlobalConfig>
    {
        public int _home_music_id;//主页音乐
        public int _game_music_id;//游戏音乐
        public int _game_music_bloom;//bloom音乐

        public int _item_type_id = 0;//局内道具/1.remove/2.recall
        public int _bundle_type_id = 0;//礼包id/3.remove/4.recall/5.flower
        public int _notice_id = 0;//提示id
        public int _sign_reward_id = 1;//给签到奖励时用来传递ID
        public List<int> itemlist = new List<int> { 0, 0, 0, 0 };
        public int _home_fly = 0;//home的飞行/1.levelchest/2.飞心/4.飞小花/5.飞每日任务奖励
        public bool _quest_condition;//猫任务状态
        public int _level_condition;//关卡的种类/1.play关卡/2.猫任务关卡
        public bool _love_exp_pause = true;//动画播放完才会增加loveexp
        public bool _levelwin_count_start;
        public bool _storybundle_check = false;//用来判断bundle在home是否主动弹出（只要局内使用道具就会弹）
        public int _game_outitem_jump = 0;//如果道具为0，则每3关在局内主动弹出提示框
        public bool _is_interstitial = false;//是否有插屏

        public bool _is_catquest_active = false;//测试用dailytask状态
        public bool _is_winstreak = false;//是否处于连赢任务中
        public int _winstreak_notice_type = 0;//1.game_setting/2.revive
        public int _shop_pop_cd = 5;

        public Dictionary<int, bool> _pop_ui = new Dictionary<int, bool>();
        //public Dictionary<int, int> _order_condition = new Dictionary<int, int>();

        //public bool _is_achievebubbleshowed = false;//成就bubble是否显示
        //public int _is_oder_active = 0;//成就bubble是否显示
        //public bool _level_button_active = false;//level按钮是否激活
        //public bool _quest_level;//猫任务关卡
        //public bool _story_game_out = false;//从游戏退出到home时story不切换
    }
}


using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSFramework
{
    public class FrameworkContext
    {
        public string ProjectName;
        public Camera MainCamera;
        public Camera UICamera;
        public Transform SceneRoot;
        public Transform GameObjectPoolRoot;
        // public IConfigRegistry ConfigRegistry;
        public string ConfigJsonRoot;
        public string ConfigRegistryPath;
        public IStorageRegistry StorageRegistry;
        public string ProtocolMapConfigPath;
        public float ProtocolWaitTime = 10.0f;
        public AudioContext AudioContext = new AudioContext();
        public UIContext UIContext = new UIContext();
        public LocalizeContext LocalizeContext = new LocalizeContext();
        public DownloadContext DownloadContext = new DownloadContext();
        public int TickScale = 1;
        public string DefaultScenePath;
        public string DefaultLoadingScenePath;
    }
    public enum DeviceQualityLevel
    {
        Low,
        High,
        // todo more level
    }
    public class Framework : Module<BaseModule>
    {
        private static Framework _instance;

        public FrameworkContext Context { get; private set; }
        public DeviceQualityLevel DeviceQualityLevel { get; private set; }
        #region Module
        /// <summary>
        /// 原生管理器
        /// 用于管理安卓和iOS的相关接口实现
        /// </summary>
        public NativeManager NativeManager { get { return submodule<NativeManager>(); }}
        /// <summary>
        /// 版本管理器
        /// 用于管理程序的版本和资源版本
        /// </summary>
        public VersionManager VersionManager { get { return submodule<VersionManager>(); }}
        /// <summary>
        /// 下载管理器
        /// 用于管理游戏资源的下载
        /// </summary>
        public DownloadManager DownloadManager { get { return submodule<DownloadManager>(); }}
        /// <summary>
        /// 资源管理器
        /// 用于管理游戏资源的加载和释放
        /// </summary>
        public ResourcesManager ResourcesManager { get { return submodule<ResourcesManager>(); }}
        /// <summary>
        /// 声音管理器
        /// 用于管理游戏内的背景音乐和音效
        /// </summary>
        public AudioManager AudioManager { get { return submodule<AudioManager>(); }}
        /// <summary>
        /// 事件管理器
        /// 用于管理和分发游戏内的自定义事件
        /// </summary>
        public EventManager EventManager { get { return submodule<EventManager>(); }}
        /// <summary>
        /// GameObject对象池
        /// 用于管理对象的缓存池
        /// </summary>
        public GameObjectPoolManager GameObjectPoolManager { get { return submodule<GameObjectPoolManager>(); }}
        /// <summary>
        /// 场景管理器
        /// 用于管理游戏内的场景的加载, 释放, 以及切换
        /// </summary>
        public SceneManager SceneManager { get { return submodule<SceneManager>(); }}
        /// <summary>
        /// UI管理器
        /// 用于管理游戏内的UI窗口
        /// </summary>
        public UIManager UIManager { get { return submodule<UIManager>(); }}
        /// <summary>
        /// 多语言管理器
        /// 用于管理游戏内的多语言文本
        /// </summary>
        public LocalizeManager LocalizeManager { get { return submodule<LocalizeManager>(); }}
        /// <summary>
        /// 配置管理器
        /// 用于管理游戏内的配置加载和读取
        /// </summary>
        public ConfigManager ConfigManager { get { return submodule<ConfigManager>(); }}
        /// <summary>
        /// 存档管理器
        /// 用于管理游戏内存档的加载和存储
        /// </summary>
        public StorageManager StorageManager { get { return submodule<StorageManager>(); }}
        /// <summary>
        /// 网络管理器
        /// 用于管理网络连接相关的内容
        /// </summary>
        public NetManager NetManager { get { return submodule<NetManager>(); }}
        /// <summary>
        /// 共享数据管理器
        /// 用于管理游戏内各个模块的共享数据
        /// </summary>
        public ShareDataManager ShareDataManager { get { return submodule<ShareDataManager>(); }}
        /// <summary>
        /// 子模块管理器
        /// 用于管理游戏内的各个自定义子模块
        /// </summary>
        public SubsystemContainer SubsystemContainer { get { return submodule<SubsystemContainer>(); }}
        #endregion
        #region FPS
        public float FPS { get; private set; }
        private int _fps_frame_count;
        private long _fps_escape_ms_time;
        private long _fps_measuring_ms_time;
        private long _last_drill_timestamp;
        #endregion

        public static void SetStorageDirty (bool force_save)
        {
            _instance.StorageManager.Dirty(force_save);
        }

        public IEnumerator Init (FrameworkContext context, BaseModule main_module)
        {
            _instance = this;

            Context = context;

            init_device_quality_level();

            yield return base.Init(main_module);
            
            yield return init_base_submodule();
            yield return init_all_submodule();
            yield return on_init_finished();

            FPS = 0;
            _fps_frame_count = 0;
            _fps_escape_ms_time = 0;
            _fps_measuring_ms_time = 2000;
            _last_drill_timestamp = 0;
        }

        private void init_device_quality_level ()
        {
            DeviceQualityLevel = DeviceQualityLevel.High;
            
            if (Application.platform == RuntimePlatform.Android)
            {
                if ((SystemInfo.processorFrequency != 0 && SystemInfo.processorFrequency < 1250) || // CPU低于1.2GH
                    (SystemInfo.systemMemorySize != 0 && SystemInfo.systemMemorySize < 1200)) // 或者内存低于1G
                {
                    DeviceQualityLevel = DeviceQualityLevel.Low;
                }
            }
            // else if (Application.platform == RuntimePlatform.IPhonePlayer) // 苹果设备不能获取cpu频率，这里通过GPU型号和内存判断
            // {
            //     if (SystemInfo.systemMemorySize != 0 && SystemInfo.systemMemorySize < 1200) // 内存低于1G
            //     {
            //         DeviceQualityLevel = DeviceQualityLevel.Low;
            //     }
            // }

            Application.targetFrameRate = DeviceQualityLevel == DeviceQualityLevel.Low ? 30 : 60;
            CSFramework.Logger.Log("Application.targetFrameRate: " + Application.targetFrameRate);
        }

        private IEnumerator init_base_submodule ()
        {
            yield return register_submodule<NativeManager>();
            yield return register_submodule<VersionManager>();
            yield return register_submodule<DownloadManager>();
            yield return register_submodule<ResourcesManager>();
            yield return register_submodule<AudioManager>();
            // yield return VersionManager.CheckCopyAssetBundle();
        }

        private IEnumerator init_all_submodule ()
        {
            yield return register_submodule<GameObjectPoolManager>();
            yield return register_submodule<EventManager>();
            yield return register_submodule<SceneManager>();
            yield return register_submodule<UIManager>();
            yield return register_submodule<LocalizeManager>();
            yield return register_submodule<ConfigManager>();
            yield return register_submodule<StorageManager>();
            yield return register_submodule<NetManager>();
            yield return register_submodule<ShareDataManager>();
            yield return register_submodule<SubsystemContainer>();
        }

        private IEnumerator on_init_finished ()
        {
            LocalizeManager.AutoSelectLocalName();
            
            _enable = true;
            Logger.Log("framework on_init_finished");
            yield return null;
        }

        protected override void on_tick_drive (long ms_dt)
        {
            ++_fps_frame_count;
            _fps_escape_ms_time += ms_dt;
            if (_fps_escape_ms_time >= _fps_measuring_ms_time)
            {
                FPS = _fps_frame_count / (_fps_escape_ms_time / 1000.0f);
                _fps_frame_count = 0;
                _fps_escape_ms_time = 0;
            }

            var tick_scale = Mathf.Max(1, Context.TickScale);
            for (var i = 0; i < tick_scale; ++i)
                base.on_tick_drive(ms_dt);
        }

        public void ResetDrillTime ()
        {
            _last_drill_timestamp = (long)Utils.CurrentTimestamp();
        }

        public void DrillTime (string str)
        {
            var current_timestamp = (long)Utils.CurrentTimestamp();
            if (_last_drill_timestamp == 0)
                log($"Drill[{str}] => start");
            else
                log($"Drill[{str}] => {current_timestamp - _last_drill_timestamp} ms");

            _last_drill_timestamp = current_timestamp;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSFramework;
using System;
using ExcelDataReader.Log;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
// using AppsFlyerSDK;

public class Game : Module<CSBehaviour>
{
    private static Game _instance;
    public Framework Framework { get; private set; }

    public Camera MainCamera;
    public Camera UICamera;
    public Transform SceneRoot;
    public Transform GameObjectPoolRoot;
    public Transform DefaultAudioSourceTransform;
    #region UI
    public Canvas UIRoot;
    public Transform DefaultUIRoot;
    public Transform SceneUIRoot;
    public Transform SceneUIEffectRoot;
    public Transform CommonUIRoot;
    public Transform CommonUIEffectRoot;
    public Transform GuideUIRoot;
    public Transform NoticeUIRoot;
    public Transform MessageUIRoot;
    public Transform BlockUIRoot;
    public Transform GameManager;
    #endregion
    #region SceneAdapt
    public float DesignMinFactor = 1.333f;
    public float CameraMinSize = 5.4f;
    public float DesignMaxFactor = 2.2f;
    public float CameraMaxSize = 7.2f;
    #endregion
    public Transform Splash;
    public Transform DebugConsoleEntry;
    public string ProjectName = "Tile2nd";

    private void Awake () 
    {
        _instance = this;

        // 初始化适配
        refresh_screen_adapt(Screen.width, Screen.height);

#if DEBUG || DEVELOPMENT_BUILD
        DebugConsoleEntry.SetActive(true);
#else
        DebugConsoleEntry.SetActive(false);
#endif

        Framework = this.gameObject.AddComponent<Framework>();

        StartCoroutine(init_game());
    }

    private void Start ()
    {
        try
        {
            init_data_analytics();
        }
        catch (Exception e)
        {
            error($"Init Data Analytics Exception: {e}");
        }
    }

    private void init_data_analytics ()
    {
        // 初始化数据统计
    }

    private IEnumerator init_game ()
    {
        var config_registry = new ConfigRegistry();

        var context = new FrameworkContext
        {
            ProjectName = ProjectName,
            MainCamera = MainCamera,
            SceneRoot = SceneRoot,
            GameObjectPoolRoot = GameObjectPoolRoot,
            UICamera = UICamera,
            
            // ConfigRegistry = config_registry,
            ConfigJsonRoot = "Config",
            ConfigRegistryPath = "Config/Common/registry",
            StorageRegistry = new StorageRegistry(),
            // ProtocolMapConfigPath = "Config/Common/ProtocolMap",
            
            // TickScale = 5, // todo

            AudioContext = new AudioContext
            {
                MusicResourceRootPath = "Audio/Music",
                SoundResourceRootPath = "Audio/Sound",
                DefaultAudioSourceTransform = DefaultAudioSourceTransform,
                SameSoundMaxPlayingCount = 4,
            },

            UIContext = new UIContext
            {
                UIRoot = UIRoot,
                WindowUILayerDict = new Dictionary<UILayer, Transform>
                {
                    { UILayer.Default, DefaultUIRoot},
                    { UILayer.SceneUI, SceneUIRoot},
                    { UILayer.SceneUIEffect, SceneUIEffectRoot},
                    { UILayer.Common, CommonUIRoot},
                    { UILayer.CommonUIEffect, CommonUIEffectRoot},
                    { UILayer.Guide, GuideUIRoot},
                    { UILayer.Notice, NoticeUIRoot},
                    { UILayer.Message, MessageUIRoot},
                    { UILayer.Block, BlockUIRoot},
                    { UILayer.GameManager, GameManager},
                },
                PrefabRootPath = "Prefab/UI",
                CommonMessagePrefabPath = "Common/Base/CommonMessage",
                CommonWindowPrefabPath = "Common/Base/Notice",
                CommonWaitingPrefabPath = "Common/Base/Waiting",
                CommonAnimationTransformName = "Content",
                AudioContext = new UIAudioContext
                {
                    // ButtonClicked = AudioConst.button_clicked,
                    // ToggleSelected = AudioConst.button_clicked,
                    // WindowOpen = AudioConst.window_open,
                    // WindowClose = AudioConst.window_close,
                },
            },

            LocalizeContext = new LocalizeContext
            {
                RequireLocalizeDataFunc = require_localize_data,
                RequireLocalizeFontFunc = require_localize_font,
                RequireLocalizeMaterialFunc = require_localize_material,
                ParseFontNameFunc = Game.ParseFontName,
                SupportLocalNameList = new List<string>
                {
                    "en"
                },
                LocalizeKeyPrefix = "&key.",
            },

            DefaultScenePath = "Prefab/Scene/EmptyScene",
            DefaultLoadingScenePath = "Prefab/Scene/EmptyScene",
        };
        yield return Framework.Init(context, this);

        yield return init_subsystem();

        yield return EnterScene();
    }

    private Dictionary<string, string> require_localize_data (string local_name)
    {
        var config_path = "Config/LocalizeConfig/localize_" + local_name;
        var config_asset = Framework.ResourcesManager.LoadResource<TextAsset>(config_path);
        if (config_asset)
        {
            var config_list = JsonConvert.DeserializeObject<List<LocalizeConfig>>(config_asset.text);
            var localize_data = new Dictionary<string, string>();
            foreach (var config in config_list)
            {
                if (!string.IsNullOrEmpty(config.Key))
                    localize_data[config.Key] = config.Value;
            }
            return localize_data;
        }
        return null;
    }

    public static bool LoadLocalizeFontInEditor (TMP_FontAsset old_font, string local_name, out TMP_FontAsset font_asset)
    {
#if UNITY_EDITOR
        var font_name = ParseFontName(old_font);
        var resource_path = Utils.GetEditorExtraResourcesPath($"Fonts/{local_name}/{font_name}_{local_name} SDF.asset");
        font_asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(resource_path);
        return font_asset != null;
#else
        font_asset = null;
        return false;
#endif
    }

    public static bool LoadLocalizeMaterialInEditor (TMP_FontAsset old_font, string local_name, string material_name, out Material material)
    {
#if UNITY_EDITOR
        var font_name = ParseFontName(old_font);
        var resource_path = Utils.GetEditorExtraResourcesPath($"Fonts/{local_name}/{font_name}_{local_name} SDF {material_name}.mat");
        material = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(resource_path);
        return material != null;
#else
        material = null;
        return false;
#endif
    }

    private bool require_localize_font (TMP_FontAsset old_font, string local_name, out TMP_FontAsset font_asset)
    {
        var font_name = ParseFontName(old_font);
        var resource_path = $"Fonts/{local_name}/{font_name}_{local_name} SDF";
        font_asset = Framework.ResourcesManager.LoadResource<TMP_FontAsset>(resource_path);
        return font_asset != null;
    }

    private bool require_localize_material (TMP_FontAsset old_font, string local_name, string material_name, out Material material)
    {
        var font_name = ParseFontName(old_font);
        var resource_path = $"Fonts/{local_name}/{font_name}_{local_name} SDF {material_name}";
        material = Framework.ResourcesManager.LoadResource<Material>(resource_path);
        return material != null;
    }

    public static string ParseFontName (TMP_FontAsset old_font)
    {
        if (old_font == null)
        {
            CSFramework.Logger.Error("ParseFontName error, font was null");
            return "LocaleFont";
        }
        else
        {
            var index = old_font.name.IndexOf('_');
            if (index == -1)
            {
                CSFramework.Logger.Error("ParseFontName error, invalid format: {old_font.name}");
                return "LocaleFont";
            }
            else
            {
                return old_font.name.Substring(0, index);
            }
        }
    }

    public IEnumerator EnterScene ()
    {
        // LoginScene scene
        yield return Framework.SceneManager.LoadScene<LoginScene, BaseLoadingScene>(Framework.Context.DefaultScenePath, Framework.Context.DefaultLoadingScenePath, new object[]{});
    }

    private IEnumerator init_subsystem ()
    {
        yield return null;
    }

    private void Update() 
    {
        Framework.TickDrive((long)(Time.deltaTime * 1000));
    }

    private void OnApplicationPause(bool is_pause) 
    {

    }

    private void refresh_screen_adapt (float screen_width, float screen_height)
    {
        var factor = screen_height / screen_width;
        var size = CameraMinSize + (CameraMaxSize - CameraMinSize) * ((factor - DesignMinFactor) / (DesignMaxFactor - DesignMinFactor));
        size = Mathf.Clamp(size, CameraMinSize, CameraMaxSize);
        MainCamera.orthographicSize = size;

        // if (Framework && Framework.ShareDataManager)
        //     Framework.ShareDataManager.Data<CommonShareData>().OnScreenSizeChanged?.Invoke(screen_width, screen_height);
    }

#if UNITY_EDITOR
    private float _screen_width;
    private float _screen_height;
    private void OnGUI() 
    {
        if (_screen_width != Screen.width || _screen_height != Screen.height)
        {
            _screen_width = Screen.width;
            _screen_height = Screen.height;
            refresh_screen_adapt(Screen.width, Screen.height);
        }
    }
#endif
}

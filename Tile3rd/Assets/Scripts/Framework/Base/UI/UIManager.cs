using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using DG.Tweening;
using DG.Tweening.Core;
using Spine.Unity;
using UnityEngine.U2D;

namespace CSFramework
{
    public enum UILayer
    {
        Default, // 默认
        SceneUI, // 场景UI
        SceneUIEffect, // 场景UI特效
        Common, // 通用
        CommonUIEffect, // 通用UI特效
        Guide, // 引导
        Notice, // 提示框
        Message, // 提示消息
        Block, // 阻塞UI
        GameManager,//广告/review
    }

    public class UIContext
    {
        public Canvas UIRoot;
        public Dictionary<UILayer, Transform> WindowUILayerDict = new Dictionary<UILayer, Transform>();
        public string PrefabRootPath;
        public string CommonMessagePrefabPath;
        public string CommonWindowPrefabPath;
        public string CommonWaitingPrefabPath;
        public UIAudioContext AudioContext;
        public float TickInterval = 0.2f;
        public float ButtonContinuousTriggerInterval = 12f / 60f;
        public string CommonAnimationTransformName;
        public int WindowSortingOrderOffset = 100;
    }

    public class UIAudioContext
    {
        public string ButtonClicked;
        public string ToggleSelected;
        public string WindowOpen;
        public string WindowClose;
    }
    
    public class UIManager : Module<Framework>
    {
        public Framework Framework { get { return _main_module; } }
        public UIContext UIContext { get { return _context; } }
        public IEnumerable<WindowUI> AllWindow { get { return _window_dict.Values; } }
        public IEnumerable<BaseUI> AllRootUI { get { return _root_ui_set; } }

        public Action<BaseUI, bool> OnCreateUICallback;
        public Action<float> OnUITickCallback;
        public Action<float> OnRealTimeTickCallback;

        private Dictionary<string, WindowUI> _window_dict;
        private HashSet<BaseUI> _root_ui_set;
        public UIContext _context;//原本是private
        private float _escape_time;

        protected override IEnumerator on_init (params object[] param_list)
        {
            _window_dict = new Dictionary<string, WindowUI>();
            _root_ui_set = new HashSet<BaseUI>();
            _context = _main_module.Context.UIContext;
            var window_layer_list = Enum.GetValues(typeof(UILayer));
            foreach (var layer in window_layer_list)
            {
                var window_layer = (UILayer)layer;
                if (!_context.WindowUILayerDict.ContainsKey(window_layer))
                {
                    _context.WindowUILayerDict[window_layer] = _context.UIRoot.transform;
                    Logger.Warning(string.Format("UIManager.Context.WindowUILayerDict: not found WindowLayer[{0}], auto convert to UIRoot", window_layer));
                }
            }
            _escape_time = 0;
            yield return null;
        }

        public T CreateUI<T> (Transform t, BaseUI root_ui) where T : BaseUI
        {
            return BaseUI.Create<T>(t, this, false, root_ui);
        }

        public T CreateUI<T> (string ui_prefab_path, Transform parent_t, BaseUI root_ui) where T : BaseUI
        {
            return BaseUI.Create<T>(ui_prefab_path, parent_t, this, root_ui);
        }

        public T CreateUI<T> (GameObject clone, Transform parent_t, BaseUI root_ui) where T : BaseUI
        {
            return BaseUI.Create<T>(clone, parent_t, this, root_ui);
        }

        public T CreateRootUI<T> (string ui_prefab_path, UILayer ui_layer, bool dont_cleanup = false, int sorting_order_offset = 0) where T : BaseUI
        {
            var ui_layer_root = _context.WindowUILayerDict[ui_layer];
            var ui = BaseUI.Create<T>(ui_prefab_path, ui_layer_root, this, null);
            ui.DontCleanup = dont_cleanup;
            
            // add graphic raycaster
            Utils.GetOrCreateComponent<GraphicRaycaster>(ui);
            var canvas = Utils.GetOrCreateComponent<Canvas>(ui);
            canvas.overrideSorting = true;
            canvas.sortingOrder = ui_layer_root.GetComponent<Canvas>().sortingOrder + sorting_order_offset;

            _root_ui_set.Add(ui);
            OnCreateUICallback?.Invoke(ui, true);
            return ui;
        }

        public GameObject LoadUIGameObject (string ui_prefab_path, Transform parent)
        {
            var full_path = string.Format("{0}/{1}", _context.PrefabRootPath, ui_prefab_path);
            // var prefab = _main_module.ResourcesManager.LoadResource<GameObject>(full_path, true);
            // var ui_obj = GameObject.Instantiate(prefab, parent);
            var ui_obj = _main_module.GameObjectPoolManager.Spawn(full_path, parent);
            // ui_obj.name = Utils.RemoveClone(ui_obj.name);
            return ui_obj;
        }

        public Sprite FindSprite (string atlas_name, string sprite_name, bool warning = true)
        {
            var atlas = Framework.ResourcesManager.LoadSpriteAtlas(atlas_name);
            if (atlas)
            {
                var sp = atlas.GetSprite(sprite_name);
                if (!sp && warning)
                    Logger.Warning(string.Format("UIManager.FindSprite Error: not found Sprite: {0} in {1}", sprite_name, atlas_name));
                return sp;
            }
            else if (warning)
            {
                Logger.Warning(string.Format("UIManager.FindSprite Error: not found SpriteAtlas: {0} for {1}", atlas_name, sprite_name));
            }
            return null;
        }

        public SpriteAtlas PreloadSpriteAtlas (string atlas_name)
        {
            return Framework.ResourcesManager.LoadSpriteAtlas(atlas_name);
        }

        public CommonMessageUI AddCommonMessage (object msg_obj)
        {
            return AddCommonMessage(msg_obj.ToString());
        }

        public CommonMessageUI AddCommonMessage (string msg)
        {
            var ui = OpenWindowWithPath<CommonMessageUI>(_context.CommonMessagePrefabPath, UILayer.Message);
            ui.AddMessage(msg);
            return ui;
        }

        public CommonWindow OpenCommonWindow (CommonWindow.Data data = null)
        {
            var window = OpenWindowWithPath<CommonWindow>(_context.CommonWindowPrefabPath, UILayer.Notice);
            if (data != null)
                window.Refresh(data);
            return window;
        }

        public WaitingWindow OpenWaitingWindow (float delay_time = 0)
        {
            var window = FindWindow<WaitingWindow>();
            if (window)
            {
                window.AddReference();
            }
            else
            {
                window = OpenWindowWithPath<WaitingWindow>(_context.CommonWaitingPrefabPath, UILayer.Block);
                window.Init(delay_time);
            }
            return window;
        }

        public void CloseWaitingWindow ()
        {
            var window = FindWindow<WaitingWindow>();
            if (window)
                window.SubReference();
        }

        public T OpenWindow<T> (UILayer window_layer = UILayer.Common) where T : WindowUI
        {
            var window_path = typeof(T).GetField("DefaultPrefabPath").GetValue(null) as string;
            return OpenWindowWithPath<T>(window_path, window_layer);
        }

        public WindowUI OpenWindow (Type window_type, UILayer window_layer = UILayer.Common)
        {
            var window_path = window_type.GetField("DefaultPrefabPath").GetValue(null) as string;
            return OpenWindowWithPath(window_type, window_path, window_layer);
        }

        public T OpenWindowWithPath<T> (string window_path, UILayer window_layer = UILayer.Common) where T : WindowUI
        {
            return OpenWindowWithPath(typeof(T), window_path, window_layer) as T;
        }

        public WindowUI OpenWindowWithPath (Type window_type, string window_path, UILayer window_layer = UILayer.Common)
        {
            var window = FindWindow(window_type, window_path);
            if (!window)
            {
                if (string.IsNullOrEmpty(window_path))
                {
                    error($"OpenWindow[{window_type}] Failed, window_path was null.");
                }
                else
                {
                    window = create_window(window_type, window_path, window_layer);
                    _window_dict[window_path] = window;
                }
            }

            if (window)
                open_window(window, window_path);
                
            return window;
        }

        public bool TryCloseWindow<T> () where T : WindowUI
        {
            return TryCloseWindow(typeof(T));
        }

        public bool TryCloseWindow (Type type)
        {
            var window = FindWindow(type);
            if (window)
            {
                window.Close();
                return true;
            }
            return false;
        }

        public T FindWindow<T> (string window_path = null) where T : WindowUI
        {
            return FindWindow(typeof(T), window_path) as T;
        }

        public WindowUI FindWindow (Type window_type, string window_path = null)
        {
            if (string.IsNullOrEmpty(window_path))
            {
                foreach (var window in _window_dict.Values)
                {
                    if (window.GetType().Equals(window_type))
                        return window;
                }
            }
            else
            {
                if (_window_dict.TryGetValue(window_path, out var window))
                    return window;
            }
            return null;
        }


        public void RemoveWindow (string window_path)
        {
            if (_window_dict.TryGetValue(window_path, out var window))
            {
                _window_dict.Remove(window_path);
            }
            else
            {
                Logger.Warning(string.Format("UIManager.RemoveWindow Warning: not found window [{0}]", window_path));
            }
        }

        public void OnCleanupScene ()
        {
            var window_key_list = new List<string>(_window_dict.Keys);
            foreach (var window_key in window_key_list)
            {
                var window = _window_dict[window_key];
                if (!window.Property.DontCloseWhenCleanupScene)
                    window.Close();
            }

            var root_ui_set = new HashSet<BaseUI>(_root_ui_set);
            foreach (var root_ui in root_ui_set)
            {
                if (!root_ui.DontCleanup)
                    root_ui.DestroySelf();
            }
        }

        public void PlayCommonOpenAnimation (Transform t, Action complete_callback)
        {
            var group = Utils.GetOrCreateComponent<CanvasGroup>(t.parent);
            var scale = t.localScale;
            t.localScale = Vector3.zero;
            group.alpha = 0;
            
            var sequence = DOTween.Sequence();
            var tweenScale1 = t.DOScale(scale * 1.04f, 0.13f).SetEase(Ease.OutBounce);
            var tweenScale2 = t.DOScale(scale * 0.997f, 0.17f).SetEase(Ease.InOutBounce);
            var tweenScale3 = t.DOScale(scale * 1, 0.13f).SetEase(Ease.InOutBounce);
            var tweenCallback = t.DOScale(scale * 1f, 0.08f).OnComplete(() =>
            {
                try
                {
                    complete_callback?.Invoke(); 
                }
                catch (Exception e)
                {
                    CSFramework.Logger.Error(e);
                }
            });

            sequence.Append(tweenScale1);
            sequence.Append(tweenScale2);
            sequence.Append(tweenScale3);
            sequence.Append(tweenCallback);
            
            var alphaAnim = DOTween.To(
                () => group.alpha,
                (x) =>
                {
                    group.alpha = x;
                }, 
                1.0f, 
                0.13f).SetEase(Ease.InOutBounce);
            alphaAnim.Play();
            
            var skeltons = t.GetComponentsInChildren<SkeletonGraphic>(true);
            for (var i = 0; i < skeltons.Length; ++i)
            {
                var skelton = skeltons[i];
                var skelton_group = Utils.GetOrCreateComponent<CanvasGroup>(skelton);
                skelton_group.ignoreParentGroups = true;
                var oldValue = skelton.color.a;
                skelton.color = new Color(skelton.color.r, skelton.color.g, skelton.color.b, 0);
                var skeletonAlphaAnim = DOTween.To(
                    () => skelton.color.a,
                    (x) =>
                    {
                        skelton.color = new Color(skelton.color.r, skelton.color.g, skelton.color.b, x);
                    }, 
                    oldValue, 
                    0.13f).SetEase(Ease.InOutBounce);
                skeletonAlphaAnim.Play();
            }
        }

        public void PlayCommonCloseAnimation (Transform t, Action complete_callback)
        {
            var group = Utils.GetOrCreateComponent<CanvasGroup>(t.parent);
            
            var scale = t.localScale;
            
            var alphaAnim = DOTween.To(
                () => group.alpha,
                (x) =>
                {
                    group.alpha = x;
                }, 
                0.0f, 
                0.13f).SetEase(Ease.OutSine).SetDelay(0.06f);
            
            var scaleAnim = t.DOScale(scale * 0.7f, 0.2f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                try
                {
                    complete_callback?.Invoke(); 
                }
                catch (Exception e)
                {
                    CSFramework.Logger.Error(e);
                }
            });

            var sequence = DOTween.Sequence();
            sequence.Append(alphaAnim);
            sequence.Append(scaleAnim);
            
            var skeltons = t.GetComponentsInChildren<SkeletonGraphic>(true);
            for (var i = 0; i < skeltons.Length; ++i)
            {
                var skelton = skeltons[i];
                var skelton_group = Utils.GetOrCreateComponent<CanvasGroup>(skelton);
                skelton_group.ignoreParentGroups = true;
                var skeletonAlphaAnim = DOTween.To(
                    () => skelton.color.a,
                    (x) =>
                    {
                        skelton.color = new Color(skelton.color.r, skelton.color.g, skelton.color.b, x);
                    }, 
                    0.0f, 
                    0.13f).SetEase(Ease.OutSine).SetDelay(0.06f);;
                skeletonAlphaAnim.Play();
            }
        }

        private T create_window<T> (string window_path, UILayer window_layer) where T : WindowUI
        {
            var window_obj = LoadUIGameObject(window_path, _context.WindowUILayerDict[window_layer]);
            var window = WindowUI.Create<T>(window_obj.transform, this, window_path, window_layer);
            return window;
        }

        private WindowUI create_window (Type window_type, string window_path, UILayer window_layer)
        {
            var window_obj = LoadUIGameObject(window_path, _context.WindowUILayerDict[window_layer]);
            var window = WindowUI.Create(window_type, window_obj.transform, this, window_path, window_layer);
            return window;
        }

        private void open_window (WindowUI window, string window_path)
        {
            switch (window.State)
            {
                case WindowUI.WindowState.None:
                case WindowUI.WindowState.Hided:
                case WindowUI.WindowState.Closed:
                {
                    window.Open();
                    break;
                }
                case WindowUI.WindowState.HideAnimation:
                {
                    // todo
                    break;
                }
                case WindowUI.WindowState.OpenAnimation:
                case WindowUI.WindowState.Opened:
                {
                    // do nothing
                    break;
                }
                case WindowUI.WindowState.CloseAnimation:
                {
                    // don't arrived there
                    // reopen
                    window.Open();
                    break;
                }
            };
        }

        public int GetWindowMaxSortingOrder (UILayer ui_layer)
        {
            var max_sorting_order = GetWindowLayerSortingOrder(ui_layer);
            foreach (var window in _window_dict.Values)
            {
                if (window.Layer == ui_layer && window.State != WindowUI.WindowState.CloseAnimation && window.State != WindowUI.WindowState.Closed)
                {
                    var canvas = window.GetComponent<Canvas>();
                    if (canvas && canvas.sortingOrder > max_sorting_order)
                        max_sorting_order = canvas.sortingOrder;
                }
            }
            return max_sorting_order;
        }

        public int GetWindowLayerSortingOrder (UILayer ui_layer)
        {
            return _context.WindowUILayerDict[ui_layer].GetComponent<Canvas>().sortingOrder;
        }

        protected override void on_tick (float dt)
        {
            _escape_time += Time.deltaTime;
            if (_escape_time >= _context.TickInterval)
            {
                var window_list = new List<WindowUI>(_window_dict.Values);
                foreach (var window in window_list)
                    window.Tick(_escape_time);

                OnUITickCallback?.Invoke(_escape_time);

                _escape_time = 0;
            }
            OnRealTimeTickCallback?.Invoke(dt);
        }

        public void SetEnableTouch (bool value)
        {
            foreach (var wd in _window_dict.Values)
            {
                var gr = wd.GetComponent<GraphicRaycaster>();
                if (gr)
                    gr.enabled = value;
            }
        }

        public void OnDestroyUI (BaseUI ui)
        {
            _root_ui_set.Remove(ui);
        }
    }
}
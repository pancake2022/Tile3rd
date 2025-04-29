using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace CSFramework
{
    public abstract class WindowUI : BaseUI, IPointerClickHandler
    {
        public enum WindowState
        {
            None,
            OpenAnimation, // 创建出来
            Opened, // 彻底激活
            CloseAnimation, // 已经从UIManager中清除
            Closed, // 彻底清除
            HideAnimation, // 暂停激活状态
            Hided, // 隐藏状态
        }

        public class WindowProperty
        {
            public bool DontCloseWhenCleanupScene = false;
            public bool UseCommonAnimation = true; // 是否使用公用动画（开启&关闭）
            public Transform CommonAnimationTransform = null; // 播放公用动画的节点
            public RectTransform CloseWhenClickOutsideWindowRect = null; // 点击窗口区域之外关闭窗口
            public Func<bool> ClickOutsideWindowRectCallback = null; // 点击窗口区域之外的回调，返回值为是否关闭窗口
            public bool PlayOpenCloseSound = true; // 播放界面开启关闭音效
        }

        public static string DefaultPrefabPath = null;
        public WindowState State { get; private set; } = WindowState.None;
        public WindowProperty Property { get; } = new WindowProperty();
        public UILayer Layer { get { return _ui_layer; } }
        public Action<WindowUI> OnWindowClose;
        public Action<WindowUI> OnWindowRefreshOrder;
        private string _window_path;
        private UILayer _ui_layer;
        private int _delay_call_id;
        private Action _open_callback;
        private Action _create_after_show_callback;

        public static T Create<T>(Transform t, UIManager ui_manager, string window_path, UILayer ui_layer) where T : WindowUI
        {
            return Create(typeof(T), t, ui_manager, window_path, ui_layer) as T;
        }

        public static WindowUI Create(Type type, Transform t, UIManager ui_manager, string window_path, UILayer ui_layer)
        {
            var window = BaseUI.Create(type, t, ui_manager, true, null) as WindowUI;
            window._window_path = window_path;
            window._ui_layer = ui_layer;
            if (window.Property.UseCommonAnimation && window.Property.CommonAnimationTransform == null)
            {
                var transform_name = ui_manager.UIContext.CommonAnimationTransformName;
                if (!string.IsNullOrEmpty(transform_name))
                    window.Property.CommonAnimationTransform = window.transform.Find(transform_name);
                
                if (!window.Property.CommonAnimationTransform)
                    window.Property.CommonAnimationTransform = window.transform;
            }
            window.gameObject.SetActive(false); // 初始隐藏
            return window;
        }

        public void Open ()
        {
            if (Property.PlayOpenCloseSound)
                play_sound(_ui_manager.UIContext.AudioContext.WindowOpen);
                
            _ignore_sound = true;
            State = WindowState.OpenAnimation;

            if (Property.CommonAnimationTransform)
            {
                gameObject.SetActive(true);
                var create_after_show_cb = _create_after_show_callback;
                _create_after_show_callback = null;
                Utils.SafeCall(create_after_show_cb);
                refresh_order();
                _ui_manager.PlayCommonOpenAnimation(Property.CommonAnimationTransform, () => real_open(false));
            }
            else
            {
                _delay_call_id = _ui_manager.DelayCall(() => real_open(true), 0); // next frame open
            }
        }

        protected void real_open (bool call_after_create_show)
        {
            gameObject.SetActive(true);
            if (call_after_create_show)
            {
                var create_after_show_cb = _create_after_show_callback;
                _create_after_show_callback = null;
                Utils.SafeCall(create_after_show_cb);
            }

            refresh_order();

            // add graphic raycaster
            Utils.GetOrCreateComponent<GraphicRaycaster>(this);

            on_open();
            State = WindowState.Opened;
            add_bi(BI_UIEventType.WindowOpened, this);
            _ignore_sound = false;

            var open_cb = _open_callback;
            _open_callback = null;
            open_cb?.Invoke();
        }

        protected void refresh_order ()
        {
            SortingOrder = _ui_manager.GetWindowMaxSortingOrder(_ui_layer) + _ui_manager.UIContext.WindowSortingOrderOffset;

            var canvas = Utils.GetOrCreateComponent<Canvas>(this);
            canvas.overrideSorting = true;
            canvas.sortingOrder = SortingOrder;

            refresh_child_node_order(SortingOrder);

            OnWindowRefreshOrder?.Invoke(this);
        }

        public void WhenOpen (Action callback)
        {
            if (State == WindowState.Opened)
                callback?.Invoke();
            else
                _open_callback += callback;
        }

        public void WhenCreateAfterShow (Action callback)
        {
            if (gameObject.activeSelf)
                callback?.Invoke();
            else
                _create_after_show_callback += callback;
        }

        public void Close ()
        {
            add_bi(BI_UIEventType.WindowClosed, this);
            if (Property.PlayOpenCloseSound)
                play_sound(_ui_manager.UIContext.AudioContext.WindowClose);

            StopAllCoroutines();

            if (State == WindowState.OpenAnimation)
            {
                State = WindowState.CloseAnimation;
                if (_delay_call_id > 0)
                {
                    _ui_manager.CancelDelayCall(_delay_call_id);
                    _delay_call_id = 0;
                }
                _delay_call_id = _ui_manager.DelayCall(real_close, 0); // next frame close
            }
            else
            {
                State = WindowState.CloseAnimation;
                if (Property.CommonAnimationTransform)
                {
                    _ui_manager.PlayCommonCloseAnimation(Property.CommonAnimationTransform, real_close);
                }
                else
                {
                    _ui_manager.DelayCall(real_close, 0); // next frame close
                }
            }
        }

        protected void real_close ()
        {
            // 可能由于其他操作重新打开了Window
            if (State != WindowState.CloseAnimation) 
                return;

            if (_delay_call_id > 0)
            {
                _ui_manager.CancelDelayCall(_delay_call_id);
                _delay_call_id = 0;
            }
            on_close();
            State = WindowState.Closed;
            _ui_manager.RemoveWindow(_window_path);
            Utils.SafeCall(() => OnWindowClose?.Invoke(this));
            // todo play animation
            destroy_ui(this);
        }

        public sealed override void Show ()
        {
            on_show();
            State = WindowState.Opened;
            add_bi(BI_UIEventType.WindowShow, this);
        }

        public sealed override void Hide ()
        {
            add_bi(BI_UIEventType.WindowHide, this);
            on_hide();
            State = WindowState.Hided;
            gameObject.SetActive(false);
        }

        public void Tick (float dt)
        {
            if (State == WindowState.Opened)
                on_tick(dt);
        }

        protected virtual void on_open () {}
        protected virtual void on_close () {}
        protected virtual void on_show () {}
        protected virtual void on_hide () {}
        protected virtual void on_tick (float dt) {}

        public void OnPointerClick (PointerEventData data)
        {
            if (Property.CloseWhenClickOutsideWindowRect && State == WindowState.Opened)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(Property.CloseWhenClickOutsideWindowRect, data.position, _ui_manager.Framework.Context.UICamera, out var local_position);
                if (!Property.CloseWhenClickOutsideWindowRect.rect.Contains(local_position))
                {
                    if (Property.ClickOutsideWindowRectCallback == null || Property.ClickOutsideWindowRectCallback.Invoke())
                        Close();
                }
            }
        }
    }
}
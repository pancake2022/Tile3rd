using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using System;
using System.Collections.Generic;
using TMPro;

namespace CSFramework
{
    public abstract class BaseUI : CSBehaviour
    {
        protected UIManager _ui_manager;
        protected Dictionary<BaseUI, bool> _created_ui_dict;
        protected HashSet<int> _delay_call_id_set;
        protected bool _ignore_sound;
        protected ProtocolManager _protocol_manager { get { return _ui_manager.Framework.NetManager.ProtocolManager; } }
        protected LocalizeManager _localize_manager { get { return _ui_manager.Framework.LocalizeManager; } }

        public bool DontCleanup;
        public BaseUI RootUI { get; protected set; }
        public int SortingOrder { get; protected set; }

        private void OnEnable ()
        {
            if (_ui_manager == null) // special ui, not use CreateUI interface
            {
                var parent = transform.parent;
                while (parent)
                {
                    var ui = parent.GetComponent<BaseUI>();
                    if (ui && ui._ui_manager)
                    {
                        create_flow(ui._ui_manager);
                        break;
                    }
                    else
                    {
                        parent = parent.parent;
                    }
                }
            }
        }

        private void create_flow (UIManager ui_manager)
        {
            _ui_manager = ui_manager;
            _created_ui_dict = new Dictionary<BaseUI, bool>();
            _delay_call_id_set = new HashSet<int>();
            _ignore_sound = true;

            if (RootUI)
                refresh_child_node_order(RootUI.SortingOrder, RootUI == this);

            on_create();
            _ignore_sound = false;
            DontCleanup = false;
        }

        public static T Create<T> (Transform t, UIManager ui_manager, bool is_new_gameobject, BaseUI root_ui) where T : BaseUI
        {
            return Create(typeof(T), t, ui_manager, is_new_gameobject, root_ui) as T;
        }

        public static BaseUI Create (Type ui_type, Transform t, UIManager ui_manager, bool is_new_gameobject, BaseUI root_ui)
        {
            var old_active = t.gameObject.activeSelf;
            t.gameObject.SetActive(false);
            var ui = Utils.GetOrCreateComponent(ui_type, t) as BaseUI;
            ui.RootUI = root_ui ? root_ui : ui; // 设置RootUI，如果为空则设置为自己
            ui.create_flow(ui_manager);
            t.gameObject.SetActive(old_active);
            ui._ui_manager.OnCreateUICallback?.Invoke(ui, is_new_gameobject);
            return ui;
        }

        public static T Create<T> (string ui_prefab_path, Transform parent_t, UIManager ui_manager, BaseUI root_ui) where T : BaseUI
        {
            var ui_obj = ui_manager.LoadUIGameObject(ui_prefab_path, parent_t);
            return Create<T>(ui_obj.transform, ui_manager, true, root_ui);
        }

        public static T Create<T> (GameObject clone, Transform parent_t, UIManager ui_manager, BaseUI root_ui) where T : BaseUI
        {
            var ui_obj = GameObject.Instantiate(clone, parent_t);
            return Create<T>(ui_obj.transform, ui_manager, true, root_ui);
        }

        public static void Destroy (BaseUI ui, bool destroy_gameobject)
        {
            ui.on_destroy_previous();
            ui.on_destroy();
            if (destroy_gameobject)
            {
                ui._ui_manager.OnDestroyUI(ui);
                ui._ui_manager.Framework.GameObjectPoolManager.Recycle(ui.gameObject);
            }
        }

        protected virtual void on_create ()
        {

        }

        protected void on_destroy_previous ()
        {
            foreach (var item in _created_ui_dict)
                destroy_ui(item.Key, item.Value, false);
            _created_ui_dict.Clear();

            foreach (var delay_call_id in _delay_call_id_set)
                _ui_manager.CancelDelayCall(delay_call_id);
            _delay_call_id_set.Clear();
        }

        protected virtual void on_destroy ()
        {
            
        }

        protected T find_component<T> (string path, Component start_component, bool check_error = true) where T : Component
        {
            return find_component<T>(path, start_component ? start_component.transform : null, check_error);
        }

        protected T find_component<T> (string path, Transform start_transform = null, bool check_error = true) where T : Component
        {
            start_transform = start_transform ? start_transform : transform;
            var target_transform = start_transform.Find(path);
            if (target_transform)
            {
                var component = target_transform.GetComponent<T>();
                if (component)
                    return component;
                if (check_error)
                    Logger.Error(string.Format("find_component error, not found component [{0}] in [{1}] at [{2}]", typeof(T), path, target_transform));
                return null;
            }
            else
            {
                if (check_error)
                    Logger.Error(string.Format("find_component error, not found component [{0}] [{1}] in [{2}]", typeof(T), path, start_transform));
                return null;
            }
        }

        protected T create_ui<T> (string prefab_path, string parent_path) where T : BaseUI
        {
            return create_ui<T>(prefab_path, transform.Find(parent_path));
        }

        protected T create_ui<T> (string prefab_path, Transform parent_t) where T : BaseUI
        {
            return record_create(Create<T>(prefab_path, parent_t, _ui_manager, RootUI), true);
        }

        protected T create_ui<T> (string node_path) where T : BaseUI
        {
            if (string.IsNullOrEmpty(node_path))
                CSFramework.Logger.Error("create_ui error, node_path couldn't empty or null.");
            return record_create(create_ui<T>(transform.Find(node_path)), false);
        }

        protected T create_ui<T> (Transform t) where T : BaseUI
        {
            return record_create(Create<T>(t, _ui_manager, false, RootUI), false);
        }

        protected T create_ui<T> (GameObject clone, Transform parent_t) where T : BaseUI
        {
            return record_create(Create<T>(clone, parent_t, _ui_manager, RootUI), true);
        }

        protected T record_create<T> (T ui, bool destroy_gameobject) where T : BaseUI
        {
            _created_ui_dict[ui] = destroy_gameobject;
            return ui;
        }

        protected void destroy_ui (BaseUI ui, bool destroy_gameobject = true, bool remove_from_created_dict = true)
        {
            if (remove_from_created_dict)
                _created_ui_dict.Remove(ui);
            BaseUI.Destroy(ui, destroy_gameobject);
        }

        protected bool play_sound (string name, bool force_play = false, bool loop = false)
        {
            return play_sound(name, force_play, loop, out var sound_id);
        }

        protected bool play_sound (string name, bool force_play, bool loop, out int sound_id)
        {
            sound_id = -1;
            if (_ignore_sound && !force_play)
                return false;
            
            if (!string.IsNullOrEmpty(name))
                sound_id = _ui_manager.Framework.AudioManager.PlaySound(name, loop);
                
            return sound_id >= 0;
        }

        protected bool stop_sound (int sound_id)
        {
            return _ui_manager.Framework.AudioManager.StopSound(sound_id);
        }

        protected Button register_button (UnityAction callback, bool check_error = true, bool is_play_sound = true)
        {
            var button = GetComponent<Button>();
            if (button)
            {
                register_button(button, callback, is_play_sound);
            }
            else
            {
                if (check_error)
                    Logger.Error(string.Format("register_button error, not found button in [{0}]", this));
            }
            return button;
        }

        protected Button register_button (string path, UnityAction callback, bool check_error = true, bool is_play_sound = true)
        {
            return register_button(path, transform, callback, check_error, is_play_sound);
        }

        protected Button register_button (string path, Transform start_transform, UnityAction callback, bool check_error = true, bool is_play_sound = true)
        {
            var button_transform = start_transform.Find(path);
            if (button_transform)
            {
                var button = button_transform.GetComponent<Button>();
                register_button(button, callback, is_play_sound);
                return button;
            }
            else
            {
                if (check_error)
                    Logger.Error(string.Format("register_button error, not found [{0}] in [{1}]", path, this));
                return null;
            }
        }

        protected Button register_button (Button button, UnityAction callback, bool is_play_sound = true)
        {
            ulong last_trigger_timestamp = 0;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => 
            {
                var current_timestamp = Utils.CurrentTimestamp();
                var pass_time = (current_timestamp - last_trigger_timestamp) / 1000.0f;
                if (pass_time < _ui_manager.UIContext.ButtonContinuousTriggerInterval)
                    return;

                last_trigger_timestamp = current_timestamp;
                add_bi(BI_UIEventType.ButtonClicked, button);
                if (is_play_sound)
                    play_sound(_ui_manager.UIContext.AudioContext.ButtonClicked);
                callback?.Invoke();
            });
            return button;
        }

        protected Toggle register_toggle (UnityAction<bool> callback, bool check_error = true)
        {
            return register_toggle(null, transform, callback, check_error);
        }

        protected Toggle register_toggle (Transform start_transform, UnityAction<bool> callback, bool check_error = true)
        {
            return register_toggle(null, start_transform, callback, check_error);
        }

        protected Toggle register_toggle (string path, UnityAction<bool> callback, bool check_error = true)
        {
            return register_toggle(path, transform, callback, check_error);
        }

        protected Toggle register_toggle (string path, Transform start_transform, UnityAction<bool> callback, bool check_error = true)
        {
            var toggle_transform = string.IsNullOrEmpty(path) ? start_transform : start_transform.Find(path);
            if (toggle_transform)
            {
                var toggle = toggle_transform.GetComponent<Toggle>();
                return register_toggle(toggle, callback);
            }
            else
            {
                if (check_error)
                    Logger.Error(string.Format("register_toggle error, not found [{0}] in [{1}]", path, this));
                return null;
            }
        }

        protected Toggle register_toggle (Toggle toggle, UnityAction<bool> callback)
        {
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(selected => 
            {
                if (selected)
                {
                    add_bi(BI_UIEventType.ToggleChanged, toggle);
                    play_sound(_ui_manager.UIContext.AudioContext.ToggleSelected);
                }
                callback?.Invoke(selected);
            });
            return toggle;
        }

        protected Slider register_slider (string path, UnityAction<float> callback, bool check_error = true)
        {
            var component = find_component<Slider>(path, check_error: check_error);
            if (component)
            {
                component.onValueChanged.RemoveAllListeners();
                component.onValueChanged.AddListener(callback);
            }
            return null;
        }

        protected void register_trigger (EventTrigger trigger, EventTriggerType trigger_type, UnityAction<BaseEventData> callback)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = trigger_type;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }

        protected Dropdown register_dropdown (string path, UnityAction<int> callback, bool check_error = true)
        {
            var component = find_component<Dropdown>(path, check_error: check_error);
            if (component)
            {
                component.onValueChanged.RemoveAllListeners();
                component.onValueChanged.AddListener(callback);
            }
            return component;
        }

        protected TMP_Dropdown register_tmp_dropdown (string path, UnityAction<int> callback, bool check_error = true)
        {
            var component = find_component<TMP_Dropdown>(path, check_error: check_error);
            if (component)
            {
                component.onValueChanged.RemoveAllListeners();
                component.onValueChanged.AddListener(callback);
            }
            return component;
        }

        protected string simple_path (Component t)
        {
            if (!t) return "";

            var path = t.name;
            var parent = t.transform.parent;
            var path_depth = 0;
            var max_path_depth = 1;
            while (parent)
            {
                if (parent.GetComponent<WindowUI>())
                {
                    path = string.Format("{0}/{1}", parent.name, path);
                    break;
                }
                else
                {
                    if (path_depth < max_path_depth)
                    {
                        path = string.Format("{0}/{1}", parent.name, path);
                        ++path_depth;
                    }
                    parent = parent.parent;
                }
            }
            return path;
        }

        protected string relative_path (Component t)
        {
            var path = t.name;
            var parent = t.transform.parent;
            while (parent)
            {
                path = string.Format("{0}/{1}", parent.name, path);
                if (parent.GetComponent<WindowUI>())
                    break;
                else
                    parent = parent.parent;
            }
            return path;
        }

        protected bool add_bi (BI_UIEventType ui_event_type, Component target_ui)
        {
            var bi_manager = subsystem<BIManager>();
            if (bi_manager != null)
            {
                Utils.SafeCall(() => 
                    bi_manager.AddBI(new BI
                    {
                        BIType = BIType.UIEvent,
                        EventType = ui_event_type.ToString(),
                        Target = simple_path(target_ui),
                    }));
                return true;
            }
            return false;
        }
        
        protected Sprite find_sprite (string sprite_atlas_name, string sprite_name, bool warning = true)
        {
            return _ui_manager.FindSprite(sprite_atlas_name, sprite_name, warning);
        }

        public void DestroySelf ()
        {
            RootUI.destroy_ui(this);
        }

        public virtual void Show ()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide ()
        {
            gameObject.SetActive(false);
        }

        public bool InShow ()
        {
            return gameObject.activeSelf;
        }

        protected void delay_call (Action callback, float delay_seconds)
        {
            delay_call(callback, (long)(delay_seconds * 1000));
        }

        protected void delay_call (Action callback, long delay_ms)
        {
            var delay_call_id = 0;
            delay_call_id = _ui_manager.DelayCall(() => 
            {
                _delay_call_id_set.Remove(delay_call_id);
                callback?.Invoke();
            }, delay_ms);
            _delay_call_id_set.Add(delay_call_id);
        }

        protected T subsystem<T> () where T: Subsystem
        {
            return _ui_manager.Framework.SubsystemContainer.Subsystem<T>();
        }

        protected void refresh_child_node_order (int sorting_order, bool contain_root = false)
        {
            var renderer_list = GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderer_list)
            {
                if (contain_root || renderer.gameObject != gameObject)
                    Utils.GetOrCreateComponent<SortingOrderTracker>(renderer).BindSortingOrder(sorting_order);
            }

            var canvas_list = GetComponentsInChildren<Canvas>(true);
            foreach (var canvas in canvas_list)
            {
                if (contain_root || canvas.gameObject != gameObject)
                    Utils.GetOrCreateComponent<SortingOrderTracker>(canvas).BindSortingOrder(sorting_order);
            }
        }

        public Vector2 WorldToCanvasPosition (RectTransform parent, Vector3 world)
        {
            var ui_camera = _ui_manager.Framework.Context.UICamera;
            //world to screen
            Vector2 screen_point = RectTransformUtility.WorldToScreenPoint(ui_camera, world);
            //screen to ui
            Vector2 position = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screen_point, ui_camera,out position);
            return position;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;

namespace CSFramework
{
    public abstract class BaseModule : CSBehaviour
    {
        public abstract IEnumerator Init (CSBehaviour main_module, params object[] param_list);
        public abstract IEnumerator Cleanup ();
        public abstract void Reset ();
        public abstract void SetEnable (bool enable);
        public abstract bool IsEnabled ();
        public abstract int DelayCall (Action callback, float escape_second);
        public abstract int DelayCall (Action callback, long escape_ms);
        public abstract bool CancelDelayCall (int id);

        public abstract void Tick (float dt);
        public abstract void MillisecondTick (long ms_dt);
    }

    public class Module<T> : BaseModule where T: CSBehaviour
    {
        protected class DelayCallData
        {
            public int ID;
            public Action Callback;
            public long DelayMillisecond;
        }

        protected T _main_module;
        protected Dictionary<Type, CSBehaviour> _submodule_dict = new Dictionary<Type, CSBehaviour>();
        protected bool _enable = false;
        protected Dictionary<int, DelayCallData> _delay_call_data_dict;
        protected int _newest_delay_call_id;
        
        public override IEnumerator Init (CSBehaviour main_module, params object[] param_list)
        {
            _main_module = main_module as T;
            _delay_call_data_dict = new Dictionary<int, DelayCallData>();
            _newest_delay_call_id = 0;

            yield return on_init(param_list);

            _enable = true;
        }
        
        public override IEnumerator Cleanup ()
        {
            _enable = false;

            foreach (var value in _submodule_dict.Values)
            {
                var sub_module = value as BaseModule;
                yield return sub_module.Cleanup();
            }
            _submodule_dict.Clear();

            yield return on_cleanup();
        }
        public override void Reset()
        {
            foreach (var value in _submodule_dict.Values)
            {
                var sub_module = value as BaseModule;
                sub_module.Reset();
            }

            on_reset();
        }

        public override void SetEnable (bool enable)
        {
            _enable = enable;
        }

        public override bool IsEnabled ()
        {
            return _enable;
        }

        public override int DelayCall (Action callback, float escape_second)
        {
            return DelayCall(callback, (long)(escape_second * 1000));
        }

        public override int DelayCall (Action callback, long delay_ms)
        {
            ++_newest_delay_call_id;
            _delay_call_data_dict[_newest_delay_call_id]= new DelayCallData
            {
                ID = _newest_delay_call_id,
                Callback = callback,
                DelayMillisecond = delay_ms,
            };
            return _newest_delay_call_id;
        }

        public override bool CancelDelayCall (int id)
        {
            return _delay_call_data_dict.Remove(id);
        }

        public void TickDrive (long ms_dt)
        {
            on_tick_drive(ms_dt);
        }

        protected virtual void on_tick_drive (long ms_dt)
        {
            Tick((float)ms_dt / 1000.0f);
            MillisecondTick(ms_dt);
        }

        public override void Tick (float dt)
        {
            if (_enable)
            {
                on_tick(dt);
                foreach (var value in _submodule_dict.Values)
                {
                    var sub_module = value as BaseModule;
                    if (sub_module.IsEnabled())
                        sub_module.Tick(dt);
                }
            }
        }

        public override void MillisecondTick (long ms_dt)
        {
            if (_enable)
            {
                if (_delay_call_data_dict.Count > 0)
                {
                    var delay_call_data_dict = _delay_call_data_dict;
                    _delay_call_data_dict = new Dictionary<int, DelayCallData>();
                    foreach (var delay_call_data in delay_call_data_dict.Values)
                    {
                        if (delay_call_data.DelayMillisecond < 0)
                        {
                            delay_call_data.Callback?.Invoke();
                        }
                        else
                        {
                            delay_call_data.DelayMillisecond -= ms_dt;
                            _delay_call_data_dict[delay_call_data.ID] = delay_call_data;
                        }
                    }
                }

                on_millisecond_tick(ms_dt);
                foreach (var value in _submodule_dict.Values)
                {
                    var sub_module = value as BaseModule;
                    if (sub_module.IsEnabled())
                        sub_module.MillisecondTick(ms_dt);
                }
            }
        }

        protected virtual IEnumerator on_init (params object[] param_list)
        {
            return null;
        }

        protected virtual IEnumerator on_cleanup ()
        {
            return null;
        }

        protected virtual void on_reset ()
        {

        }

        protected virtual void on_tick (float dt)
        {

        }

        protected virtual void on_millisecond_tick (long ms_dt)
        {

        }

        protected IEnumerator register_submodule<ST> (params object[] param_list) where ST: CSBehaviour
        {
            var sub_module = Create<ST>();
            var sub_module_type = typeof(ST);
            _submodule_dict.Add(sub_module_type, sub_module);
            yield return (sub_module as BaseModule).Init(this, param_list);
        }

        protected IEnumerator silence_register_submodule<ST> (params object[] param_list) where ST: CSBehaviour
        {
            var sub_module = Create<ST>();
            var sub_module_type = typeof(ST);
            _submodule_dict.Add(sub_module_type, sub_module);
            var base_sub_module = sub_module as BaseModule;
            yield return base_sub_module.Init(this, param_list);
            base_sub_module.SetEnable(false);
        }

        protected IEnumerator deregister_submodule<ST> () where ST: CSBehaviour
        {
            var sub_module_type = typeof(ST);
            if (_submodule_dict.TryGetValue(sub_module_type, out var sub_module))
            {
                yield return (sub_module as BaseModule).Cleanup();
                _submodule_dict.Remove(sub_module_type);
            }
        }

        protected ST submodule<ST> () where ST: CSBehaviour
        {
            var submodule_type = typeof(ST);
            _submodule_dict.TryGetValue(submodule_type, out var sub_module);
            return sub_module as ST;
        }
    }
}
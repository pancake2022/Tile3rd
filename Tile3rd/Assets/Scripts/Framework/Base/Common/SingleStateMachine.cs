using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CSFramework
{
    public abstract class BaseSingleState : CSBehaviour
    {
        public abstract IEnumerator Enter (SingleStateMachine state_machine, params object[] param_list);
        public abstract IEnumerator Exit ();
        public abstract void Tick (float dt);
    }

    public abstract class SingleState<T> : BaseSingleState where T : CSBehaviour
    {
        protected SingleStateMachine _state_machine;
        protected T _entity;
        public override sealed IEnumerator Enter (SingleStateMachine state_machine, params object[] param_list)
        {
            Logger.Log(string.Format("{0} Enter", this.GetType()));
            _state_machine = state_machine;
            _entity = param_list[0] as T;
            yield return on_enter(param_list[1] as object[]);
        }

        public override sealed IEnumerator Exit ()
        {
            Logger.Log(string.Format("{0} Exit", this.GetType()));
            yield return on_exit();
            Destroy(this);
        }

        public override sealed void Tick (float dt)
        {
            on_tick(dt);
        }

        protected virtual IEnumerator on_enter (params object[] param_list) 
        {
            yield return null;
        }

        protected virtual IEnumerator on_exit () 
        {
            yield return null;
        }

        protected virtual void on_tick (float dt) {}
    }

    public class SingleStateMachine : Module<CSBehaviour>
    {
        public BaseSingleState CurrentState { get { return _current_state; } }
        private BaseSingleState _current_state = null;
        private bool _switch_lock = false;

        protected override IEnumerator on_init (params object[] param_list)
        {
            yield return null;
        }

        public void SwitchState<T> (params object[] param_list) where T : CSBehaviour
        {
            if (_switch_lock)
            {
                Logger.Error(string.Format("SingleStateMachine[{0}] in switch lock, can not switch to state[{1}]", _main_module.GetType(), typeof(T)));
            }
            else
            {
                _switch_lock = true;
                StartCoroutine(switch_state<T>(param_list));
            }
        }

        private IEnumerator switch_state<T> (params object[] param_list) where T : CSBehaviour
        {
            if (_current_state != null)
            {
                yield return _current_state.Exit();
                _current_state = null;
            }
            
            _current_state = _main_module.gameObject.AddComponent<T>() as BaseSingleState;
            _switch_lock = false;
            yield return _current_state.Enter(this, _main_module, param_list);
        }

        protected override void on_tick (float dt) 
        {
            if (_switch_lock)
                return;

            if (_current_state)
                _current_state.Tick(dt);
        }
    }
}
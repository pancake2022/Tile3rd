using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CSFramework
{
    public class EventManager : Module<Framework>
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private Dictionary<string, Action<BaseEvent>> _listener_dict = new Dictionary<string, Action<BaseEvent>>();
        private Queue<BaseEvent> _event_queue = new Queue<BaseEvent>();

        public void AddListener (string event_name, Action<BaseEvent> listener)
        {
            if (_listener_dict.TryGetValue(event_name, out var listener_action))
            {
                listener_action += listener;
            }
            else
            {
                _listener_dict[event_name] = listener;
            }
        }

        public void RemoveListener (string event_name, Action<BaseEvent> listener)
        {
            if (_listener_dict.ContainsKey(event_name))
            {
                _listener_dict[event_name] -= listener;
                if (_listener_dict[event_name] == null)
                {
                    _listener_dict.Remove(event_name);
                }
            }
        }

        public void DispatchEvent (string event_name, params object[] param_list)
        {
            _event_queue.Enqueue(new BaseEvent
            {
                Name = event_name,
                ParamList = param_list,
            });
        }

        protected override void on_tick (float dt)
        {
            if (_lock.TryEnterWriteLock(200))
            {
                try
                {
                    while (_event_queue.Count > 0)
                    {
                        var base_event = _event_queue.Dequeue();
                        if (_listener_dict.TryGetValue(base_event.Name, out var listener))
                        {
                            listener?.Invoke(base_event);
                        }
                    }
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
    }
}
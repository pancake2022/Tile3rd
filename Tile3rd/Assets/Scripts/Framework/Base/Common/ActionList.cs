using System;
using System.Collections.Generic;

namespace CSFramework
{
    public class ActionList<T>
    {
        public int Count { get { return _action_list.Count; } }
        private List<Action<T>> _action_list = new List<Action<T>>();

        public void Add (Action<T> action)
        {
            _action_list.Add(action);
        }

        public void Remove (Action<T> action)
        {
            for (var i = _action_list.Count - 1; i >= 0; --i)
            {
                if (_action_list[i] == action)
                    _action_list.RemoveAt(i);
            }
        }

        public void Invoke (T t)
        {
            for (var i = _action_list.Count - 1; i >= 0; --i)
            {
                if (i < _action_list.Count)
                {
                    var action = _action_list[i];
                        action.Invoke(t);
                }
            }
        }
    }
}
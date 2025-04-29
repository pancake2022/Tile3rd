using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

namespace CSFramework
{
    public abstract class ListenableData<TClass> where TClass : ListenableData<TClass>
    {
        private ActionList<ListenableData<TClass>> _listener = new ActionList<ListenableData<TClass>>();
        private Dictionary<string, object> _property_listener_dict = new Dictionary<string, object>();
        private Dictionary<string, object> _dirty_property_dict = new Dictionary<string, object>();
        private bool _dirty;

        public void AddListener (Action<ListenableData<TClass>> listener)
        {
            _listener.Add(listener);
        }

        public void RemoveListener (Action<ListenableData<TClass>> listener)
        {
            _listener.Remove(listener);
        }

        public void AddPropertyListener<T> (Expression<Func<T>> property_expression, Action<T> listener)
        {
            var property_name = (property_expression.Body as MemberExpression).Member.Name;
            if (!_property_listener_dict.TryGetValue(property_name, out var listener_action_list))
            {
                listener_action_list = new ActionList<T>();
                _property_listener_dict[property_name] = listener_action_list;
            }
            ((ActionList<T>)listener_action_list).Add(listener);
        }

        public void RemovePropertyListener<T> (Expression<Func<T>> property_expression, Action<T> listener)
        {
            var property_name = (property_expression.Body as MemberExpression).Member.Name;
            if (_property_listener_dict.TryGetValue(property_name, out var listener_action_list))
            {
                var action_list = ((ActionList<T>)listener_action_list);
                action_list.Remove(listener);
                if (action_list.Count == 0)
                    _property_listener_dict.Remove(property_name);
            }
        }

        public void NotifyPropertyChanged<T> (Expression<Func<T>> property_expression)
        {
            if (check_property(property_expression, out var property_name, out var property_value))
            {
                _dirty_property_dict[property_name] = property_value;
                _dirty = true;
            }
        }

        protected bool check_property<T> (Expression<Func<T>> property_expression, out string property_name, out object property_value)
        {
            var member = (property_expression.Body as MemberExpression).Member;
            property_name = member.Name;

            var property_info = member as PropertyInfo;
            if (property_info != null)
            {
                property_value = property_info.GetValue(this);
                return true;
            }
            else
            {
                property_value = null;
                Logger.Error(string.Format("{0} PropertyName {1} invalid.", GetType(), property_name));
                return false;
            }
        }

        public void Tick (float dt)
        {
            if (_dirty)
            {
                if (_dirty_property_dict.Count > 0)
                {
                    var temp_dict = _dirty_property_dict;
                    _dirty_property_dict = new Dictionary<string, object>();

                    foreach (var item in temp_dict)
                    {
                        if (_property_listener_dict.TryGetValue(item.Key, out var listener_action_list))
                        {
                            listener_action_list.GetType().GetMethod("Invoke").Invoke(listener_action_list, new object[] { item.Value });
                        }
                    }
                }

                _listener.Invoke(this);
            }
        }
    }
}
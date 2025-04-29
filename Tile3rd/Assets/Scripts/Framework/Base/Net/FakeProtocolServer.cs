using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using Google.Protobuf;

namespace CSFramework
{
    public abstract class FakeProtocolServer : Module<ProtocolManager>
    {
        public Framework Framework { get { return _main_module.Framework; }}
        protected Dictionary<Type, object> _protocol_listener = null;
        protected Queue<IMessage> _received_protocol_queue;
        protected Dictionary<Type, ProtocolWaitData> _protocol_wait_data_dict;

        protected override IEnumerator on_init (params object[] param_list)
        {
            _protocol_listener = new Dictionary<Type, object>();
            _received_protocol_queue = new Queue<IMessage>();
            _protocol_wait_data_dict = new Dictionary<Type, ProtocolWaitData>();
            yield return null;
        }

        public abstract void OnRegisterProtocolReceiver ();

        protected override void on_tick(float dt)
        {
            var received_protocol_dict = new Dictionary<Type, IMessage>();
            lock (_received_protocol_queue)
            {
                while (_received_protocol_queue.Count > 0)
                {
                    var protocol = _received_protocol_queue.Dequeue();
                    var protocol_type = protocol.GetType();
                    received_protocol_dict[protocol_type] = protocol;

                    if (_protocol_listener.TryGetValue(protocol_type, out var listener) && listener != null)
                        listener.GetType().GetMethod("Invoke").Invoke(listener, new object[] { protocol });
                    else if (!_protocol_wait_data_dict.ContainsKey(protocol_type))
                        CSFramework.Logger.Warning(string.Format("ProtocolManager warning, protocol: {0} not found listener", protocol_type));
                }
            }

            // check wait protocol
            var timeout_protocol_type_list = new List<Type>();
            var finished_wait_data_dict = new Dictionary<Type, ProtocolWaitData>();
            foreach (var item in _protocol_wait_data_dict)
            {
                var protocol_wait_data = item.Value;
                if (received_protocol_dict.TryGetValue(item.Key, out var protocol))
                {
                    protocol_wait_data.ReceivedProtocol = protocol;
                    finished_wait_data_dict[item.Key] = protocol_wait_data;
                }
                else
                {
                    protocol_wait_data.EscapeTime += dt;
                    if (protocol_wait_data.EscapeTime >= _main_module.ProtocolWaitTime)
                        timeout_protocol_type_list.Add(item.Key);
                }
            }
            // remove time out wait data
            foreach (var timeout_protocol_type in timeout_protocol_type_list)
            {
                var data = _protocol_wait_data_dict[timeout_protocol_type];
                _protocol_wait_data_dict.Remove(timeout_protocol_type);
                data.OnFailed?.Invoke();
            }
            // finish wait data
            foreach (var item in finished_wait_data_dict)
            {
                _protocol_wait_data_dict.Remove(item.Key);
                var protocol_wait_data = item.Value;
                protocol_wait_data.OnSuccess.GetType().GetMethod("Invoke").Invoke(protocol_wait_data.OnSuccess, new object[] { protocol_wait_data.ReceivedProtocol });
            }
        }

        public void OnReceivedProtocol (IMessage protocol)
        {
            lock (_received_protocol_queue)
                _received_protocol_queue.Enqueue(protocol);
        }

        public void RegisterProtocolListener<T> (Action<T> callback) where T : IMessage
        {
            var type = typeof(T);
            if (_protocol_listener.TryGetValue(type, out var listener) && listener != null)
                _protocol_listener[type] = callback + (Action<T>)listener;
            else
                _protocol_listener[type] = callback;
        }

        public void DeregisterProtocolListener<T> (Action<T> callback) where T : IMessage
        {
            var type = typeof(T);
            if (_protocol_listener.TryGetValue(type, out var listener) && listener != null)
                _protocol_listener[type] = (Action<T>)listener - callback;
        }

        public void Send (IMessage protocol)
        {
            // Clone
            var clone_func = protocol.GetType().GetMethod("Clone", new Type[] {});
            var clone_protocol = (IMessage)clone_func?.Invoke(protocol, new object[] {});
            _main_module.OnReceivedProtocol(clone_protocol);
        }

        public void Send<TWaitProtocol> (IMessage protocol, Action<TWaitProtocol> on_success = null, Action on_failed = null)
        {
            AddWaitProtocol(on_success, on_failed);
            Send(protocol);
        }

        public void AddWaitProtocol<TWaitProtocol> (Action<TWaitProtocol> on_success = null, Action on_failed = null)
        {
            _protocol_wait_data_dict[typeof(TWaitProtocol)] = new ProtocolWaitData { OnSuccess = on_success, OnFailed = on_failed };
        }
    }
}
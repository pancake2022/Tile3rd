using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Google.Protobuf;
using System.Linq;

namespace CSFramework
{
    public class ProtocolWaitData
    {
        public object OnSuccess;
        public Action OnFailed;
        public float EscapeTime = 0;
        public IMessage ReceivedProtocol;
    }

    public class ProtocolManager : Module<NetManager>
    {
        public Framework Framework { get { return _main_module.Framework; }}
        public HashSet<Type> LogExceptProtocolType;

        public const int ProtocolIDSize = 2; // sizeof(UInt16)
        public string TokenPropertyName = null;
        public string InsertToken = null;
        public float ProtocolWaitTime;

        protected Dictionary<UInt16, Type> _id_to_type_dict = null;
        protected Dictionary<Type, UInt16> _type_to_id_dict = null;
        protected Dictionary<Type, object> _protocol_listener = null;
        protected Queue<IMessage> _received_protocol_queue;
        protected Dictionary<Type, ProtocolWaitData> _protocol_wait_data_dict;
        protected FakeProtocolServer _fake_protocol_server = null;

        protected override IEnumerator on_init (params object[] param_list)
        {
            _id_to_type_dict = new Dictionary<ushort, Type>();
            _type_to_id_dict = new Dictionary<Type, ushort>();
            _protocol_listener = new Dictionary<Type, object>();
            _received_protocol_queue = new Queue<IMessage>();
            _protocol_wait_data_dict = new Dictionary<Type, ProtocolWaitData>();
            
            var framework = _main_module.Framework;
            ProtocolWaitTime = framework.Context.ProtocolWaitTime;
            if (!string.IsNullOrEmpty(framework.Context.ProtocolMapConfigPath))
            {
                var text_asset = framework.ResourcesManager.LoadResource<TextAsset>(framework.Context.ProtocolMapConfigPath);
                if (text_asset)
                {
                    var name_to_id = JsonConvert.DeserializeObject<Dictionary<string, UInt16>>(text_asset.text);
                    foreach (var item in name_to_id)
                    {
                        var protocol_id = item.Value;
                        var protocol_name = string.Format("CSFramework.Protocol.{0}", item.Key);
                        var protocol_type = Type.GetType(protocol_name);
                        if (protocol_type != null) // old protocol may deleted
                        {
                            _id_to_type_dict[protocol_id] = protocol_type;
                            _type_to_id_dict[protocol_type] = protocol_id;
                        }
                    }
                }
                else 
                {
                    Logger.Error(string.Format("ProtocolManager on_init: protocol [{0}] load failed", framework.Context.ProtocolMapConfigPath));
                }
            }
            yield return null;
        }

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
                    if (protocol_wait_data.EscapeTime >= ProtocolWaitTime)
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

        public void OnReceivedBuffers (byte[] buffers)
        {
            var protocol_id = BitConverter.ToUInt16(buffers, 0);
            if (_id_to_type_dict.TryGetValue(protocol_id, out var protocol_type))
            {
                // todo decrypt
                var property_info = protocol_type.GetProperty("Parser");
                var obj = property_info.GetValue(null, null);
                var parse_func = property_info.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]), typeof(int), typeof(int)});
                var protocol = (IMessage)parse_func?.Invoke(obj, new object[] { buffers, ProtocolIDSize, buffers.Length - ProtocolIDSize });

#if DEBUG || DEVELOPMENT_BUILD
                if (LogExceptProtocolType == null || !LogExceptProtocolType.Contains(protocol.GetType()))
                    CSFramework.Logger.Log(string.Format("ProtocolManager.OnReceivedBuffers receive protocol [{0}]:\n {1}", protocol_type, protocol.ToString()));
#endif

                OnReceivedProtocol(protocol);
            }
            else
            {
                CSFramework.Logger.Error(string.Format("ProtocolManager.OnReceivedBuffers error, not found protocol: {0}", protocol_id));
            }
        }

        public void OnReceivedProtocol (IMessage protocol)
        {
            lock (_received_protocol_queue)
                _received_protocol_queue.Enqueue(protocol);
        }

        public void Send (IMessage protocol, WebSocketConnection connection = null)
        {
            try_insert_token(protocol);
            if (_fake_protocol_server != null && _fake_protocol_server.IsEnabled())
            {
                _fake_protocol_server.OnReceivedProtocol(protocol);
            }
            else
            {
                if (try_convert_to_byte_array(protocol, out var byte_array))
                {
                    _main_module.SendWebSocketBuffers(byte_array, (result) => 
                    {
#if DEBUG || DEVELOPMENT_BUILD
                        if (LogExceptProtocolType == null || !LogExceptProtocolType.Contains(protocol.GetType()))
                            Logger.Log(string.Format("Protocol[{0}] send {1}", protocol.GetType(), result));
#endif
                    }, connection);
                }
            }
        }

        public void Send<TWaitProtocol> (IMessage protocol, Action<TWaitProtocol> on_success = null, Action on_failed = null, WebSocketConnection connection = null)
        {
            _protocol_wait_data_dict[typeof(TWaitProtocol)] = new ProtocolWaitData { OnSuccess = on_success, OnFailed = on_failed };
            Send(protocol, connection);
        }

        public void SendByUDP (IMessage protocol)
        {
            try_insert_token(protocol);

            if (try_convert_to_byte_array(protocol, out var byte_array))
            {
                _main_module.SendUDPBuffers(byte_array);
            }
        }

        private bool try_insert_token (IMessage protocol)
        {
            if (!string.IsNullOrEmpty(TokenPropertyName) && !string.IsNullOrEmpty(InsertToken))
            {
                var property_result = protocol.GetType().GetProperties().Where(p => p.Name == TokenPropertyName);
                if (property_result.Count<PropertyInfo>() > 0)
                {
                    property_result.First<PropertyInfo>().SetValue(protocol, InsertToken);
                    return true;
                }
            }
            return false;
        }

        private bool try_convert_to_byte_array (IMessage protocol, out byte[] byte_array)
        {
            var protocol_type = protocol.GetType();
            if (_type_to_id_dict.TryGetValue(protocol_type, out var protocol_id))
            {
                var head = BitConverter.GetBytes(protocol_id);
                var content = protocol.ToByteArray();
                byte_array = new byte[head.Length + content.Length];
                Array.Copy(head, byte_array, head.Length);
                Array.Copy(content, 0, byte_array, head.Length, content.Length);
                return true;
            }
            else
            {
                CSFramework.Logger.Error(string.Format("ProtocolManager.Send error, not found protocol: {0}", protocol_type));
                byte_array = null;
                return false;
            }
        }

        #region FakeProtocolServer
        public IEnumerator ConnectFakeProtocolServer<T> () where T : FakeProtocolServer
        {
            yield return register_submodule<T>();
            _fake_protocol_server = submodule<T>();
            _fake_protocol_server.OnRegisterProtocolReceiver();
        }

        public IEnumerator DisconnectFakeProtocolServer<T> () where T : FakeProtocolServer
        {
            yield return deregister_submodule<T>();
            _fake_protocol_server = null;
        }

        public T GetFakeProtocolServer<T> () where T : FakeProtocolServer
        {
            return _fake_protocol_server as T;
        }
        #endregion
    }
}
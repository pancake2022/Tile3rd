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
    public class WebSocketConnectConfig
    {
        public string Address;
        public Action<string> OnWebSocketOpen;
        public Action<string> OnWebSocketError;
        public Action<string> OnWebSocketClose;
        public long HeartbeatInterval; // 毫秒
        public IMessage HeartbeatProtocol; // 心跳协议
    }
    public class WebSocketConnection
    {
        public WebSocketConnectConfig Config;
        public long HeartbeatEscapeTime;
        public WebSocket WebSocket;
    }

    public class NetManager : Module<Framework>
    {

        private enum UdpClientState
        {
            None,
            Receiving,
            WaitingStop,
        }

        public Framework Framework { get { return _main_module; }}
        public ProtocolManager ProtocolManager { get { return submodule<ProtocolManager>(); }}
        public WebSocketConnection DefaultConnection;

        private IPEndPoint _ip_end_point = null;
        private UdpClient _udp_client = null;
        private UdpClientState _udp_client_state;
        private List<WebSocketConnection> _ws_connection_list;
        private Queue<Tuple<Action<string>, string>> _ws_status_queue;

        protected override IEnumerator on_init(params object[] param_list)
        {
            _udp_client_state = UdpClientState.None;
            _udp_client = new UdpClient();
            _ws_connection_list = new List<WebSocketConnection>();
            _ws_status_queue = new Queue<Tuple<Action<string>, string>>();
            yield return register_submodule<ProtocolManager>();
        }

        protected override void on_millisecond_tick (long ms_dt)
        {
            lock (_ws_status_queue)
            {
                while (_ws_status_queue.Count > 0)
                {
                    var ws_status = _ws_status_queue.Dequeue();
                    ws_status.Item1?.Invoke(ws_status.Item2);
                }
            }

            foreach (var ws_connection in _ws_connection_list)
            {
                if (ws_connection.Config.HeartbeatProtocol != null)
                {
                    ws_connection.HeartbeatEscapeTime += ms_dt;
                    if (ws_connection.HeartbeatEscapeTime >= ws_connection.Config.HeartbeatInterval)
                    {
                        ws_connection.HeartbeatEscapeTime = 0;
                        ProtocolManager.Send(ws_connection.Config.HeartbeatProtocol);
                    }
                }
            }
        }

        #region WebSocket
        public WebSocketConnection ConnectWebSocket (WebSocketConnectConfig config)
        {
            var connection = new WebSocketConnection
            {
                Config = config,
            };
            connection.WebSocket = new WebSocket(config.Address);

            connection.WebSocket.OnOpen += (object sender, EventArgs e) => 
            {
                Logger.Log(string.Format("ConnectWebSocket [{0}] OnWebSocketOpen", config.Address));
                lock(_ws_status_queue)
                    _ws_status_queue.Enqueue(new Tuple<Action<string>, string>(config.OnWebSocketOpen, null));
            };
            connection.WebSocket.OnClose += (object sender, CloseEventArgs e) => 
            {
                Logger.Log(string.Format("ConnectWebSocket [{0}] OnWebSocketClose: {1}", config.Address, e.Reason));
                lock(_ws_status_queue)
                    _ws_status_queue.Enqueue(new Tuple<Action<string>, string>(config.OnWebSocketClose, e.Reason));
            };
            connection.WebSocket.OnError += (object sender, ErrorEventArgs e) => 
            {
                Logger.Error(string.Format("ConnectWebSocket [{0}] OnWebSocketError: {1}, Exception: {2}", config.Address, e.Message, e.Exception));
                lock(_ws_status_queue)
                    _ws_status_queue.Enqueue(new Tuple<Action<string>, string>(config.OnWebSocketError, e.Message));
            };
            connection.WebSocket.OnMessage += (object sender, MessageEventArgs e) => 
            {
                ProtocolManager.OnReceivedBuffers(e.RawData);
            };

            _ws_connection_list.Add(connection);

            connection.WebSocket.Connect();

            return connection;
        }

        public void RetryConnectWebSocket (WebSocketConnection connection)
        {
            connection.WebSocket.Connect();
        }

        public void CloseWebSocket (WebSocketConnection connection)
        {
            _ws_connection_list.Remove(connection);
            connection.WebSocket.Close();
        }

        public void SendWebSocketBuffers (byte[] buffers, Action<bool> callback, WebSocketConnection connection = null)
        {
            if (connection == null)
                connection = DefaultConnection;
                
            if (connection != null)
            {
                if (connection.WebSocket.ReadyState == WebSocketState.Open)
                {
                    connection.WebSocket.SendAsync(buffers, callback);
                }
                else
                {
                    connection.Config.OnWebSocketError?.Invoke("WebSocket Is Not Connected : " + connection.WebSocket.ReadyState);
                    // Logger.Error("SendWebSocketBuffers Error, WebSocket Is Not Connected : " + _ws.ReadyState);
                }
            }
            else
            {
                Logger.Error("SendWebSocketBuffers Error, WebSocket Was Null");
            }
        }
        #endregion

        #region UDP
        public void StartListeningUDP (string address, int port)
        {
            StartListeningUDP(IPAddress.Parse(address), port);
        }

        public void StartListeningUDP (IPAddress address, int port)
        {
            _ip_end_point = new IPEndPoint(address, port);
            _udp_client_state = UdpClientState.Receiving;
            start_receive_udp();
        }

        public void StopListeningUDP ()
        {
            if (_udp_client_state == UdpClientState.Receiving)
                _udp_client_state = UdpClientState.WaitingStop;
        }

        protected async void start_receive_udp ()
        {
            var client = _udp_client;
            var task = client.ReceiveAsync();
            var result = await task;

            ProtocolManager.OnReceivedBuffers(result.Buffer);

            if (_udp_client_state == UdpClientState.Receiving)
            {
                if (_udp_client == client)
                    start_receive_udp();
            }
            else if (_udp_client_state == UdpClientState.WaitingStop)
            {
                _udp_client_state = UdpClientState.None;
            }
        }

        public void SendUDPMessage (string content)
        {
            if (_udp_client != null)
            {
                var buffers = Encoding.UTF8.GetBytes(content);
                _udp_client.Send(buffers, buffers.Length, _ip_end_point);
            }
            else
            {
                CSFramework.Logger.Error("NetManager.Send error, UDP not init");
            }
        }

        public void SendUDPBuffers (byte[] buffers)
        {
            if (_udp_client != null)
            {
                _udp_client.Send(buffers, buffers.Length, _ip_end_point);
            }
            else
            {
                CSFramework.Logger.Error("NetManager.Send error, UDP not init");
            }
        }
        #endregion
    }
}
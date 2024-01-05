using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pong.Unity.Scenes
{
    public class ServerClient
    {
        Uri _server_uri = new("ws://185.107.52.99/ws");

        WebSocket _webSocket;

        public ServerClient()
        {
        }

        public ServerClient(WebSocket webSocket)
        {
            _webSocket = webSocket;
        }

        public async Task ConnectAsync(Uri uri = null)
        {
            uri ??= _server_uri;
            var webSocket = new ClientWebSocket();
            try
            {
                //Debug.Log($"Try to connect to Server: {_server_uri}");
                _ = webSocket.ConnectAsync(uri, CancellationToken.None);
                while (_webSocket.State == WebSocketState.Connecting)
                {
                    //Debug.Log("Waiting to connect...");
                    await Task.Delay(500);
                }
                //Debug.Log($"Connect to Server. State: {_webSocket.State}");
                _webSocket = webSocket;
            }
            catch (Exception ex)
            {
                //Debug.Log($"Exception: {ex.Message}");
            }
        }

        public Task SendAsync(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            if (_webSocket.State == WebSocketState.Open)
            {
                _webSocket.SendAsync(new ArraySegment<byte>(dataBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            return Task.CompletedTask;
        }

        public async Task<string> ReceiveAsync()
        {
            byte[] buffer = new byte[1024 * 4];
            if (_webSocket.State == WebSocketState.Open)
            {
                var webSocketReceiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                return Encoding.UTF8.GetString(buffer)[..webSocketReceiveResult.Count];
            }
            return null;
        }
    }
}
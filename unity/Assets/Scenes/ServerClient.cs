using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;

namespace Pong.Unity.Scenes
{
    public class ServerClient
    {
        string _server_uri = "ws://185.107.52.99:80/ws";

        public WebSocket WebSocket;

        public bool IsAlive => WebSocket.IsAlive;

        public ServerClient()
        {
        }

        public ServerClient(WebSocket webSocket)
        {
            WebSocket = webSocket;
        }

        public void Connect(Uri uri = null)
        {
            var webSocket = new WebSocket(_server_uri);
            try
            {
                Debug.Log($"Try to connect to Server: {uri}");
                webSocket.Connect();
                Debug.Log($"Connect to Server.");
                WebSocket = webSocket;
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception: {ex.Message}");
            }
        }

        public void Send(string data)
        {
            WebSocket.Send(data);
        }

        internal Task<string> ReceiveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
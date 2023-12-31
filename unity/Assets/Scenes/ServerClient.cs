using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Pong.Unity.Scenes
{
    public class ServerClient
    {
        public Socket? Socket;

        public void Connect()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry("185.107.52.99");
                IPAddress ipAddress = host.AddressList.FirstOrDefault(it => it.AddressFamily == AddressFamily.InterNetwork) ?? host.AddressList.First();
                var remoteEP = new IPEndPoint(ipAddress, 11000);
                Socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                Socket.Connect(remoteEP);
            }
            catch (Exception)
            {
            }
        }

        public void Send(string data)
        {
            if (Socket == null) return;

            SocketAsyncEventArgs socketAsyncData = new();
            socketAsyncData.SetBuffer(Encoding.ASCII.GetBytes(data), 0, data.Length);
            Socket.SendAsync(socketAsyncData);
        }
    }
}
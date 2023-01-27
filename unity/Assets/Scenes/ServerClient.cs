using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

namespace Pong.Unity.Scenes
{
    public class ServerClient
    {
        public Socket? Socket;

        public void Connect()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry("100.64.2.99");
                IPAddress ipAddress = host.AddressList[0];
                var remoteEP = new IPEndPoint(ipAddress, 11000);
                Socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.

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
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Pong.Unity.Scenes
{
    public class ServerClient
    {
        byte[] _bytes = null;

        public Socket Socket;

        public void Connect()
        {
            _bytes = new byte[1024];
            try
            {
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

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
            SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
            socketAsyncData.SetBuffer(Encoding.ASCII.GetBytes(data), 0, data.Length);
            Socket.SendAsync(socketAsyncData);
        }
    }
}
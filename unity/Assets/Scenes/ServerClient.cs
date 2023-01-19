using System.Net;
using System.Net.Sockets;
using System.Text;

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
            var bytes = Encoding.ASCII.GetBytes(data);
            
            SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
            socketAsyncData.SetBuffer(bytes, 0, bytes.Length);
            Socket.SendAsync(socketAsyncData);
        }

    }
}
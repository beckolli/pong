using System.Net;
using System.Net.Sockets;

namespace Pong.Server
{
    public class SocketListener
    {
        Socket? _listener = null;
        public void StartServer()
        {
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            try
            {
                // Create a Socket that will use Tcp protocol
                _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // A Socket must be associated with an endpoint using the Bind method
                _listener.Bind(localEndPoint);
                // Specify how many requests a Socket can listen before it gives Server busy response.
                // We will listen 10 requests at a time
                _listener.Listen(1);

                Listen();

            }
            catch(Exception e)
            {
                if (_listener != null)
                {
                    _listener.Close();
                }       
            }
        }

        void Listen()
        {
            Console.WriteLine("Waiting for a connection...");
            var handler = _listener.Accept();
            var socketHolder = new SocketHolder(handler);
            Console.WriteLine("connecting...");
            new Task(() => socketHolder.ReadDataAndSendToOpponent()).Start();
            Listen();
        }
        
    }
}
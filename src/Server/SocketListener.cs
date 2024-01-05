using System.Net;
using System.Net.Sockets;
using Pong.Server.Extensions;
using Pong.Server.Models;

namespace Pong.Server
{
    public class SocketListener
    {
        Socket? _listener = null;
        readonly List<Game> _gameList = new();

        public void StartServer()
        {
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = host.AddressList.FirstOrDefault(it => it.AddressFamily == AddressFamily.InterNetwork) ?? host.AddressList.First();
            IPEndPoint localEndPoint = new(ipAddress, 11000);

            try
            {
                // Create a Socket that will use Tcp protocol
                _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // A Socket must be associated with an endpoint using the Bind method
                _listener.Bind(localEndPoint);
                // Specify how many requests a Socket can listen before it gives Server busy response.
                // We will listen 1 requests at a time
                _listener.Listen(1);
                foreach (var address in host.AddressList)
                {
                    Console.WriteLine("adress list entry: " + address);
                }
                Console.WriteLine("IpAdress: " + ipAddress);
                Console.WriteLine("LocalEndPoint: " + localEndPoint);

                Listen();

            }
            catch (Exception)
            {
                _listener?.Close();
            }
        }

        void Listen()
        {
            Console.WriteLine("Waiting for a connection...");
            if (_listener == null)
            {
                return;
            }
            var newPlayerSocket = _listener.Accept();

            Console.WriteLine("connecting...");
            Player player = new(newPlayerSocket);
            var lastOpenGame = _gameList.LastOrDefault(it => it.Player2 == null);
            if (lastOpenGame == null)
            {
                lastOpenGame = player.ConnectPlayerToGame(null);
                _gameList.Add(lastOpenGame);
            }
            else
            {
                player.ConnectPlayerToGame(lastOpenGame);
            }

            var socketHolder = new SocketHolder(lastOpenGame, player);
            new Task(() => socketHolder.ReadDataAndSendToOpponent()).Start();
            lastOpenGame?.SendGameStartMessage();

            Listen();
        }

    }
}
using System.Net;
using System.Net.Sockets;
using Pong.Server.Models;
using System.Text;
using System.Collections.Generic;

namespace Pong.Server
{
    public class SocketListener
    {
        Socket? _listener = null;
        readonly List<Game> _gameList = new();

        public string StartMessage = new("start");

        public void StartServer()
        {
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList.FirstOrDefault(it => it.AddressFamily == AddressFamily.InterNetwork) ?? host.AddressList.First();
            IPEndPoint localEndPoint = new(ipAddress, 11000);

            try
            {
                // Create a Socket that will use Tcp protocol
                _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // A Socket must be associated with an endpoint using the Bind method
                _listener.Bind(localEndPoint);
                // Specify how many requests a Socket can listen before it gives Server busy response.
                // We will listen 10 requests at a time
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
            var socket = _listener.Accept();

            Console.WriteLine("connecting...");
            Player player = new(socket);
            var lastOpenGame = _gameList.LastOrDefault(it => it.Player2 == null);
            if (lastOpenGame == null)
            {
                lastOpenGame = ConnectPlayerToGame(player, null);
                _gameList.Add(lastOpenGame);
            }
            else
            {
                ConnectPlayerToGame(player, lastOpenGame);
            }

            var socketHolder = new SocketHolder(lastOpenGame, player);
            new Task(() => socketHolder.ReadDataAndSendToOpponent()).Start();
            SendGameStartMessage(lastOpenGame);

            Listen();
        }

        void SendGameStartMessage(Game game)
        {
            if (game.Player2 != null)
            {
                byte[] startMessageBytes = Encoding.ASCII.GetBytes(StartMessage);
                game.Player1.Socket.Send(startMessageBytes);
                game.Player2.Socket.Send(startMessageBytes);
            }
        }

        public static Game ConnectPlayerToGame(Player player, Game? game)
        {
            var currentGame = game ?? new Game(player);
            if (game == null)
            {
                player.Name = "Player 1";
            }
            else
            {
                game.Player2 = player;
                player.Name = "Player 2";
            }
            return currentGame;
        }
    }
}
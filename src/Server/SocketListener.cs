using System.Net;
using System.Net.Sockets;
using Pong.Server.Models;
using System.Text;

namespace Pong.Server
{
    public class SocketListener
    {
        Game? _game;
        Socket? _listener = null;

        public string StartMessage = new("start");

        public void StartServer()
        {
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
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
            _game = ConnectPlayerToGame(player, _game);
            var socketHolder = new SocketHolder(_game, player);
            new Task(() => socketHolder.ReadDataAndSendToOpponent()).Start();
            SendGameStartMessage();

            Listen();
        }

        void SendGameStartMessage()
        {
            if (_game != null && _game.Player2 != null)
            {
                byte[] startMessageBytes = Encoding.ASCII.GetBytes(StartMessage);
                _game.Player1.Socket.Send(startMessageBytes);
                _game.Player2.Socket.Send(startMessageBytes);
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
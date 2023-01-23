using System.Net.Sockets;
using System.Text;
using Pong.Server.Models;

namespace Pong.Server
{
    public class SocketHolder
    {
        /// <summary>
        /// The socket for the player connection
        /// <param name = "game"></param>
        /// <param name = "player">the player of the socket</param>
        /// </summary>
        readonly Game _game;
        readonly Player _currentPlayer;

        public SocketHolder(Game game, Player currentPlayer)
        {
            _game = game;
            _currentPlayer = currentPlayer;
        }

        public void ReadDataAndSendToOpponent()
        {
            try
            {
                // Incoming data from the client.
                string data;
                byte[] bytes;

                while (true)
                {
                    Console.WriteLine("wait for data for currentPlayer: " + _currentPlayer.Name);
                    bytes = new byte[1024];
                    int bytesRec = _currentPlayer.Socket.Receive(bytes);
                    if (bytesRec == 0)
                    {
                        Disconnect();
                    }
                    data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    Console.WriteLine("Text received : {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    byte[] responseMessage = Encoding.ASCII.GetBytes(data);
                    // Send the response to the opposite player
                    if (_game.Player2 != null && _game.Player1 != null)
                    {
                        if (_currentPlayer == _game.Player1)
                        {
                            _game.Player2.Socket.Send(responseMessage);
                        }
                        else
                        {
                            _game.Player1.Socket.Send(responseMessage);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                if (_currentPlayer != null)
                {
                    Disconnect();
                }
            }
        }

        void Disconnect()
        {
            Console.WriteLine("disconnecting...");
            try
            {
                _currentPlayer.Socket.Shutdown(SocketShutdown.Send);
                _currentPlayer.Socket.Close();
            }
            catch (Exception) { }
        }
    }
}
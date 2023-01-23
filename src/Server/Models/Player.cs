using System.Net.Sockets;

namespace Pong.Server.Models
{
    public class Player
    {
        public Player(Socket socket)
        {
            Socket = socket;
        }
        public string? Name { get; set; }
        public Socket Socket { get; set; }
    }
}
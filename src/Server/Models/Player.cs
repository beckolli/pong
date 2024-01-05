using System.Net.Sockets;
using System.Net.WebSockets;

namespace Pong.Server.Models
{
    public class Player
    {

        public Player()
        {
            // for testing
        }

        public Player(Socket socket)
        {
            Socket = socket;
        }

        public Player(WebSocket webSocket)
        {
            WebSocket = webSocket;
        }

        public string? Name { get; set; }
        public Socket? Socket { get; set; }
        public WebSocket? WebSocket { get; set; }
    }
}
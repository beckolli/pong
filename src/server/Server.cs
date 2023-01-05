
namespace Pong.Server
{
    // Socket Listener acts as a server and listens to the incoming
    // messages on the specified port and protocol.
    public class PongServer
    {
        public static int Main(String[] args)
        {
            var socketListener  = new SocketListener();
            socketListener.StartServer();
            return 0;
        }
    }
}
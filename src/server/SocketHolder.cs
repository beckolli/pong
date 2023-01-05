using System.Net.Sockets;
using System.Text;

namespace Pong.Server
{
    public class SocketHolder
    {

        Socket _handler;

        public SocketHolder(Socket handler)
        {
            _handler = handler;
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
                    bytes = new byte[1024];
                    int bytesRec = _handler.Receive(bytes);
                    if (bytesRec == 0)
                    {
                        Disconnect();
                    }
                    Console.WriteLine("wait for data...");
                    data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    Console.WriteLine("Text received : {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    byte[] response_msg = Encoding.ASCII.GetBytes(data);
                    _handler.Send(response_msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                if (_handler != null)
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
                _handler.Shutdown(SocketShutdown.Send);
                _handler.Close();
            }
            catch (Exception) { }
        }
    }
}
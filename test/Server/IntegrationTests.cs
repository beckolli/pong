using System.Net.Sockets;
using System.Text;
using Pong.Unity.Scenes;
using Xunit;

namespace Pong.Server.Test
{
    public class IntegrationTests
    {
        [Fact]
        public void Server()
        {
            var socketListener = new SocketListener();
            new Task(() => socketListener.StartServer()).Start();
            Task.Delay(100).Wait();

            var player1ServerClient = new ServerClient();
            var player1DataReceiver = new DataReceiver();
            player1ServerClient.Connect();
            new Task(() => RecieveAsync(player1ServerClient.Socket, player1DataReceiver)).Start();

            var player2ServerClient = new ServerClient();
            var player2DataReceiver = new DataReceiver();
            player2ServerClient.Connect();
            new Task(() => RecieveAsync(player2ServerClient.Socket, player2DataReceiver)).Start();

            player1ServerClient.Send("player1 test");
            Task.Delay(100).Wait();
            Assert.Equal("player1 test", player2DataReceiver.Data);

            player2ServerClient.Send("player2 test");
            Task.Delay(100).Wait();
            Assert.Equal("player2 test", player1DataReceiver.Data);
        }

        static Task RecieveAsync(Socket socket, DataReceiver dataReceiver)
        {
            var bytes = new byte[1024];
            var bytesCount = socket.ReceiveAsync(bytes, SocketFlags.None).GetAwaiter().GetResult();
            dataReceiver.Data = Encoding.ASCII.GetString(bytes, 0, bytesCount);
            return Task.CompletedTask;
        }
    }

    public class DataReceiver
    {
        public string? Data { get; set; }
    }
}
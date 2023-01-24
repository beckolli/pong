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

            Task.Delay(100).Wait();
            Assert.Equal("start", player1DataReceiver.Data);
            Assert.Equal("start", player2DataReceiver.Data);

            player1ServerClient.Send("player1 to player2");
            Task.Delay(200).Wait();
            Assert.Equal("player1 to player2", player2DataReceiver.Data);

            player2ServerClient.Send("player2 to player1");
            Task.Delay(200).Wait();
            Assert.Equal("player2 to player1", player1DataReceiver.Data);

            var player3ServerClient = new ServerClient();
            var player3DataReceiver = new DataReceiver();
            player3ServerClient.Connect();
            new Task(() => RecieveAsync(player3ServerClient.Socket, player3DataReceiver)).Start();

            var player4ServerClient = new ServerClient();
            var player4DataReceiver = new DataReceiver();
            player4ServerClient.Connect();
            new Task(() => RecieveAsync(player4ServerClient.Socket, player4DataReceiver)).Start();

            Task.Delay(100).Wait();
            Assert.Equal("start", player3DataReceiver.Data);
            Assert.Equal("start", player4DataReceiver.Data);

            player3ServerClient.Send("player3 to player4");
            Task.Delay(200).Wait();
            Assert.Equal("player3 to player4", player4DataReceiver.Data);

            player4ServerClient.Send("player4 to player3");
            Task.Delay(200).Wait();
            Assert.Equal("player4 to player3", player3DataReceiver.Data);
        }

        static Task RecieveAsync(Socket? socket, DataReceiver dataReceiver)
        {
            if(socket == null) return Task.CompletedTask;
            var bytes = new byte[1024];
            var bytesCount = socket.ReceiveAsync(bytes, SocketFlags.None).GetAwaiter().GetResult();
            dataReceiver.Data = Encoding.ASCII.GetString(bytes, 0, bytesCount);
            // storage is running out of space because the data is never deleted only added
            RecieveAsync(socket, dataReceiver).Wait();
            return Task.CompletedTask;
        }
    }

    public class DataReceiver
    {
        public string? Data { get; set; }
    }
}
using System.Net.WebSockets;
using Microsoft.AspNetCore.TestHost;
using Pong.Unity.Scenes;
using Xunit;

namespace Pong.Server.Test
{
    public class IntegrationTests : IClassFixture<TestFixture>
    {
        readonly string _uri = "ws://localhost/ws";
        readonly WebSocketClient _webSocketClient;

        public IntegrationTests(TestFixture testFixture)
        {
            _webSocketClient = testFixture.WebSocketClient;
        }

        [Fact]
        public async Task Server()
        {
            var player1DataReceiver = new DataReceiver();
            var player1ServerClient = await ConnectNewPlayerAsync();
            new Task(() => RecieveAsync(player1ServerClient, player1DataReceiver).Wait()).Start();
            Task.Delay(200).Wait();

            var player2DataReceiver = new DataReceiver();
            var player2ServerClient = await ConnectNewPlayerAsync();
            new Task(() => RecieveAsync(player2ServerClient, player2DataReceiver).Wait()).Start();
            Task.Delay(200).Wait();

            Task.Delay(200).Wait();
            Assert.Contains("PlayerNumber\":1", player1DataReceiver.Data);
            Assert.Contains("PlayerNumber\":2", player2DataReceiver.Data);

            await player1ServerClient.SendAsync("player1 to player2");
            Task.Delay(200).Wait();
            Assert.Equal("player1 to player2", player2DataReceiver.Data);

            await player2ServerClient.SendAsync("player2 to player1");
            Task.Delay(200).Wait();
            Assert.Equal("player2 to player1", player1DataReceiver.Data);

            var player3DataReceiver = new DataReceiver();
            var player3ServerClient = await ConnectNewPlayerAsync();            
            new Task(() => RecieveAsync(player3ServerClient, player3DataReceiver).Wait()).Start();

            var player4DataReceiver = new DataReceiver();
            var player4ServerClient = await ConnectNewPlayerAsync();            
            new Task(() => RecieveAsync(player4ServerClient, player4DataReceiver).Wait()).Start();

            Task.Delay(200).Wait();
            Assert.Contains("PlayerNumber\":1", player3DataReceiver.Data);
            Assert.Contains("PlayerNumber\":2", player4DataReceiver.Data);

            await player3ServerClient.SendAsync("player3 to player4");
            Task.Delay(200).Wait();
            Assert.Equal("player3 to player4", player4DataReceiver.Data);

            await player4ServerClient.SendAsync("player4 to player3");
            Task.Delay(200).Wait();
            Assert.Equal("player4 to player3", player3DataReceiver.Data);
        }

        async Task<ServerClient> ConnectNewPlayerAsync()
        {
            WebSocket webSocket = await _webSocketClient.ConnectAsync(new Uri(_uri), CancellationToken.None);
            return new ServerClient(webSocket);
        }

        static async Task RecieveAsync(ServerClient serverClient, DataReceiver dataReceiver)
        {
            if (serverClient == null)
                return;
            dataReceiver.Data = await serverClient.ReceiveAsync();
            // storage is running out of space because the data is never deleted only added
            _ = RecieveAsync(serverClient, dataReceiver);
            return;
        }
    }

    public class DataReceiver
    {
        public string? Data { get; set; }
    }
}
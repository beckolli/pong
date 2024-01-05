using System.Net.WebSockets;
using Pong.Server.Extensions;
using Pong.Server.Models;

namespace Pong.Server;

/// <summary>
/// WebSocketServer implementation
/// </summary>
public class WsServer
{
    readonly List<Game> _gameList = [];

    public void ConnectClient(WebSocket webSocket)
    {
        Player player = new(webSocket);        
        var lastOpenGame = _gameList.LastOrDefault(it => it.Player2 == null);
        if (lastOpenGame == null)
        {
            lastOpenGame = player.ConnectPlayerToGame(null);
            _gameList.Add(lastOpenGame);
        }
        else
        {
            player.ConnectPlayerToGame(lastOpenGame);
        }
        Console.WriteLine($"{player.Name} added to game.");
        new WsConnectionHolder(lastOpenGame, player, webSocket).ReadDataAndSendToOpponent().Wait();
    }
}


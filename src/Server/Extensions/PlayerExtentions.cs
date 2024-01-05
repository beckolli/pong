using System.Text;
using Pong.Server.Models;

namespace Pong.Server.Extensions;

public static class PlayerExtentions
{
    public static Game ConnectPlayerToGame(this Player player, Game? game = null)
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


    public static Task SendAsync(this Player player, string message)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(message);

        if (player.WebSocket != null)
        {
            Console.WriteLine($"{player.Name} send data: {message}");
            player.WebSocket?.SendAsync(
                new ArraySegment<byte>(dataBytes, 0, dataBytes.Length),
                System.Net.WebSockets.WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        if (player.Socket != null)
        {
            player.Socket?.Send(dataBytes);
        }

        return Task.CompletedTask;
    }
}
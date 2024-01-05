using System.Text.Json;
using Pong.Server.Models;

namespace Pong.Server.Extensions;

public static class GameExtensions
{
    public static void SendGameStartMessage(this Game game)
    {
        if (game.Player2 != null)
        {
            game.Player1.SendAsync(JsonSerializer.Serialize(new GameStart(playerNumber: 1)));
            game.Player2.SendAsync(JsonSerializer.Serialize(new GameStart(playerNumber: 2)));
        }
    }
}
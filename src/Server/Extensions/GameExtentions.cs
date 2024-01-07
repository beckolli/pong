using System.Text.Json;
using Microsoft.VisualBasic;
using Pong.Server.Models;

namespace Pong.Server.Extensions;

public static class GameExtensions
{
    public static void SendGameStartMessage(this Game game)
    {
        var sendGameStartPlayer1 = new GameStart(playerNumber: 1);
        var sendGameStartPlayer2 = new GameStart(playerNumber: 2)
        {
            BallX = sendGameStartPlayer1.BallX,
            BallY = sendGameStartPlayer1.BallY
        };
        if (game.Player2 != null)
        {
            game.Player1.SendAsync(JsonSerializer.Serialize(sendGameStartPlayer1));
            game.Player2.SendAsync(JsonSerializer.Serialize(sendGameStartPlayer2));
        }
    }
}
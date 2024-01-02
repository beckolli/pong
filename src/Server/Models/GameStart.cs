namespace Pong.Server.Models;

public class GameStart
{
    public GameStart(short playerNumber)
    {
        PlayerNumber = playerNumber;

        var random = new Random();
        BallX = random.Next(0, 2) == 0 ? -1 : 1;
        BallY = random.Next(0, 2) == 0 ? -1 : 1;
    }
    public float BallX { get; set; }
    public float BallY { get; set; }
    public short PlayerNumber { get; set; } = 1;
}

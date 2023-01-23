namespace Pong.Server.Models
{
    public class Game
    {
        public Game(Player player1)
        {
            Player1 = player1;
        }
        public Player Player1 { get; set; }
        public Player? Player2 { get; set; }
    }
}
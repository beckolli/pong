using Pong.Server.Extensions;
using Pong.Server.Models;
using Xunit;

namespace Pong.Server.Test
{
    public class UnitTests
    {
        //disabled because the socket can't be nullable
        [Fact]
        public void ConnectPlayerToGame()
        {
            Player player1 = new();
            Player player2 = new();

            var game = player1.ConnectPlayerToGame();
            Assert.NotNull(game);
            Assert.NotNull(game.Player1);

            game = player2.ConnectPlayerToGame(game);
            Assert.NotNull(game);
            Assert.NotNull(game.Player2);
        }
    }
}
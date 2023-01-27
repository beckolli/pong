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
            #pragma warning disable 8625
            Player player1 = new (null);
            Player player2 = new (null);

            var game = SocketListener.ConnectPlayerToGame(player1, null);
            Assert.NotNull(game);
            Assert.NotNull(game.Player1);

            game = SocketListener.ConnectPlayerToGame(player2, game);
            Assert.NotNull(game);
            Assert.NotNull(game.Player2);
        }
    }
}
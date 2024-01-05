using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using Pong.Server.Extensions;
using Pong.Server.Models;

namespace Pong.Server;

/// <summary>
/// WebSocketServer implementation
/// </summary>
public class WsConnectionHolder(Game _game, Player _currentPlayer, WebSocket _webSocket)
{
    public async Task ReadDataAndSendToOpponent()
    {        
        if(_game == null) return;

        if (_game.Player1 != null && _game.Player2 != null)
            _game?.SendGameStartMessage();
                
        while (true)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await _webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            var data = Encoding.UTF8.GetString(buffer)[..receiveResult.Count];
            
            Console.WriteLine($"Player {_currentPlayer.Name} send data: {data}");
            
            if (_game.Player2 != null && _game.Player1 != null)
            {
                if (_currentPlayer == _game.Player1)
                {
                    _game.Player2.SendAsync(data).GetAwaiter().GetResult();
                }
                else
                {
                    _game.Player1.SendAsync(data).GetAwaiter().GetResult();
                }
            }

        }
    }
}


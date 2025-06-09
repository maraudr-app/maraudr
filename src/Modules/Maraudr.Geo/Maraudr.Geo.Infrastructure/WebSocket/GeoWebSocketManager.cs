using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Maraudr.Geo.Infrastructure.WebSocket;

public static class GeoWebSocketManager
{
    private static readonly List<System.Net.WebSockets.WebSocket> _sockets = new();

    public static void Add(System.Net.WebSockets.WebSocket socket) => _sockets.Add(socket);

    public static async Task BroadcastAsync(object message)
    {
        var json = JsonSerializer.Serialize(message);
        var buffer = Encoding.UTF8.GetBytes(json);

        foreach (var socket in _sockets.ToList())
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                _sockets.Remove(socket);
            }
        }
    }
}

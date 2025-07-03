using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Maraudr.Geo.Infrastructure.WebSocket;

public record SocketWithAssociation(System.Net.WebSockets.WebSocket Socket, Guid AssociationId);

public static class GeoWebSocketManager
{
    private static readonly List<SocketWithAssociation> Sockets = new();

    public static void Add(System.Net.WebSockets.WebSocket socket, Guid associationId)
    {
        Sockets.Add(new SocketWithAssociation(socket, associationId));
    }

    public static async Task BroadcastAsync(double latitude, double longitude, DateTime  observationDate, string notes, Guid associationId)
    {
        var json = JsonSerializer.Serialize(new
        {
            latitude,
            longitude,
            observationDate,
            notes,
            associationId
        });

        var buffer = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(buffer);

        foreach (var socketWithId in Sockets.ToList().Where(socketWithId => socketWithId.Socket.State == WebSocketState.Open &&
                     socketWithId.AssociationId == associationId))
        {
            await socketWithId.Socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

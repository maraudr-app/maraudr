using System.Net.WebSockets;
using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Domain.Entities;
using Maraudr.Geo.Domain.Interfaces;
using Maraudr.Geo.Infrastructure;
using Maraudr.Geo.Infrastructure.WebSocket;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GeoContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("GeoDb")));
builder.Services.AddScoped<IGeoRepository, GeoRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseWebSockets();

app.MapPost("/geo", async (CreateGeoDataRequest dto, IGeoRepository repo) =>
{
    var store = await repo.GetGeoStoreByAssociationAsync(dto.AssociationId);
    if (store is null) return Results.NotFound("GeoStore not found");

    var data = new GeoData
    {
        Id = Guid.NewGuid(),
        GeoStoreId = store.Id,
        Latitude = dto.Latitude,
        Longitude = dto.Longitude,
        Notes = dto.Notes,
        ObservedAt = DateTime.UtcNow
    };

    await repo.AddEventAsync(data);

    var response = new GeoDataResponse(data.Id, data.GeoStoreId, data.Latitude, data.Longitude, data.Notes, data.ObservedAt);
    return Results.Ok(response);
});

app.MapPost("/geo/store", async (CreateGeoStoreRequest request, IGeoRepository repo) =>
{
    var existing = await repo.GetGeoStoreByAssociationAsync(request.AssociationId);
    if (existing is not null)
        return Results.Conflict("GeoStore already exists for this association");

    var created = await repo.CreateGeoStoreAsync(request.AssociationId);
    return Results.Created($"/geo/store/{created.Id}", new GeoStoreResponse(created.Id, created.AssociationId));
});

app.MapGet("/geo/{associationId}", async (Guid associationId, int days, IGeoRepository repo) =>
{
    var from = DateTime.UtcNow.AddDays(-days);
    var data = await repo.GetEventsAsync(associationId, from);
    return Results.Ok(data);
});

app.MapGet("/geo/store/{associationId}", async (Guid associationId, IGeoRepository repo) =>
{
    var geoStore = await repo.GetGeoStoreByAssociationAsync(associationId);
    return geoStore is not null
        ? Results.Ok(new { geoStore.Id })
        : Results.NotFound("GeoStore not found for this association.");
});

app.Map("/geo/live", async (HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        GeoWebSocketManager.Add(socket);
        while (socket.State == WebSocketState.Open)
        {
            var buffer = new byte[1024];
            await socket.ReceiveAsync(buffer, CancellationToken.None);
        }
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();
using System.Net.WebSockets;
using Maraudr.Geo.Application;
using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Application.UseCases;
using Maraudr.Geo.Endpoints;
using Maraudr.Geo.Infrastructure;
using Maraudr.Geo.Infrastructure.WebSocket;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseWebSockets();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/geo", [Authorize] async (
    HttpContext httpContext,
    CreateGeoDataRequest dto,
    ICreateGeoDataForAnAssociation handler) =>
{
    var response = await handler.HandleAsync(dto);

    if (response == null)
        return Results.BadRequest();

    await GeoWebSocketManager.BroadcastAsync(
        response.Latitude,
        response.Longitude,
        response.ObservedAt,
        response.Notes,
        dto.AssociationId);

    return Results.Created($"/geo/store/{response.Id}", new { response.Id });
});

app.MapPost("/geo/store", async (CreateGeoStoreRequest request, ICreateGeoStoreForAnAssociation handler) =>
{
    try
    {
        var response = await handler.HandleAsync(request);
        return Results.Created($"/geo/store/{response.Id}", new GeoStoreResponse(response.Id, response.AssociationId));
    }
    catch (Exception e)
    {
        return Results.BadRequest(e.Message);
    }
});

app.MapGet("/geo/{associationId}", [Authorize] async (
    Guid associationId,
    int days,
    IGetAllGeoDataForAnAssociation handler) =>
{
    var response = await handler.HandleAsync(associationId, days);
    return Results.Ok(response);
});

app.MapGet("/geo/store/{associationId}", [Authorize] async (
    Guid associationId,
    IGetGeoStoreInfoForAnAssociation handler) =>
{
    var id = await handler.HandleAsync(associationId);
    return id is not null
        ? Results.Ok(new { id })
        : Results.NotFound("GeoStore not found for this association.");
});

app.Map("/geo/live", async context =>
{
    var associationIdQuery = context.Request.Query["associationId"];
    if (!Guid.TryParse(associationIdQuery, out var associationId))
    {
        context.Response.StatusCode = 400;
        return;
    }

    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        GeoWebSocketManager.Add(socket, associationId);

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
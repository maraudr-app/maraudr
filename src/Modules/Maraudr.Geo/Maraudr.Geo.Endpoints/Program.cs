using System.Net.WebSockets;
using System.Text.Json;
using Maraudr.Geo.Application;
using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Application.UseCases;
using Maraudr.Geo.Endpoints;
using Maraudr.Geo.Infrastructure;
using Maraudr.Geo.Infrastructure.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowFrontend");

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

app.MapPost("/geo/store", async (
    CreateGeoStoreRequest request,
    ICreateGeoStoreForAnAssociation handler,
    [FromHeader(Name = "X-Geo-Api-Key")] string apiKey) =>
{
    if (apiKey != Environment.GetEnvironmentVariable("GEO_API_KEY"))
    {
        return Results.Unauthorized();
    }
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

app.MapGet("/geo/route", [Authorize] async (
    Guid associationId,
    double latitude,
    double longitude,
    double radiusKm,
    double startLat,
    double startLng,
    IGetGeoRouteHandler handler) =>
{
    var result = await handler.HandleAsync(associationId, latitude, longitude, radiusKm, startLat, startLng);

    if (result is null)
        return Results.NotFound("No route found for the selected association.");

    return Results.Ok(new
    {
        geoJson = JsonSerializer.Deserialize<JsonElement>(result.GeoJson),
        result.Distance,
        result.Duration,
        result.Coordinates,
        result.GoogleMapsUrl
    });
});

app.Map("/geo/live", async context =>
{
    if (!context.User.Identity?.IsAuthenticated ?? true)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }

    var associationIdQuery = context.Request.Query["associationId"];
    if (!Guid.TryParse(associationIdQuery, out var associationId))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
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
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});


app.Run();
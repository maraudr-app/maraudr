using System.Net.WebSockets;
using Maraudr.Geo.Application;
using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Application.UseCases;
using Maraudr.Geo.Endpoints;
using Maraudr.Geo.Infrastructure;
using Maraudr.Geo.Infrastructure.GeoData;
using Maraudr.Geo.Infrastructure.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IGeoAddressEnricher, GeoapifyReverseGeocodingService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://maraudr.eu", "https://www.maraudr.eu","https://maraudr-front-737l.onrender.com")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

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


app.MapGet("/autocomplete", [Authorize] async (
    [FromQuery] string text,
    [FromServices] IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();

    var apiKey = Environment.GetEnvironmentVariable("GEOAPIFY_API_KEY"); 
    var url = $"https://api.geoapify.com/v1/geocode/autocomplete?text={Uri.EscapeDataString(text)}&apiKey={apiKey}";

    var response = await client.GetAsync(url);

    if (!response.IsSuccessStatusCode)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        return Results.BadRequest(errorContent);
    }

    var content = await response.Content.ReadAsStringAsync();
    return Results.Content(content, "application/json");
});


app.MapPost("/itineraries", [Authorize] async (
    [FromBody] CreateItineraryRequest dto,
    ICreateItineraryHandler handler) =>
{
    var result = await handler.HandleAsync(dto);

    return result is null
        ? Results.BadRequest("Unable to generate route or event not found.")
        : Results.Created($"/itineraries/{result.Id}", result);
});

app.MapGet("/itineraries/{id:guid}", [Authorize] async (
    Guid id,
    IGetItineraryHandler handler) =>
{
    var result = await handler.GetByIdAsync(id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapPatch("/geo/{id:guid}/status", [Authorize] async (
    Guid id,
    IToogleGeoStatusHandler handler) =>
{
    await handler.HandleAsync(id);
    return Results.NoContent();
});

app.MapPatch("/itinerary/{id:guid}/status", [Authorize] async (
    Guid id,
    IToogleItineraryStatusHandler handler) =>
{
    await handler.HandleAsync(id);
    return Results.NoContent();
});

app.MapGet("/itineraries", [Authorize] async (
    Guid associationId,
    IGetItineraryHandler handler) =>
{
    var results = await handler.GetByAssociationIdAsync(associationId);
    return Results.Ok(results);
});

app.Map("/geo/live", async context =>
{
    var associationIdQuery = context.Request.Query["associationId"];
    if (!Guid.TryParse(associationIdQuery, out var associationId))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        Console.WriteLine("[WS] ❌ Invalid or missing associationId.");
        return;
    }

    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        Console.WriteLine("[WS] ❌ Not a WebSocket request.");
        return;
    }

    var socket = await context.WebSockets.AcceptWebSocketAsync();
    Console.WriteLine($"[WS] ✅ WebSocket connected for associationId: {associationId}");

    GeoWebSocketManager.Add(socket, associationId);

    var buffer = new byte[1024 * 4];

    while (socket.State == WebSocketState.Open)
    {
        try
        {
            var receiveTask = socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));

            var completedTask = await Task.WhenAny(receiveTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                var pingMessage = System.Text.Encoding.UTF8.GetBytes("ping");
                await socket.SendAsync(
                    new ArraySegment<byte>(pingMessage),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
                Console.WriteLine($"[WS] 🔄 Sent keep-alive ping to {associationId}");
                continue;
            }

            var result = await receiveTask;

            if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine($"[WS] ❌ Client requested close for {associationId}");
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);
            }
            else
            {
                var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"[WS] 📩 Received from {associationId}: {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WS] ❌ Exception for {associationId}: {ex.Message}");
            break;
        }
    }

    Console.WriteLine($"[WS] 🔌 WebSocket closed for {associationId}");
});

app.Run();

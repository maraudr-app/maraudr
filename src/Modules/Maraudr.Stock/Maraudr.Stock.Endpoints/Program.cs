using Maraudr.Stock.Application.Dtos;
using Maraudr.Stock.Domain.Entities;
using Maraudr.Stock.Domain.Interfaces;
using Maraudr.Stock.Endpoints;
using Maraudr.Stock.Infrastructure.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddValidation();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddAuthenticationServices(builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>(),builder.Configuration);

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
app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/item/{id}", [Authorize] async (
    Guid id,
    [FromQuery] Guid associationId,
    IQueryItemByAssociationHandler handler) =>
{
    if (associationId == Guid.Empty)
        return Results.BadRequest(new { message = "associationId requis" });

    var item = await handler.HandleAsync(id, associationId);

    return item is null ? Results.NotFound(new { message = "Item introuvable" }) : Results.Ok(item);
});

app.MapGet("/item/barcode/{barcode}", [Authorize] async (string barcode,Guid associationId, IQueryItemWithBarCodeHandler handler) =>
{
    var item = await handler.HandleAsync(barcode,associationId);
    return Results.Ok(item);
});

app.MapPost("/item/{barcode}", [Authorize] async (
    string barcode,
    [FromBody] CreateItemRequest request,
    ICreateItemFromBarcodeHandler handler,
    IRedisCacheService cache,
    IStockRepository repository) =>
{
    Console.WriteLine(request.AssociationId);
    if (request.AssociationId == Guid.Empty)
    {
        return Results.BadRequest(new { message = "associationId requis" });
    }

    try
    {
        Console.WriteLine(request.AssociationId);
        var id = await handler.HandleAsync(barcode, request.AssociationId);
        return Results.Created($"/item/{id}", new { id });
    }
    catch (Exception e)
    {
        return Results.NotFound(new { message = e.Message });
    }
    finally
    {
        var stockId = await repository.GetStockIdByAssociationIdAsync(request.AssociationId);
        await cache.RemoveByPatternAsync($"items:{stockId}");
    }
});

app.MapPut("/item/reduce/{barcode}", [Authorize] async (
    string barcode,
    [FromBody] UpdateItemQuantityInStockRequest request,
    IReduceQuantityFromItemHandler handler,
    IRedisCacheService cache, IStockRepository repository) =>
{
    if (request.AssociationId == Guid.Empty)
        return Results.BadRequest(new { message = "associationId requis" });

    var quantity = request.Quantity.GetValueOrDefault(1);
    if (quantity <= 0)
        return Results.BadRequest(new { message = "La quantité doit être > 0" });

    try
    {
        await handler.HandleAsync(barcode, request.AssociationId, quantity);
        return Results.Ok(new { message = "Quantité réduite ou item supprimé" });
    }
    catch (Exception e)
    {
        return Results.BadRequest(new { message = e.Message });
    }
    finally
    {
        var stockId = await repository.GetStockIdByAssociationIdAsync(request.AssociationId);
        await cache.RemoveByPatternAsync($"items:{stockId}");
    }
});

app.MapPut("/item/update-quantity/{id}", [Authorize] async (
    Guid id,
    [FromBody] UpdateItemQuantityInStockRequest request,
    IUpdateQuantityFromItemHandler handler,
    IRedisCacheService cache, IStockRepository repository) =>
{
    if (request.AssociationId == Guid.Empty)
        return Results.BadRequest(new { message = "associationId requis" });

    var quantity = request.Quantity.GetValueOrDefault(1);
    
    try
    {
        await handler.HandleAsync(id, request.AssociationId, quantity);
        return Results.Ok(new { message = "Quantité réduite ou item supprimé" });
    }
    catch (Exception e)
    {
        return Results.BadRequest(new { message = e.Message });
    }
    finally
    {
        var stockId = await repository.GetStockIdByAssociationIdAsync(request.AssociationId);
        await cache.RemoveByPatternAsync($"items:{stockId}");
    }
});

app.MapPost("/item", [Authorize] async (
    CreateItemCommand item,
    ICreateItemHandler handler,
    IValidator<CreateItemCommand> validator,
    IRedisCacheService cache) =>
{
    var result = validator.Validate(item);
    if (!result.IsValid)
    {
        var messages = result.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
        return Results.BadRequest(messages);
    }

    var id = await handler.HandleAsync(item);

    await cache.RemoveByPatternAsync($"items:{item.StockId}");
    return Results.Created($"/item/{id}", new { id });
});

app.MapPost("/create-stock", async (
    CreateStockRequest request, 
    ICreateStockHandler handler, 
    [FromHeader(Name = "X-Stock-Api-Key")] string apiKey) =>
{
    if (apiKey != Environment.GetEnvironmentVariable("STOCK_API_KEY"))
    {
        return Results.Unauthorized();
    }
    
    if (request.AssociationId == Guid.Empty)
    {
        return Results.BadRequest("associationId is required");
    }

    var id = await handler.HandleAsync(request.AssociationId);
    return Results.Created($"/create-stock/{id}", new { Id = id });
});

app.MapGet("/stock/items", [Authorize] async (
    Guid associationId,
    string? category,
    string? name,
    IGetItemsOfAssociationHandler handler,
    IRedisCacheService redis, 
    IStockRepository repository) =>
{
    if (associationId == Guid.Empty)
        return Results.BadRequest(new { message = "Missing or invalid association ID." });

    var stockId = await repository.GetStockIdByAssociationIdAsync(associationId);
    
    var filter = new ItemFilter(category, name);
    var cacheKey = $"items:{stockId}";

    var cachedItems = await redis.GetAsync<IEnumerable<StockItem>>(cacheKey);
    if (cachedItems is not null)
        return Results.Ok(cachedItems);

    var items = (await handler.HandleAsync(associationId, filter)).ToList();
        
    await redis.SetAsync(cacheKey, items, TimeSpan.FromMinutes(10));

    return Results.Ok(items);
});

app.MapDelete("/stock/item/{itemId}", [Authorize] async (
    Guid itemId,
    [FromQuery] Guid associationId,
    IDeleteItemFromStockHandler handler,
    IRedisCacheService cache,
    IStockRepository repository) =>
{
    if (associationId == Guid.Empty || itemId == Guid.Empty)
        return Results.BadRequest(new { message = "Invalid parameters" });

    var success = await handler.HandleAsync(associationId, itemId);
    
    if (!success)
        return Results.NotFound(new { message = "L'item n'existe pas ou n'appartient pas à cette association" });
    
    var stockId = await repository.GetStockIdByAssociationIdAsync(associationId);
    
    await cache.RemoveByPatternAsync($"items:{stockId}");

    return Results.NoContent();
});

app.MapGet("/stock/{associationId}", [Authorize] async (
    Guid associationId,
    IGetStockIdByAssociationHandler handler) =>
{
    if (associationId == Guid.Empty)
        return Results.BadRequest(new { message = "associationId manquant ou invalide" });

    var stockId = await handler.HandleAsync(associationId);
    return stockId is not null
        ? Results.Ok(new { StockId = stockId })
        : Results.NotFound(new { message = "Aucun stock trouvé pour cette association" });
});


app.Run();

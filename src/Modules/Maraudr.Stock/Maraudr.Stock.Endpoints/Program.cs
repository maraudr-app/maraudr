using Maraudr.Stock.Application.Dtos;
using Maraudr.Stock.Endpoints;
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

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/item/{id}", [Authorize] async (
    Guid id,
    Guid associationId,
    IQueryItemByAssociationHandler handler) =>
{
    if (associationId == Guid.Empty)
        return Results.BadRequest(new { message = "associationId requis" });

    var item = await handler.HandleAsync(id, associationId);
    return item is not null
        ? Results.Ok(item)
        : Results.NotFound(new { message = "Item introuvable ou non autorisé" });
});


app.MapGet("/item/type/{type}", [Authorize] async (Category type, IQueryItemByType handler) =>
{
    var item = await handler.HandleAsync(type);
    return Results.Ok(item);
});

app.MapGet("/item/barcode/{barcode}", [Authorize] async (string barcode, IQueryItemWithBarCodeHandler handler) =>
{
    var item = await handler.HandleAsync(barcode);
    return Results.Ok(item);
});

/*
app.MapPatch("/item/{id}/quantity", async (Guid id, [FromBody] int quantity, IRemoveQuantityFromStockHandler handler) =>
{
    try
    {
        await handler.HandleAsync(id, quantity);
        return Results.Ok(id);
    }
    catch (Exception e)
    {
        return Results.BadRequest(e.Message);
    }
});
*/

app.MapPost("/item/{barcode}", [Authorize] async (
    string barcode,
    [FromBody] CreateItemRequest request,
    ICreateItemFromBarcodeHandler handler) =>
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
});

app.MapPost("/item", [Authorize] async (
    CreateItemCommand item,
    ICreateItemHandler handler,
    IValidator<CreateItemCommand> validator) =>
{
    var result = validator.Validate(item);

    if (!result.IsValid)
    {
        var messages = result.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
        return Results.BadRequest(messages);
    }

    var id = await handler.HandleAsync(item);
    return Results.Created($"/item/{id}", new { id });
});

app.MapPost("/create-stock", async (CreateStockRequest request, ICreateStockHandler handler) =>
{
    if (request.AssociationId == Guid.Empty)
    {
        return Results.BadRequest("associationId is required");
    }

    var id = await handler.HandleAsync(request.AssociationId);
    return Results.Created($"/create-stock/{id}", new { Id = id });
});

/*
app.MapDelete("/item/{id}", async (Guid id, IDeleteItemHandler handler) =>
{
    await handler.HandleAsync(id);
    return Results.Ok();
});
*/

app.MapGet("/stock/items", [Authorize] async (
    Guid associationId,
    string? category,
    string? name,
    IGetItemsOfAssociationHandler handler) =>
{
    if (associationId == Guid.Empty)
        return Results.BadRequest(new { message = "Missing or invalid association ID." });

    var filter = new ItemFilter(category, name);
    var items = await handler.HandleAsync(associationId, filter);

    return Results.Ok(items);
});

app.MapDelete("/stock/item", [Authorize] async (
    Guid associationId,
    Guid itemId,
    IDeleteItemFromStockHandler handler) =>
{
    if (associationId == Guid.Empty || itemId == Guid.Empty)
        return Results.BadRequest(new { message = "Invalid parameters" });

    var success = await handler.HandleAsync(associationId, itemId);

    return success
        ? Results.NoContent()
        : Results.NotFound(new { message = "Item not found or does not belong to this association" });
});

app.MapGet("/stock/id", [Authorize] async (
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

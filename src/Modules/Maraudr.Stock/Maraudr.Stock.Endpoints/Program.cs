using Maraudr.Stock.Endpoints;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddValidation();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/stock/{id}", async (Guid id, IQueryItemHandler handler) =>
{
    var item = await handler.HandleAsync(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});

app.MapGet("/stock/type/{type}", async (Category type, IQueryItemByType handler) =>
{
    var item = await handler.HandleAsync(type);
    return Results.Ok(item);
});

app.MapPatch("/stock/{id}/quantity", async (Guid id, [FromBody] int quantity, IRemoveQuantityFromStockHandler handler) =>
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

app.MapPost("/stock/{barcode}", async (
    string barcode,
    ICreateItemFromBarcodeHandler handler) =>
{
    Guid id;
    try
    {
        id = await handler.HandleAsync(barcode);
    }
    catch (Exception e)
    {
        return Results.NotFound(e.Message);
    }

    return Results.Created($"/stock/{id}", new { id });
});

app.MapPost("/stock/", async (
    CreateItemCommand item, 
    ICreateItemHandler handler, 
    IValidator<CreateItemCommand> validator) =>
{
    var result = validator.Validate(item);

    if (!result.IsValid)
    {
        var messages = result.Errors
            .ToDictionary(e => e.PropertyName, e => e.ErrorMessage);

        return Results.BadRequest(messages);
    }
    
    var id = await handler.HandleAsync(item);
    
    return Results.Created($"/stock/{id}", new { id });
});



app.MapDelete("/stock/{id}", async (Guid id, IDeleteItemHandler handler) =>
{
    await handler.HandleAsync(id);
    return Results.Ok();
});



app.Run();

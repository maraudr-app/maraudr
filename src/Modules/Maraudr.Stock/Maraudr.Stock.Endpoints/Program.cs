using Maraudr.Stock.Endpoints;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddValidation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.MapPost("/stock/bulk", async (
    CreateItemCommand item, 
    [FromQuery] int quantity, 
    ICreateMultipleItemsHandler handler, 
    IValidator<CreateItemCommand> validator) =>
{
    var result = validator.Validate(item);

    if (!result.IsValid)
    {
        var errors = result.Errors
            .ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
        return Results.BadRequest(errors);
    }

    if (quantity <= 0)
    {
        return Results.BadRequest(new { Error = "Quantity must be greater than 0." });
    }

    var commands = Enumerable.Range(0, quantity)
        .Select(_ => new CreateItemCommand(item.Name, item.Description, item.ItemType))
        .ToList();

    var items = await handler.HandleAsync(commands);

    return Results.Created("/stock", new { items });
});

app.MapDelete("/stock/{id}", async (Guid id, IDeleteItemHandler handler) =>
{
    await handler.HandleAsync(id);
    return Results.Ok();
});

app.Run();

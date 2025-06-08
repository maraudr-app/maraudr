using System.Text;
using Maraudr.Stock.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddValidation();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddAuthenticationServices(builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>(),builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/stock/{id}", [Authorize] async (Guid id, IQueryItemHandler handler) =>
{
    var item = await handler.HandleAsync(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});

app.MapGet("/stock/type/{type}", [Authorize] async (Category type, IQueryItemByType handler) =>
{
    var item = await handler.HandleAsync(type);
    return Results.Ok(item);
});

app.MapGet("/stock/barcode/{barcode}", [Authorize] async (string barcode, IQueryItemWithBarCodeHandler handler) =>
{
    var item = await handler.HandleAsync(barcode);
    return Results.Ok(item);
});

app.MapPatch("/stock/{id}/quantity", [Authorize] async (Guid id, [FromBody] int quantity, IRemoveQuantityFromStockHandler handler) =>
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

app.MapPost("/stock/{barcode}", [Authorize] async (
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

app.MapPost("/stock/", [Authorize] async (
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



app.MapDelete("/stock/{id}", [Authorize] async (Guid id, IDeleteItemHandler handler) =>
{
    await handler.HandleAsync(id);
    return Results.Ok();
});



app.Run();

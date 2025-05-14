using Maraudr.Associations.Application;
using Maraudr.Associations.Application.UseCases;
using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/association", async (Guid id, IGetAssociationHandler handler) =>
{
    var result = await handler.HandleAsync(id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapPost("/association", async (CreateAssociationCommand asso, ICreateAssociationHandler handler) =>
{
    var result = await handler.HandleAsync(asso);
    return Results.Ok(new { Id = result});
});

app.MapGet("/association/verify", async (string siret, IVerifyAssociationBySiret handler, IHttpClientFactory factory) =>
{
    var result = await handler.HandleAsync(siret, factory);
    return Results.Ok(result);
});

app.MapDelete("/association", async (Guid id, IUnregisterAssociation handler) =>
{
    await handler.HandleAsync(id);
    return Results.Ok();    
});

app.UseHttpsRedirection();

app.Run();

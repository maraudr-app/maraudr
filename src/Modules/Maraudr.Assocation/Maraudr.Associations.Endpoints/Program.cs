using Maraudr.Associations.Application;
using Maraudr.Associations.Application.Dtos;
using Maraudr.Associations.Application.UseCases.Command;
using Maraudr.Associations.Application.UseCases.Query;
using Maraudr.Associations.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("JWT");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["ValidIssuer"],
            ValidAudience = jwtSection["ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["Secret"]!))
        };
    }
);

builder.Services.AddAuthorization();

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

app.MapGet("/association/name", async (string name, ISearchAssociationsByNameHandler handler) =>
{
    var result = await handler.HandleAsync(name);
    return result.Any() ? Results.Ok(result) : Results.NotFound();
});

app.MapGet("/association/city", async (string city, ISearchAssociationsByCityHandler handler) =>
{
    var results = await handler.HandleAsync(city);
    return results.Any() ? Results.Ok(results) : Results.NotFound();
});

app.MapPost("/association", async (string siret, 
    ICreateAssociationHandlerSiretIncluded handler, 
    IHttpClientFactory factory) =>
{
    try
    {
        var result = await handler.HandleAsync(siret, factory);
        return Results.Ok(new { Id = result });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/association", [Authorize(Roles = "Admin,Manager")] 
    async (UpdateAssociationInformationDto informations, IUpdateAssociationHandler handler) =>
    {
        var updated = await handler.HandleAsync(informations);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
);

app.MapDelete("/association", [Authorize(Roles = "Admin,Manager")] 
    async (Guid id, IUnregisterAssociation handler) =>
    {
        await handler.HandleAsync(id);
        return Results.Ok();    
    }
);

app.UseAuthentication();
app.UseAuthorization();
    
app.UseHttpsRedirection();

app.Run();

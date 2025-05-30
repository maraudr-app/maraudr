using Maraudr.Associations.Application;
using Maraudr.Associations.Application.Dtos;
using Maraudr.Associations.Application.UseCases.Command;
using Maraudr.Associations.Application.UseCases.Query;
using Maraudr.Associations.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowFrontend3000");

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/association", async (Guid id, IGetAssociationHandler handler) =>
{
    if (id == Guid.Empty)
    {
        return Results.BadRequest("Missing or invalid Id.");
    }
    var result = await handler.HandleAsync(id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapGet("/associations", async (
    int page, 
    IListAssociationsPaginatedHandler handler) =>
{
    if (page < 1)
        return Results.BadRequest("Page must be at least 1.");

    const int pageSize = 15;
    var result = await handler.HandleAsync(page, pageSize);
    return Results.Ok(result);
});

app.MapGet("/association/name", async (string name, ISearchAssociationsByNameHandler handler) =>
{
    if (string.IsNullOrWhiteSpace(name))
        return Results.BadRequest("Name is required.");
    var result = await handler.HandleAsync(name);
    return result.Count != 0 ? Results.Ok(result) : Results.NotFound();
});

app.MapGet("/association/city", async (string city, ISearchAssociationsByCityHandler handler) =>
{
    if (string.IsNullOrWhiteSpace(city))
        return Results.BadRequest("City is required.");
    var results = await handler.HandleAsync(city);
    return results.Count != 0 ? Results.Ok(results) : Results.NotFound();
});

app.MapPost("/association", async (
    string siret,
    ICreateAssociationHandlerSiretIncluded handler,
    IHttpClientFactory factory) =>
{
    if (string.IsNullOrWhiteSpace(siret) || siret.Length != 14 || !siret.All(char.IsDigit))
    {
        return Results.BadRequest(new { message = "Missing or invalid SIRET." });
    }

    try
    {
        var id = await handler.HandleAsync(siret, factory);

        return Results.Created($"/association?id={id}", new { Id = id });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});

app.MapPut("/association", [Authorize(Roles = "Admin,Manager")] 
    async (UpdateAssociationInformationDto dto, 
        IValidator<UpdateAssociationInformationDto> validator,
        IUpdateAssociationHandler handler) =>
    {
        var result = await validator.ValidateAsync(dto);

        if (!result.IsValid)
        {
            return Results.BadRequest(result.Errors.Select(e => new
            {
                e.PropertyName,
                e.ErrorMessage
            }));
        }

        var updated = await handler.HandleAsync(dto);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    });


app.MapDelete("/association", [Authorize(Roles = "Admin,Manager")] 
    async (Guid id, IUnregisterAssociation handler) =>
    {
        if (id == Guid.Empty)
        {
            return Results.BadRequest("Missing or invalid id");
        }
        await handler.HandleAsync(id);
        return Results.NoContent();    
    }
);


app.UseAuthentication();
app.UseAuthorization();
    
app.UseHttpsRedirection();

app.Run();

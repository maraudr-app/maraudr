using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddHealthChecks();

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

app.UseCors("AllowFrontend");

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
    string siret, Guid userId,
    ICreateAssociationHandlerSiretIncluded handler,
    IHttpClientFactory factory) =>
    {
        if (string.IsNullOrWhiteSpace(siret) || siret.Length != 14 || !siret.All(char.IsDigit) )
        {
            return Results.BadRequest(new { message = "Missing or invalid SIRET." });
        }
        
        if (userId == Guid.Empty)
        {
            return Results.BadRequest(new { message = "Missing or invalid Id." });
        }

        try
        {
            var id = await handler.HandleAsync(siret, userId, factory);

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
    });

app.MapPost("/association/member",
    async (AddMemberRequestDto request,
        IAddMemberToAssociationHandler handler) =>
    {
        try
        {
            await handler.HandleAsync(request.UserId, request.AssociationId);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    });

app.MapGet("/association/membership",
    async (Guid id,
        IGetAssocationsOfUserHandler handler) =>
    {
        try
        {
            var associations = await handler.HandleAsync(id);
            return Results.Ok(associations);
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    });

app.MapHealthChecks("/health");
app.UseAuthentication();
app.UseAuthorization();
    
app.UseHttpsRedirection();

app.Run();


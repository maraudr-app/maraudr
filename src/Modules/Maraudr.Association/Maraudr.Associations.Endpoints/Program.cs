using System.Security.Claims;
using Maraudr.Associations.Endpoints;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddHealthChecks();
builder.Services.AddAuthenticationServices(builder.Configuration);

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

app.MapHealthChecks("/health");
app.UseAuthentication();
app.UseAuthorization();
    
app.UseHttpsRedirection();
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

app.MapPost("/association", [Authorize] async (
    HttpContext httpContext,
    [FromQuery] string siret,
    ICreateAssociationHandlerSiretIncluded handler,
    IHttpClientFactory siretHttpFactory,
    IHttpClientFactory stockHttpFactory,
    IHttpClientFactory geoHttpFactory) =>
{
    if (string.IsNullOrWhiteSpace(siret) || siret.Length != 14 || !siret.All(char.IsDigit))
    {
        return Results.BadRequest(new { message = "Missing or invalid SIRET." });
    }

    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (!Guid.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized(); 
    }

    try
    {
        var result = await handler.HandleAsync(siret, userId, siretHttpFactory, stockHttpFactory, geoHttpFactory);
        return Results.Created($"/association?id={result.AssociationId}", new
        {
            result.AssociationId,
            result.StockId,
            result.GeoStoreId,
            result.PlanningId
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
});


app.MapPut("/association", [Authorize] async (
    HttpContext httpContext,
    UpdateAssociationInformationDto dto,
    IValidator<UpdateAssociationInformationDto> validator,
    IUpdateAssociationHandler handler,
    IIsUserMemberOfAssociationHandler membershipHandler) =>
{
    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!Guid.TryParse(userIdClaim, out var userId))
        return Results.Unauthorized();

    var isMember = await membershipHandler.HandleAsync(userId, dto.Id);
    if (!isMember)
        return Results.Forbid();

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

app.MapDelete("/association", [Authorize] async (
    HttpContext httpContext,
    Guid id,
    IUnregisterAssociation handler,
    IIsUserMemberOfAssociationHandler membershipHandler) =>
{
    if (id == Guid.Empty)
        return Results.BadRequest("Missing or invalid id");

    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!Guid.TryParse(userIdClaim, out var userId))
        return Results.Unauthorized();

    var isMember = await membershipHandler.HandleAsync(userId, id);
    if (!isMember)
        return Results.Forbid();

    await handler.HandleAsync(id);
    return Results.NoContent();
});


app.MapPost("/association/member", [Authorize] async (
    HttpContext httpContext,
    AddMemberRequestDto request,
    IAddMemberToAssociationHandler handler,
    IIsUserMemberOfAssociationHandler membershipHandler) =>
{
    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!Guid.TryParse(userIdClaim, out var requesterId))
        return Results.Unauthorized();

    var isMember = await membershipHandler.HandleAsync(requesterId, request.AssociationId);
    if (!isMember)
        return Results.Forbid();

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

app.MapGet("/association/membership", [Authorize] async (
    HttpContext httpContext,
    IGetAssocationsOfUserHandler handler) =>
{
    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!Guid.TryParse(userIdClaim, out var userId))
        return Results.Unauthorized();

    try
    {
        var associations = await handler.HandleAsync(userId);
        return Results.Ok(associations);
    }
    catch (Exception e)
    {
        return Results.BadRequest(e.Message);
    }
});

app.MapGet("/association/is-member/{associationId}/{userId}",
    async (Guid userId, Guid associationId, IIsUserMemberOfAssociationHandler handler) =>
    {
        try
        {
            var isMember = await handler.HandleAsync(userId, associationId);
            return Results.Ok(isMember);

        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    });



app.Run();


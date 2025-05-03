using Application;
using Application.DTOs.Requests;
using Application.UseCases.User.CreateUser;
using FluentValidation;
using Maraudr.User.Endpoints;
using Maraudr.User.Application;
using Maraudr.User.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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



// USERS

app.MapPost("/users", async (CreateUserDto user, ICreateUserHandler handler, IValidator<CreateUserDto> validator ) => {
    var result = validator.Validate(user);

    if (!result.IsValid)
    {
        var messages = result.Errors
            .ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
        return Results.BadRequest(messages);
    }
    
    var id = await handler.HandleAsync(user);
    
    return Results.Created($"/stock/{id}", new { id });    return Results.Created("/users/{newId}", null);
});


app.MapGet("/users", () => {
    // TODO: Return list of users (with optional filters)
    return Results.Ok();
});

app.MapGet("/users/{id:guid}", (Guid id) => {
    // TODO: Return specific user by id
    return Results.Ok();
});

app.MapPut("/users/{id:guid}", (Guid id, HttpContext context) => {
    // TODO: Update user info
    return Results.NoContent();
});

app.MapDelete("/users/{id:guid}", (Guid id) => {
    // TODO: Delete user
    return Results.NoContent();
});

app.MapPatch("/users/{id:guid}/activate", (Guid id) => {
    // TODO: Toggle user active state
    return Results.Ok();
});

// ADMIN PRIVILEGES
app.MapPost("/users/{id:guid}/grant-admin", (Guid id) => {
    // TODO: Grant admin privileges
    return Results.Ok();
});

app.MapPost("/users/{id:guid}/revoke-admin", (Guid id) => {
    // TODO: Revoke admin privileges
    return Results.Ok();
});

// ROLE MANAGEMENT
app.MapPost("/users/{id:guid}/change-role", (Guid id, HttpContext context) => {
    // TODO: Change user role
    return Results.Ok();
});

// MANAGER TEAM
app.MapGet("/managers/{id:guid}/team", (Guid id) => {
    // TODO: Get manager team members
    return Results.Ok();
});

app.MapPost("/managers/{id:guid}/team", (Guid id, HttpContext context) => {
    // TODO: Add members to manager's team
    return Results.Ok();
});

app.MapDelete("/managers/{id:guid}/team/{memberId:guid}", (Guid id, Guid memberId) => {
    // TODO: Remove member from manager's team
    return Results.Ok();
});

app.MapPut("/users/{id:guid}/change-manager", (Guid id, HttpContext context) => {
    // TODO: Change manager of a user
    return Results.Ok();
});

// SEARCH / STATS / SELF
app.MapGet("/users/search", (string? name, string? role) => {
    // TODO: Search users by name and/or role
    return Results.Ok();
});

app.MapGet("/stats/users", () => {
    // TODO: Return user statistics
    return Results.Ok();
});

app.MapGet("/me", () => {
    // TODO: Return currently authenticated user
    return Results.Ok();
});

app.Run();

using Application;
using Application.DTOs.Requests;
using Application.UseCases.Manager.QueryManagersTeam;
using Application.UseCases.User.CreateUser;
using Application.UseCases.User.DeleteUser;
using Application.UseCases.User.QueryAllUsers;
using Application.UseCases.User.QueryUser;
using Application.UseCases.User.UpdateUser;
using FluentValidation;
using Maraudr.User.Endpoints;
using Maraudr.User.Application.DTOs.Requests;
using Maraudr.User.Domain.Entities;
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
app.MapPost("/users", async (CreateUserDto user, ICreateUserHandler handler, 
    IValidator<CreateUserDto> validator ) => {
    var result = validator.Validate(user);

    if (!result.IsValid)
    {
        var messages = result.Errors
            .ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
        return Results.BadRequest(messages);
    }

    Guid id;
    try
    {
        id = await handler.HandleAsync(user);
    }
    catch (InvalidOperationException e)
    {
        return Results.BadRequest(e.Message);
    }
    catch (Exception e)
    {
        return Results.Problem((e.Message));
    }
    return Results.Created($"/users/{id}", id);
});


app.MapGet("/users", async (IQueryAllUsersHandler handler) =>
{
    var users = await handler.HandleAsync();
    return Results.Ok(users);
});

app.MapGet("/users/{id:guid}", async (Guid id, IQueryUserHandler handler ) => {
    var user = await handler.HandleAsync(id);
    return user == null ? Results.NotFound() : Results.Ok(user);
});

app.MapPut("/users/{id:guid}", async (Guid id, UpdateUserDto user,
    IUpdateUserHandler handler, IValidator<UpdateUserDto> validator) => {
    var result = validator.Validate(user);

    if (!result.IsValid)
    {
        var messages = result.Errors
            .ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
        return Results.BadRequest(messages);
    }
    
    try
    {
        await handler.HandleAsync(id, user);
    }
    catch (InvalidOperationException e)
    {
        return Results.BadRequest(e.Message);

    }
    catch (Exception e)
    {
        return Results.Problem(e.Message);
    }
    return Results.Accepted($"/users/{id}", new { id });
});

app.MapDelete("/users/{id:guid}", async (Guid id,IDeleteUserHandler handler) =>
{
    try
    {
        await handler.HandleAsync(id);
    }
    catch (InvalidOperationException e)
    {
        return Results.BadRequest(e.Message);
    }
    catch (Exception e)
    {
        return Results.Problem(e.Message);
    } 
    return Results.Ok();
});



// MANAGER TEAM
app.MapGet("/managers/{id:guid}/team", async (Guid id,IQueryManagersTeamHandler handler) =>
{
    IEnumerable<AbstractUser> team; 
    try
    {
        team  = await handler.HandleAsync(id);
    }
    catch (InvalidOperationException e)
    {
        return Results.BadRequest(e.Message);
    }
    catch (ArgumentException e1)
    {
        return Results.NotFound(e1.Message);
    }
    return Results.Ok(team);
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

app.Run();

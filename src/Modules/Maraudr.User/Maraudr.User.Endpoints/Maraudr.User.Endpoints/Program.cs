using System.Security.Claims;
using Application;
using Application.DTOs.Requests;
using Application.Services.User;
using Application.UseCases.Manager.AddUserToManagersTeam;
using Application.UseCases.Manager.QueryManagersTeam;
using Application.UseCases.Manager.RemoveUserFromManagersTEam;
using Application.UseCases.User.CreateUser;
using Application.UseCases.User.DeleteUser;
using Application.UseCases.User.QueryAllUsers;
using Application.UseCases.User.QueryUser;
using Application.UseCases.User.QueryUserByEmail;
using Application.UseCases.User.SearchByNameUser;
using Application.UseCases.User.UpdateUser;
using FluentValidation;
using Maraudr.User.Endpoints;
using Maraudr.User.Application.DTOs.Requests;
using Maraudr.User.Domain.Entities;
using Maraudr.User.Infrastructure;
using Microsoft.AspNetCore.Mvc;

// TODO : many endpoints require auth -> doit être impléménté assez vite 
// TODO : verifeir unicité via email aussi & numéro de telephone aussi  

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


app.MapGet("users/email/{email}", async (string email, IQueryUserByEmailHandler handler) => {
    if (string.IsNullOrWhiteSpace(email))
        return Results.BadRequest("L'adresse e-mail est requise");

    try {
        var user = await handler.HandleAsync(email);
        return user == null ? Results.NotFound() : Results.Ok(user);
    }
    catch (ArgumentException e) {
        return Results.BadRequest(e.Message);
    }
    catch (Exception e) {
        return Results.Problem(e.Message);
    }
});


// MANAGER TEAM
app.MapGet("/managers/team", async (
        IQueryManagersTeamHandler handler,
        ClaimsPrincipal currentUser) =>
    {
        var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
    
        if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var managerGuid))
        {
            return Results.Unauthorized();
        }
    
        IEnumerable<AbstractUser> team; 
        try
        {
            team = await handler.HandleAsync(managerGuid);
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
    })
    .RequireAuthorization(); 


app.MapPost("/managers/team/add-user", async (
        [FromBody] UserIdRequest request, 
        IAddUserToManagersTeamHandler handler,
        ClaimsPrincipal user) =>
    {
        var managerId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    
        if (string.IsNullOrEmpty(managerId) || !Guid.TryParse(managerId, out var managerGuid))
        {
            return Results.Unauthorized();
        }
        try
        {
            await handler.HandleAsync(managerGuid, request.UserId);
            return Results.Ok();
        }
        catch (ArgumentException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Results.Forbid();
        }
        catch (Exception ex)
        {
            return Results.Problem($"Une erreur est survenue lors de l'ajout de l'utilisateur à l'équipe. {ex.Message}");
        }
    })
    .RequireAuthorization(); 




app.MapDelete("/managers/remove-from-team", async ([FromBody] UserIdRequest request, 
        IRemoveUserFromManagerTeamHandler handler,
        ClaimsPrincipal user) =>
    {
        var managerId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    
        if (string.IsNullOrEmpty(managerId) || !Guid.TryParse(managerId, out var managerGuid))
        {
            return Results.Unauthorized();
        }
        try
        {
            await handler.HandleAsync(managerGuid, request.UserId);
            return Results.Ok();
        }
        catch (ArgumentException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Results.Forbid();
        }
        catch (Exception ex)
        {
            return Results.Problem($"Une erreur est survenue lors de l'ajout de l'utilisateur à l'équipe.{ex.Message}" );
        }
    })
    .RequireAuthorization(); 



// SEARCH / STATS / SELF
app.MapGet("/users/search", async (string? name, ISearchByNameUserHandler handler) => {
    if (string.IsNullOrWhiteSpace(name))
        return Results.BadRequest("Le terme de recherche est requis");
        
    var results = await handler.HandleAsync(name);
    return Results.Ok(results);
});


/*
app.MapPut("/users/{id:guid}/change-manager", (Guid id, HttpContext context) => {
    // TODO: Change manager of a user
    return Results.Ok();
});
app.MapPost("/managers/team", (Guid id,Guid userId, IAddUserToManagersTeamHandler handler) =>
{
    //todo : add multiple users 
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
});*/

app.Run();

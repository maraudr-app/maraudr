using System.Security.Claims;
using Application.DTOs.UsersQueriesDtos.Requests;
using Application.UseCases.Users.User.CreateUser;
using Application.UseCases.Users.User.DeleteUser;
using Application.UseCases.Users.User.QueryAllUsers;
using Application.UseCases.Users.User.QueryConnectedUsers;
using Application.UseCases.Users.User.QueryUser;
using Application.UseCases.Users.User.QueryUserByEmail;
using Application.UseCases.Users.User.SearchByNameUser;
using Application.UseCases.Users.User.UpdateUser;
using FluentValidation;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Endpoints.Identity;
using Maraudr.User.Infrastructure;
using Microsoft.Extensions.Options;

namespace Maraudr.User.Endpoints;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
   
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto user, [FromServices]ICreateUserHandler handler,[FromServices] IValidator<CreateUserDto> validator)
    {
        var result = validator.Validate(user);

        if (!result.IsValid)
        {
            var messages = result.Errors
                .ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
            return BadRequest(messages);
        }

        Guid id;
        try
        {
            id = await handler.HandleAsync(user);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return Problem((e.Message));
        }
        return Created($"/users/{id}", id);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers([FromServices]IQueryAllUsersHandler handler)
    {
        var users = await handler.HandleAsync();
        return Ok(users);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id, 
        [FromServices]IQueryUserHandler handler, 
        [FromServices]IOptions<ApiSettings> options)
    {
        if (Request.Headers.TryGetValue("X-API-KEY", out var apiKey) && 
            apiKey.FirstOrDefault() == options.Value.UserApiKey)
        {
            var user = await handler.HandleAsync(id);
            return user == null ? NotFound() : Ok(user);
        }
    
        if (User?.Identity?.IsAuthenticated == true)
        {
            var user = await handler.HandleAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        return Unauthorized();
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IResult> UpdateUser(Guid id,[FromServices]IUpdateUserHandler handler
        ,[FromBody]UpdateUserDto user,[FromServices]IValidator<UpdateUserDto> validator) {
            var result = validator.Validate(user);

            if (!result.IsValid)
            {
                var messages = result.Errors
                    .ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
                return Results.BadRequest(messages);
            }

            var currentUserId = User.GetUserId();
    
            try
            {
                await handler.HandleAsync(id, user,currentUserId);
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
        }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IResult> DeleteUser( Guid id,
        [FromServices] IDeleteUserHandler handler)
    {
        var currentUserId = User.GetUserId();
        try
        {
            await handler.HandleAsync(id, currentUserId);
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
    }

    [HttpGet("email/{email}")]
    [Authorize]
    public async Task<IResult> GetUSerByEmail(string email, [FromServices] IQueryUserByEmailHandler handler)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Results.BadRequest("L'adresse e-mail est requise");
    
        var currentUserEmail = User.GetEmail();
    
        try {
            var user = await handler.HandleAsync(email,currentUserEmail);
            return user == null ? Results.NotFound() : Results.Ok(user);
        }
        catch (ArgumentException e) {
            return Results.BadRequest(e.Message);
        }
        catch (Exception e) {
            return Results.Problem(e.Message);
        }
    }

    [HttpGet("search/{name}")]
    [Authorize]
    public async Task<IResult> searchUserByName(string name,[FromServices]ISearchByNameUserHandler handler)
    {
        
            if (string.IsNullOrWhiteSpace(name))
                return Results.BadRequest("Le terme de recherche est requis");
        
            var results = await handler.HandleAsync(name);
            return Results.Ok(results);
    }

    [HttpGet("signed-in")]
    [Authorize]
    public async Task<List<AbstractUser?>> getSigned([FromServices]IQueryConnectedUsersHandler handler)
    {
        
            var connectedUsers = await handler.HanleAsync();
            return connectedUsers;

        
    }


    [HttpGet("me")]
    [Authorize]
    public async Task<IResult> Me([FromServices] IQueryUserHandler handler)
    {
            var currentUserId = User.GetUserId();
            var user = await handler.HandleAsync(currentUserId);
            return Results.Ok(user);
   
    }
}

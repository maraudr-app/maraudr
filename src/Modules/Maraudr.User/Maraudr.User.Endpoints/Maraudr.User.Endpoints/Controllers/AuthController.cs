using System.Security.Claims;
using Application.DTOs.AuthenticationQueriesDto.Requests;
using Application.UseCases.Tokens.Authentication.AuthenticateUser;
using Application.UseCases.Tokens.Authentication.RefreshToken;
using Application.UseCases.Users.User.LogoutUser;
using FluentValidation;
using Maraudr.User.Endpoints.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maraudr.User.Endpoints;
[ApiController]
[Route("api/auth")]
public class AuthController: ControllerBase
{

    [HttpPost]
    public async Task<IResult>  Log([FromBody] LoginRequestDto request, [FromServices] IAuthenticateUserHandler handler,
        [FromServices] IValidator<LoginRequestDto> validator)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);
    
        var result = await handler.HandleAsync(request);
        if (!result.Success)
            return Results.BadRequest(result.Errors);
        
        return Results.Ok(new { 
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            ExpiresIn = result.ExpiresIn
        });
    }

    // Permet uniquement de revoke ler refresh token 
    // LA suppression du bearer doit se faire côté client 
    [HttpPost("logout")]
    [Authorize]
    public async Task<IResult> Logout([FromServices]ILogoutUserHandler handler){
  
        var currentUserId = User.GetUserId();
        await handler.HandleAsync(currentUserId);
        return Results.Ok();
    }

    [HttpPost("refresh")]
    public async Task<IResult> RefreshToken([FromBody] string request,
        [FromServices] IRefreshTokenHandler handler)
    {
        var result = await handler.HandleAsync(request);
        if (!result.Success)
            return Results.Problem("Token invalide ou expiré, veuillez vous reconnecter");
        
        return Results.Ok(new { 
            AccessToken = result.AccessToken,
            RefreshToken = request,
            ExpiresIn = result.AccessToken
        });
    }

    [HttpGet("validate")]
    public async Task<IResult> ValidateToken()
    {
        var claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value });
        return Results.Ok(claims);
    }




}
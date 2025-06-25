using System.Security.Claims;
using Application.DTOs.AuthenticationQueriesDto.Requests;
using Application.DTOs.InvitationDto;
using Application.UseCases.Tokens;
using Application.UseCases.Tokens.Authentication.AuthenticateUser;
using Application.UseCases.Tokens.Authentication.RefreshToken;
using Application.UseCases.Tokens.RefreshPasswordToken;
using Application.UseCases.Users.User.LogoutUser;
using FluentValidation;
using Maraudr.User.Endpoints.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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

    [HttpGet("password-reset/validate")]
    public async Task<IResult> ValidateToken()
    {
        var claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value });
        return Results.Ok(claims);
    }
    
    [HttpPost("password-reset/initiate")]
    [Consumes("application/json")]
    [EnableRateLimiting("password-reset")] 

    public async Task<IActionResult> InitiateReset([FromServices] IInitiatePasswordResetAsync handler,[FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        await handler.HandleAsync(request.Email);
        return Ok(new { message = "Si votre email existe dans notre système, vous recevrez un lien de réinitialisation." });
    }

    [HttpGet("password-reset/validate/{token}")]
    public async Task<IActionResult> ValidateToken([FromServices] IValidateResetTokenHandler handler, string token)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest("Token invalide");

        var isValid = await handler.HandleAsync(token);
        
        if (!isValid)
            return BadRequest("Token invalide ou expiré");

        return Ok(new { message = "Token valide" });
    }

    [HttpPost("password-reset/confirm")]
    [Consumes("application/json")]
    public async Task<IActionResult> ConfirmReset([FromServices] IResetPasswordHandler handler,[FromBody] ConfirmResetRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await handler.HandleAsync(request);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
        return Ok(new { message = "Mot de passe réinitialisé avec succès" });
    }

    [HttpPost("invitation-link/send")]
    [Authorize]
    public async Task<IActionResult> SendInvitation([FromBody] SendInvitationRequest request, [FromServices] ISendInvitationRequestHandler handler)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = User.GetUserId();
        
        try
        {
            await handler.HandleAsync(
                currentUserId, request);

            return Ok(new { message = "Invitation envoyée avec succès" });


        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }



}
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using FluentValidation;
using Maraudr.Authentication.Application.UseCases.AuthenticateUser;
using Maraudr.Authentication.Domain.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration des services
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    // Configuration JWT
})
.AddGoogle(options => {
    // Configuration Google Auth
});

var app = builder.Build();

// Endpoints d'authentification de base
app.MapPost("/auth/login", async (
    [FromBody] LoginRequestDto request, 
    IAuthenticateUserHandler handler,
    IValidator<LoginRequestDto> validator) =>
{
    var validationResult = validator.Validate(request);
    if (!validationResult.IsValid)
        return Results.BadRequest(validationResult.Errors);
    
    var result = await handler.HandleAsync(request);
    if (!result.Success)
        return Results.Unauthorized();
        
    return Results.Ok(new { 
        AccessToken = result.AccessToken,
        RefreshToken = result.RefreshToken,
        ExpiresIn = result.ExpiresIn
    });
});

app.MapPost("/auth/register", async (
    [FromBody] RegisterRequestDto request,
    IAuthService authService,
    IValidator<RegisterRequestDto> validator) =>
{
    var validationResult = validator.Validate(request);
    if (!validationResult.IsValid)
        return Results.BadRequest(validationResult.Errors);

    var result = await authService.RegisterUserAsync(request);
    if (!result.Success)
        return Results.BadRequest(result.Errors);
        
    return Results.Created($"/users/{result.UserId}", new { UserId = result.UserId });
});

app.MapPost("/auth/refresh", async (
    [FromBody] RefreshTokenRequestDto request,
    IAuthService authService) =>
{
    var result = await authService.RefreshTokenAsync(request.RefreshToken);
    if (!result.Success)
        return Results.Unauthorized();
        
    return Results.Ok(new { 
        AccessToken = result.AccessToken,
        RefreshToken = result.RefreshToken,
        ExpiresIn = result.ExpiresIn
    });
});

app.MapPost("/auth/logout", [Authorize] async (
    ClaimsPrincipal user,
    IAuthService authService) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    await authService.LogoutUserAsync(userId);
    return Results.Ok();
});

// Vérification du token
app.MapGet("/auth/validate", [Authorize] (ClaimsPrincipal user) =>
{
    var claims = user.Claims.Select(c => new { Type = c.Type, Value = c.Value });
    return Results.Ok(claims);
});

// Authentification externe
app.MapGet("/auth/google", async (HttpContext context) =>
{
    await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
    {
        RedirectUri = "/auth/google-callback"
    });
});

app.MapGet("/auth/google-callback", async (
    HttpContext context,
    IAuthService authService) =>
{
    var result = await authService.HandleGoogleAuthCallbackAsync(context);
    if (!result.Success)
        return Results.Redirect("/login?error=google-auth-failed");
        
    // Rediriger vers l'application front-end avec le token
    return Results.Redirect($"/login/success?token={result.AccessToken}");
});

// Gestion des mots de passe
app.MapPost("/auth/forgot-password", async (
    [FromBody] ForgotPasswordRequestDto request,
    IAuthService authService) =>
{
    await authService.SendPasswordResetLinkAsync(request.Email);
    return Results.Ok(new { Message = "Si cette adresse est associée à un compte, un email de réinitialisation a été envoyé." });
});

app.MapPost("/auth/reset-password", async (
    [FromBody] ResetPasswordRequestDto request,
    IAuthService authService,
    IValidator<ResetPasswordRequestDto> validator) =>
{
    var validationResult = validator.Validate(request);
    if (!validationResult.IsValid)
        return Results.BadRequest(validationResult.Errors);
        
    var result = await authService.ResetPasswordAsync(request.Token, request.Password);
    if (!result.Success)
        return Results.BadRequest(new { Message = "Impossible de réinitialiser le mot de passe." });
        
    return Results.Ok(new { Message = "Mot de passe réinitialisé avec succès." });
});

// Routes administratives d'authentification
app.MapPost("/auth/revoke/{userId}", [Authorize(Roles = "Admin")] async (
    Guid userId,
    IAuthService authService) =>
{
    await authService.RevokeUserTokensAsync(userId);
    return Results.Ok();
});

app.MapGet("/auth/active-sessions", [Authorize(Roles = "Admin")] async (
    IAuthService authService) =>
{
    var sessions = await authService.GetActiveSessionsAsync();
    return Results.Ok(sessions);
});
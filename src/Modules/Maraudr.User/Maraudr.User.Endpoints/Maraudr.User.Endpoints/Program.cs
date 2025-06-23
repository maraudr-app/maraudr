using System.Security.Claims;
using Application;
using Application.DTOs.UsersQueriesDtos.Requests;
using Application.UseCases.Users.Manager.AddUserToManagersTeam;
using Application.UseCases.Users.Manager.QueryManagersTeam;
using Application.UseCases.Users.Manager.RemoveUserFromManagersTeam;

using Maraudr.User.Endpoints;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Endpoints.Identity;
using Maraudr.User.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddValidation();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowFrontend3000");
app.MapControllers(); 

// MANAGER TEAM
app.MapGet("/managers/team/{managerGuid:guid}", [Authorize] async (
Guid managerGuid, IQueryManagersTeamHandler handler,
        ClaimsPrincipal currentUser) =>
    {
        var currentUserId = currentUser.GetUserId();
    
        IEnumerable<AbstractUser> team; 
        try
        {
            team = await handler.HandleAsync(managerGuid,currentUserId);
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


app.MapPost("/managers/team/{managerId:guid}", [Authorize] async (
Guid managerId, ClaimsPrincipal currentUser,
        [FromBody] UserIdRequest request, 
        IAddUserToManagersTeamHandler handler,
        ClaimsPrincipal user) =>
    {
        var currentUserId = currentUser.GetUserId();     
        try
        {
            await handler.HandleAsync(managerId,currentUserId, request.UserId);
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




app.MapDelete("/managers/team/{managerId:guid}", async ([FromBody] UserIdRequest request, 
        IRemoveUserFromManagerTeamHandler handler,
        ClaimsPrincipal currentUser, Guid managerId) =>
    {
        var currentUserId = currentUser.GetUserId();     
    
        
        try
        {
            await handler.HandleAsync(managerId,currentUserId, request.UserId);
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




/*










// AUTHENTICATION 



    



/*


// Vérification du token


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
});* /


*/
app.Run();

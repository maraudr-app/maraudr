using System.Security.Claims;
using System.Text;
using Application;
using Application.DTOs.AuthenticationQueriesDto.Requests;
using Application.DTOs.UsersQueriesDtos.Requests;
using Application.UseCases.Tokens.Authentication.AuthenticateUser;
using Application.UseCases.Tokens.Authentication.RefreshToken;
using Application.UseCases.Users.Manager.AddUserToManagersTeam;
using Application.UseCases.Users.Manager.QueryManagersTeam;
using Application.UseCases.Users.Manager.RemoveUserFromManagersTeam;
using Application.UseCases.Users.User.CreateUser;
using Application.UseCases.Users.User.DeleteUser;
using Application.UseCases.Users.User.QueryAllUsers;
using Application.UseCases.Users.User.QueryUser;
using Application.UseCases.Users.User.QueryUserByEmail;
using Application.UseCases.Users.User.SearchByNameUser;
using Application.UseCases.Users.User.UpdateUser;
using FluentValidation;
using Maraudr.User.Endpoints;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Endpoints.Identity;
using Maraudr.User.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// TODO : many endpoints require auth -> doit être impléménté assez vite 
// TODO : verifeir unicité via email aussi & numéro de telephone aussi  

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddValidation();



//todo : dependency injection 
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("JWT");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["Secret"])),
            ValidateIssuer = true,
            ValidIssuer = jwtSection["ValidIssuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["ValidAudience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/users", [Authorize] async (IQueryAllUsersHandler handler) =>
{
    var users = await handler.HandleAsync();
    return Results.Ok(users);
});

app.MapGet("/users/{id:guid}", [Authorize] async (Guid id, IQueryUserHandler handler ) => {
    var user = await handler.HandleAsync(id);
    return user == null ? Results.NotFound() : Results.Ok(user);
});

//update user info
//tésté
app.MapPut("/users/{id:guid}", [Authorize] async (ClaimsPrincipal userClaim,Guid id, UpdateUserDto user,
    IUpdateUserHandler handler, IValidator<UpdateUserDto> validator) => {
    var result = validator.Validate(user);

    if (!result.IsValid)
    {
        var messages = result.Errors
            .ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
        return Results.BadRequest(messages);
    }

    var currentUserId = userClaim.GetUserId();
    
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
});
//tésté
app.MapDelete("/users/{id:guid}",[Authorize] async (ClaimsPrincipal userClaim, Guid id,IDeleteUserHandler handler) =>
{
    var currentUserId = userClaim.GetUserId();
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
});

// doesnt work yet

app.MapGet("users/email/{email}", [Authorize] async (ClaimsPrincipal userClaim,string email, IQueryUserByEmailHandler handler) => {
    if (string.IsNullOrWhiteSpace(email))
        return Results.BadRequest("L'adresse e-mail est requise");
    
    var currentUserEmail = userClaim.GetEmail();
    
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
});


// MANAGER TEAM
app.MapGet("/managers/{managerGuid:guid}/team", [Authorize] async (
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


app.MapPost("/managers/team/add-user/{managerId:guid}", [Authorize] async (
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



// AUTHENTICATION 


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
        return Results.BadRequest(result.Errors);
        
    return Results.Ok(new { 
        AccessToken = result.AccessToken,
        RefreshToken = result.RefreshToken,
        ExpiresIn = result.ExpiresIn
    });
});

    
    
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
    ;
app.MapPost("/auth/refresh", async (
    [FromBody] string request,
    IRefreshTokenHandler handler) =>
{
    var result = await handler.HandleAsync(request);
    if (!result.Success)
        return Results.Problem("Token invalide ou expiré, veuilelz vous reconnecter");
        
    return Results.Ok(new { 
        AccessToken = result.AccessToken,
        RefreshToken = request,
        ExpiresIn = result.AccessToken
    });
});



app.MapGet("/auth/validate", [Authorize] (ClaimsPrincipal user) =>
{
    var claims = user.Claims.Select(c => new { Type = c.Type, Value = c.Value });
    return Results.Ok(claims);
});
/*
app.MapPost("/auth/logout", [Authorize] async (
    ClaimsPrincipal user,
    IAuthService authService) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    await authService.LogoutUserAsync(userId);
    return Results.Ok();
});

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
});*/
app.Run();

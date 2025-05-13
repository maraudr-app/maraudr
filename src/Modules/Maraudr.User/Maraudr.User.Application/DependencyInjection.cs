using Application.UseCases.Tokens.Authentication.AuthenticateUser;
using Application.UseCases.Tokens.Authentication.RefreshToken;
using Application.UseCases.Tokens.JwtManagement.GenerateAccessToken;
using Application.UseCases.Tokens.JwtManagement.GenerateRefreshToken;
using Application.UseCases.Tokens.JwtManagement.RefreshToken;
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
using Microsoft.Extensions.DependencyInjection;

namespace Application;


public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateUserHandler, CreateUserHandler>();
        services.AddScoped<IUpdateUserHandler, UpdateUserHandler>();
        services.AddScoped<IQueryAllUsersHandler, QueryAllUsersHandler>();
        services.AddScoped<IQueryUserHandler, QueryUserHandler>();
        services.AddScoped<IDeleteUserHandler, DeleteUserHandler>();
        services.AddScoped<IQueryManagersTeamHandler, QueryManagersTeamHandler>();
        services.AddScoped<IAddUserToManagersTeamHandler, AddUserToManagersTeamHandler>();
        services.AddScoped<IRemoveUserFromManagerTeamHandler, RemoveUserFromManagerTeamHandler>();
        services.AddScoped<ISearchByNameUserHandler, SearchByNameUserHandler>();
        services.AddScoped<IQueryUserByEmailHandler, QueryUserByEmailHandler>();
        services.AddScoped<IAuthenticateUserHandler, AuthenticateUserHandler>();
        services.AddScoped<IGenerateAccessTokenHandler, GenerateAccessTokenHandler>();
        services.AddScoped<IGenerateRefreshTokenHandler, GenerateRefreshTokenHandler>();
        services.AddScoped<IRefreshTokenHandler, RefreshTokenHandler>();
    }
}

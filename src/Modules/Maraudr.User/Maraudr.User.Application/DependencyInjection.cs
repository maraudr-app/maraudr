using Application.UseCases.Manager.AddUserToManagersTeam;
using Application.UseCases.Manager.QueryManagersTeam;
using Application.UseCases.Manager.RemoveUserFromManagersTEam;
using Application.UseCases.User.CreateUser;
using Application.UseCases.User.DeleteUser;
using Application.UseCases.User.QueryAllUsers;
using Application.UseCases.User.QueryUser;
using Application.UseCases.User.UpdateUser;
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
    }
}

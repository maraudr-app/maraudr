using Application.UseCases.User.CreateUser;
using Application.UseCases.User.UpdateUser;
using Microsoft.Extensions.DependencyInjection;

namespace Application;


public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateUserHandler, CreateUserHandler>();
        services.AddScoped<IUpdateUserHandler, UpdateUserHandler>();
        
    }
}

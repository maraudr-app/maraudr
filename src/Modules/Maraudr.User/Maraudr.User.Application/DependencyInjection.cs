using Application.UseCases.User.CreateUser;
using Microsoft.Extensions.DependencyInjection;

namespace Application;


public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateUserHandler, CreateUserHandler>();
    }
}

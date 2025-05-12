using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Infrastructure.Repositories;
using Maraudr.User.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.User.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<UserContext>(options =>
            options.UseInMemoryDatabase("UserDatabase"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordManager, PasswordManager>();

    }
}

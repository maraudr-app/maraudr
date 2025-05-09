using Application.DTOs.Requests;
using Application.Validators;
using FluentValidation;
using Maraudr.User.Application.DTOs.Requests;
using Maraudr.User.Application.Validators;

namespace Maraudr.User.Endpoints;

public static class DependencyInjection
{
    public static void AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidator>();
        services.AddScoped<IValidator<UpdateUserDto>, UpdateUserDtoValidator>();
    }
}
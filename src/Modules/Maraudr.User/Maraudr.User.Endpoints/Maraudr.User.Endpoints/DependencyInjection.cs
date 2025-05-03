using Application.DTOs.Requests;
using Application.Validators;
using FluentValidation;
using Maraudr.Stock.Application.Validation;

namespace Maraudr.User.Endpoints;

public static class DependencyInjection
{
    public static void AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidator>();
    }
}
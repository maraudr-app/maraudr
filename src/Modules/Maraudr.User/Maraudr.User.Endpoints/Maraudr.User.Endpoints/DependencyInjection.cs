using Application.DTOs.AuthenticationQueriesDto.Requests;
using Application.DTOs.UsersQueriesDtos.Requests;
using Application.Validators;
using FluentValidation;
namespace Maraudr.User.Endpoints;

public static class DependencyInjection
{
    public static void AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidator>();
        services.AddScoped<IValidator<UpdateUserDto>, UpdateUserDtoValidator>();
        services.AddScoped<IValidator<LoginRequestDto>, LoginRequestDtoValidator>();
    }
}
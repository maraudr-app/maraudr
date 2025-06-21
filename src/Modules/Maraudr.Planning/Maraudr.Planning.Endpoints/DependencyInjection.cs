
using FluentValidation;
using Maraudr.Planning.Application.DTOs;
using Maraudr.Planning.Application.Validators;

namespace Maraudr.Planning.Endpoints;

public static class DependencyInjection
{
    public static void AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateEventDto>, CreateEventDtoValidator>();
      
    }
}
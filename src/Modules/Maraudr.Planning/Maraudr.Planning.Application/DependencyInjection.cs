using Maraudr.Planning.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.Planning.Application;


public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    
    {
        services.AddScoped<ICreateAnEventHandler, CreateAnEventHandler>();
        services.AddScoped<IDeleteAnEventHandler, DeleteAnEventHandler>();
        services.AddScoped<IGetAllAssociationEventsHandler, GetAllAssociationEventsHandler>();
        services.AddScoped<IGetAllEventsOfUserHandler,GetAllEventsOfUserHandler>();
        services.AddScoped<ICreatePlanningHandler,CreatePlanningHandler>();
        services.AddScoped<IGetAnEventByIdHandler, GetAnEventByIdHandler>();


    }
}

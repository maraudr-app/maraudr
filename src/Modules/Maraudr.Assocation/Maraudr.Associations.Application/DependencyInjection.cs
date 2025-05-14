using Maraudr.Associations.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.Associations.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddHttpClient("siret", client =>
        {
            client.BaseAddress = new Uri("https://siva-integ1.cegedim.cloud/apim/api-asso/api/structure/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddScoped<ICreateAssociationHandler, CreateAssociation>();
        services.AddScoped<IGetAssociationHandler, GetAssociation>();
        services.AddScoped<IVerifyAssociationBySiret, VerifyAssociationBySiret>();
        services.AddScoped<IUnregisterAssociation, UnregisterAssociation>();
    }
}
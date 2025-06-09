using FluentValidation;
using Maraudr.Associations.Application.Dtos;
using Maraudr.Associations.Application.UseCases.Command;
using Maraudr.Associations.Application.UseCases.Query;
using Maraudr.Associations.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.Associations.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddHttpClient("siret", client =>
        {
            client.BaseAddress = new Uri("https://siva-integ1.cegedim.cloud/apim/api-asso/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        
        services.AddHttpClient("stock", client =>
        {
            client.BaseAddress = new Uri("http://stock:8080");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddScoped<IGetAssociationHandler, GetAssociation>();
        services.AddScoped<IGetAssocationsOfUserHandler, GetAssociationsOfUser>();
        services.AddScoped<IUnregisterAssociation, UnregisterAssociation>();
        services.AddScoped<IAddMemberToAssociationHandler, AddMemberToAssociation>();
        services.AddScoped<ICreateAssociationHandlerSiretIncluded, CreateAssociationSiretIncluded>();
        services.AddScoped<ISearchAssociationsByNameHandler, SearchAssociationsByName>();
        services.AddScoped<IUpdateAssociationHandler, UpdateAssociation>();
        services.AddScoped<ISearchAssociationsByCityHandler, SearchAssociationsByCity>();
        services.AddScoped<IListAssociationsPaginatedHandler, ListAssociationsPaginated>();
        services.AddScoped<IValidator<AddressDto>, AddressDtoValidator>();
        services.AddScoped<IValidator<UpdateAssociationInformationDto>, UpdateAssociationInformationDtoValidator>();
        services.AddScoped<IIsUserMemberOfAssociationHandler, IsUserMemberOfAssociationHandler>();

    }
}
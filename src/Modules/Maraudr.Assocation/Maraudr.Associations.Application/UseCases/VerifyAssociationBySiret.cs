using Maraudr.Associations.Domain.Entities;

namespace Maraudr.Associations.Application.UseCases;

public interface IVerifyAssociationBySiret
{
    Task<bool> HandleAsync(string siret, IHttpClientFactory factory);
}

public class VerifyAssociationBySiret(IAssociations repository) : IVerifyAssociationBySiret
{
    public async Task<bool> HandleAsync(string siret, IHttpClientFactory factory)
    {
        using var client =  factory.CreateClient("siret");
        using var response = await client.GetAsync(siret);
        
        Console.WriteLine(response.StatusCode);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        
        var content =  await response.Content.ReadAsStringAsync();

        var association = await repository.GetAssociationBySiret(siret);

        if (!content.Contains("statusCode: 404") && association != null)
        {
            association.IsVerified = true;
            await repository.UpdateAssociation(association);
            return true;
        }

        return false;

    }
}
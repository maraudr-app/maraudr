using System.Net.Http.Json;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Options;

namespace Maraudr.User.Infrastructure.Repositories;



public class AssociationRepository(HttpClient httpClient, IOptions<ApiSettings> options):IAssociationRepository
{
    public async Task<bool> AssociationExists(Guid id)
    {
        var url = options.Value.AssociationApiUrl + $"association?id={id}";
    
        try 
        {
            var response = await httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<bool> IsUserMemberOfAssociationAsync(Guid userId,Guid associationId)
    {
        var url = options.Value.AssociationApiUrl + $"association/is-member/{associationId}/{userId}";
      
        try
        {
            var response = await httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {responseString}");
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: HTTP status code {response.StatusCode}");
                return false;
            }
            
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false; 
        }

    }

    public async Task<string> GetAssociationName(Guid associationId)
    {
        var url = options.Value.AssociationApiUrl + $"association/?id={associationId}";

        try
        {
            var response = await httpClient.GetAsync(url);
        
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Erreur: Code HTTP {response.StatusCode}");
                throw new HttpRequestException($"Impossible de récupérer les informations de l'association: {response.StatusCode}");
            }
        
            var association = await response.Content.ReadFromJsonAsync<AssociationDto>();
            return association.name;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erreur lors de la récupération du nom de l'association: {e.Message}");
            throw;
        }
    }
    

}    

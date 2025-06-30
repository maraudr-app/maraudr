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
        Console.WriteLine($"URL: {url}");
        Console.WriteLine($"AssociationId: {associationId}");
        Console.WriteLine($"UserId: {userId}");
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
    
}    

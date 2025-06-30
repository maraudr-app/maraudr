using System.Net.Http.Json;
using Maraudr.MCP.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Maraudr.MCP.Infrastructure.Repositories;

public class StockRepository(HttpClient httpClient, IOptions<ApiSettings> options):IStockRepository
{
    public async Task<StockItemDto?> GetStockItemByBarCodeAsync(string code,Guid associationId)
    {
        try
        {
            var url = options.Value.StockApiUrl + $"item/{code}/?associationId={associationId}";
            var response = await httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<StockItemDto>();
            }
            
            return null;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    


    public Task<IEnumerable<StockItemDto?>> GetStockItemByTypeAsync(Category type,Guid associationId)
    {
        throw new NotImplementedException();
    }
}
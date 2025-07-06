using System.Net.Http.Json;
using Maraudr.MCP.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Maraudr.MCP.Infrastructure.Repositories;

public class StockRepository(HttpClient httpClient, IOptions<ApiSettings> options):IStockRepository
{
    
    private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "stock_repository.log");

  
    
    private void LogToFile(string message)
    {
        try
        {
            var logMessage = $"{DateTime.UtcNow:O} - {message}{Environment.NewLine}";
            File.AppendAllText(LogFilePath, logMessage);
        }
        catch
        {
            // Ignorer les erreurs de journalisation
        }
    }

    public async Task<StockItemDto?> GetStockItemByBarCodeAsync(string code,Guid associationId)
    {
        try
        {
            var url = options.Value.StockApiUrl + $"stock/item/{code}/?associationId={associationId}";
           LogToFile("------------------------------------------------------");
           LogToFile("URL stock:" + url);
           LogToFile("------------------------------------------------------");
            var response = await httpClient.GetAsync(url);
            LogToFile("------------------------------------------------------");
            LogToFile("Response " + response);
            LogToFile("------------------------------------------------------");
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

    public async Task<StockItemDto?> GetStockItemByName(string name, Guid associationId,string jwt)
    {
        try
        {
            LogToFile("------------------------------------------------------");
            LogToFile("Début de retrait des élements du stock");

            LogToFile("------------------------------------------------------");

            var url = options.Value.StockApiUrl + $"stock/items?associationId={associationId}&name={name}";
            LogToFile($"URL {url}");

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                LogToFile("Succes");

                return await response.Content.ReadFromJsonAsync<StockItemDto?>();
            }

            LogToFile($"Echec : {response}");


            return null;
        }
        catch (Exception e)
        {
            LogToFile($"Echec : {e.Message}");
            LogToFile($"Echec : {e.StackTrace}");

            return null;
        }
    }


    public async Task<IEnumerable<StockItemDto?>> GetStockItemByTypeAsync(Category type,Guid associationId)
    {
        try
        {
            var url = $"{options.Value.StockApiUrl}stock/items?associationId={associationId}&category={type}";        
            var response = await httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<StockItemDto?>>();
            }
            
            return null;
        }
        catch (Exception e)
        {
            return null;
        }
        
    }

    public async Task<IEnumerable<StockItemDto>> GetStockItemsAsync(Guid associationId,string jwt)
    {
        try
        {
            LogToFile("------------------------------------------------------");
            LogToFile("Début de retrait des élements du stock");

            LogToFile("------------------------------------------------------");

            var url = options.Value.StockApiUrl + $"stock/items?associationId={associationId}";
            LogToFile($"URL {url}");

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            var response = await httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                LogToFile("Succes");

                return await response.Content.ReadFromJsonAsync<IEnumerable<StockItemDto?>>();
            }
            LogToFile($"Echec : {response}");

            
            return null;
        }
        catch (Exception e)
        {
            LogToFile($"Echec : {e.Message}");
            LogToFile($"Echec : {e.StackTrace}");

            return null;
        }
        
    }
}
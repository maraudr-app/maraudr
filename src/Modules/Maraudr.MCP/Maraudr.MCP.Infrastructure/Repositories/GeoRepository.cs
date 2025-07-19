using System.Net.Http.Json;
using Maraudr.MCP.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Maraudr.MCP.Infrastructure.Repositories;

public class GeoRepository(HttpClient httpClient,IOptions<ApiSettings> options):IGeoRepository
{
    private static readonly string LogFilePath = Path.Combine("/tmp", "geo_repository.log");  
    
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
    public async Task<IEnumerable<GeoDataDto>> GetAllInterestPoints(Guid associationId, int days, string jwt)
    {
        try
        {
            var url = options.Value.GeoUrl + $"geo/{associationId}?days={days}";
            LogToFile("------------------------------------------------------");
            LogToFile("URL stock:" + url);
            LogToFile("------------------------------------------------------");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var response = await httpClient.GetAsync(url);
            
            LogToFile("------------------------------------------------------");
            LogToFile("Response " + response);
            LogToFile("------------------------------------------------------");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<GeoDataDto>>();
            }
            LogToFile("------------------------------------------------------");
            LogToFile("Echec total lors du retrait");
            LogToFile("Reponse du serveur "+response.StatusCode);
            return null;
        }
        catch (Exception e)
        {
            LogToFile("------------------------------------------------------");
            LogToFile("Echec total lors du retrait");
            LogToFile("Exception levée "+e.Message);
            LogToFile("At "+e.StackTrace);
            return null;
        }    }
}
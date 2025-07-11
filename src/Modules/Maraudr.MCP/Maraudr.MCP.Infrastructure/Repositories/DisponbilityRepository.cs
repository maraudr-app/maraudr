using System.Net.Http.Json;
using Maraudr.MCP.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Maraudr.MCP.Infrastructure.Repositories;

public class DisponbilityRepository(HttpClient httpClient, IOptions<ApiSettings> options):IDisponibilityRepository
{
    private static readonly string LogFilePath = Path.Combine("/tmp", "disponibility-repository.log");    
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
    public async Task<IEnumerable<DisponibilityDto>> GetMyDisponibilitiesInAssociation(Guid associationId,string jwt)
    {
        var associationUrl = $"{options.Value.UserApiUrl}api/users/disponibilities/{associationId}";
        LogToFile("Association URL : " + associationUrl);

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",jwt);
        var response = await httpClient.GetAsync(associationUrl);
        try
        {
            if (!response.IsSuccessStatusCode)
            {
                LogToFile("Error while getting user disponibility in association");
                LogToFile($"Response status :"+response.StatusCode);
                return null;
            }
            LogToFile("Success");
            return await response.Content.ReadFromJsonAsync<IEnumerable<DisponibilityDto?>>();
          
        }
        catch (Exception ex)
        {
            LogToFile("Error while getting all events from PlanningRepository");
            LogToFile($"Error:{ex.StackTrace}");
            return null;
        }    
    }
    
    
    public async Task<IEnumerable<DisponibilityDto>> GetAllDisponibilitiesInAssociation(Guid associationId,string jwt)
    {
        var associationUrl = $"{options.Value.UserApiUrl}api/users/disponibilities/all/{associationId}";
        LogToFile("Association URL : " + associationUrl);

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",jwt);
        var response = await httpClient.GetAsync(associationUrl);
        try
        {
            if (!response.IsSuccessStatusCode)
            {
                LogToFile("Error while getting user disponibility in association");
                LogToFile($"Response status :"+response.StatusCode);
                return null;
            }
            LogToFile("Success");
            return await response.Content.ReadFromJsonAsync<IEnumerable<DisponibilityDto?>>();
          
        }
        catch (Exception ex)
        {
            LogToFile("Error while getting all events from PlanningRepository");
            LogToFile($"Error:{ex.StackTrace}");
            return null;
        }    
    }
}
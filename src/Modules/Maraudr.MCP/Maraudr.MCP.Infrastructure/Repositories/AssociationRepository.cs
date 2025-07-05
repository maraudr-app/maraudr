using System.Net.Http.Json;
using Maraudr.MCP.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Maraudr.MCP.Infrastructure.Repositories;

public class AssociationRepository(HttpClient httpClient, IOptions<ApiSettings> options,IMCPRepository mcpRepository):IAssociationRepository
{
    private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "association_repository.log");
    
    
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
    
    
    public async Task<AssociationDto> GetAssociationByName(string name)
    {
        var jwt =mcpRepository.GetUserJwt();

        var planningUrl = $"{options.Value.PlanningApiUrl}association/membership";
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        var response = await httpClient.GetAsync(planningUrl);
        try
        {
            if (!response.IsSuccessStatusCode)
            {
                LogToFile("Error while getting all events from PlanningRepository");
                LogToFile($"Response status :"+response.StatusCode);
                return null;
            }
            var value =await response.Content.ReadFromJsonAsync<IEnumerable<AssociationDto?>>();
            return value.FirstOrDefault(asso => asso?.Name == name);
        }
        catch (Exception ex)
        {
            LogToFile("Error while getting all events from PlanningRepository");
            LogToFile($"Error:{ex.StackTrace}");
            return null;
        }
        
    }
    
    
    public async Task<IEnumerable<AssociationDto>> GetUserAssociations()
    {
        var jwt =mcpRepository.GetUserJwt();
     
        
        var associationUrl = $"{options.Value.AssociationUrl}association/membership";
        
        LogToFile("Bearer: "+jwt);

        LogToFile("------------------------------------------------------");
        LogToFile("URL association url:" + associationUrl);
        LogToFile("------------------------------------------------------");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        var response = await httpClient.GetAsync(associationUrl);
        try
        {
            if (!response.IsSuccessStatusCode)
            {

                LogToFile("Error while getting all events from PlanningRepository");
                LogToFile($"Response status :"+response.StatusCode);
                return null;
                return null;
                
            }
            LogToFile("------------------------------------------------------");
            LogToFile("API Success" + response);
            LogToFile("------------------------------------------------------");
            var value =await response.Content.ReadFromJsonAsync<IEnumerable<AssociationDto?>>();
            LogToFile("------------------------------------------------------");
            LogToFile("Value from request " + value);
            LogToFile("------------------------------------------------------");
            return value;
            
        }
        catch (Exception ex)
        {
            LogToFile("Error while getting all events from Association API");
            LogToFile($"Error:{ex.StackTrace}");
            return null;
        }
        
    }
}
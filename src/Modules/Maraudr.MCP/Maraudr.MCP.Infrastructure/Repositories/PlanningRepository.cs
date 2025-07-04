using System.Net.Http.Json;
using Maraudr.MCP.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Maraudr.MCP.Infrastructure.Repositories;

public class PlanningRepository(HttpClient httpClient, IOptions<ApiSettings> options):IPlanningRepository
{
    private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "planning_repository.log");
    
    
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

    public async Task<IEnumerable<EventDto>> GetAllAssociationEventsAsync(Guid associationId)
    {
        var planningUrl = $"{options.Value.PlanningApiUrl}all-events/{associationId}";
        var response = await httpClient.GetAsync(planningUrl);
        try
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<EventDto?>>();
            }

            return null;
        }
        catch (Exception ex)
        {
            LogToFile("Error while getting all events from PlanningRepository");
            LogToFile($"Error:{ex.StackTrace}");
            return null;
        }

    }
    
}
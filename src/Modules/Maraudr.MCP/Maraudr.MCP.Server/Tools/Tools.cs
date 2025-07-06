using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Maraudr.MCP.Domain.Interfaces;
using MCP.Maraudr.Application.Dtos;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;

namespace Maraudr.MCP.Server.Tools;

[McpServerToolType]
public class Tools(IAssociationRepository associationRepository,IStockRepository stockRepository,IPlanningRepository planningRepository)
{
    private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "mcp_server_tools.log");
    
    /// <summary>
    /// Logs exceptions to a local file to avoid writing to stdout.
    /// </summary>
    private static void LogError(Exception ex, string methodName)
    {
        try
        {
            var logMessage = $"{DateTime.UtcNow:O} - ERROR in {methodName}:{Environment.NewLine}" +
                             $"Message: {ex.Message}{Environment.NewLine}" +
                             $"StackTrace: {ex.StackTrace}{Environment.NewLine}" +
                             "--------------------------------------------------"+ Environment.NewLine;
            File.AppendAllText(LogFilePath, logMessage);
        }
        catch
        {
            // Ignorer pour ne pas planter le serveur
        }
    }
    private static void LogMessage(string message)
    {
        try
        {
            var logMessage = $"{DateTime.UtcNow:O} - INFO: {message}{Environment.NewLine}";
            File.AppendAllText(LogFilePath, logMessage);
        }
        catch
        {
            // Ignorer pour ne pas planter le serveur
        }
    }
    
    //------------------------------------STOCK-----------------------------------------------------


    [McpServerTool, Description("Gets the data of an item given its barcode for an association given its name")]
    public  async Task<StockItemDto?> GetStock(string barcode, Guid associationId)
    {
        LogMessage($"Début de GetStock avec barcode={barcode}, associationId={associationId}");
    
        try
        {
          
        
            LogMessage("Création du scope");
        
            LogMessage("Tentative d'obtention de IStockRepository");
        
            if (stockRepository == null)
            {
                LogMessage("IStockRepository est null - service non disponible");
                throw new InvalidOperationException("IStockRepository service is not available.");
            }
        
            LogMessage($"IStockRepository obtenu, type: {stockRepository.GetType().FullName}");
            LogMessage($"Appel de GetStockItemByBarCodeAsync");
        
            var result = await stockRepository.GetStockItemByBarCodeAsync(barcode, associationId);
        
            LogMessage($"Résultat obtenu: {(result == null ? "null" : $"Item avec ID {result.Id}")}");
            return result;
        }
        catch (Exception e)
        {
            LogMessage($"Exception dans GetStock: {e.Message}\nStackTrace: {e.StackTrace}");
            LogError(e, nameof(GetStock));
            return null;
        }
    }

    [McpServerTool, Description("Gets all the items of the stock given the id of an association")]
    public  async Task<IEnumerable<StockItemDto>?> GetAllStockItems(Guid associationId)
    {
        try
        {
            
            if (stockRepository == null)
            {
                throw new InvalidOperationException("IStockRepository service is not available.");
            }
            
            return await stockRepository.GetStockItemsAsync(associationId);
        }
        catch (Exception e)
        {
            LogError(e, nameof(GetAllStockItems));
            return null;
        }
    }
    
    //------------------------------------ASSOCIATIONS-----------------------------------------------------
    [McpServerTool, Description("Gets all the associations which the user is a member of")]
    public async Task<IEnumerable<AssociationDto>?> GetUserAssociations(string jwt)
    {
        LogMessage($"Début de récupération des associations de l'utilisateur ");
        LogMessage($"--------------------------------------------------------------------------------");
        try
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                LogMessage("Bearer token null, user not logued in");
                return null;
            }
            LogMessage($"Bearer {jwt}");


            if (associationRepository == null)
            {
                LogMessage("IAssociationRepository est null - service non disponible");
                throw new InvalidOperationException("IAssociationRepository service is not available.");
            }
            LogMessage($"IAssociationRepository obtenu, type: {associationRepository.GetType().FullName}");
            LogMessage($"Appel de GetAllAssociations");
            var result = await associationRepository.GetUserAssociations(jwt);
            LogMessage($"Résultat obtenu");
            return result;

        }
        catch (Exception e)
        {
            LogMessage($"Une erreur s'est déclenchée ");
            LogMessage($"Erreur : {e.Message}");
            LogMessage($"StackTrace : {e.StackTrace}");
            
            LogMessage($"--------------------------------------------------------------------------------");
            return null;
        }
    }
    
    
    
    //------------------------------------EVENTS-----------------------------------------------------

    [McpServerTool, Description("Gets all the events of an association given its name for a user")]
    public async Task<IEnumerable<EventDto>?> GetAllEventsOfAnAssociation(string associationName,string jwt)
    {
        LogMessage($"Début de récupération des évènements de l'association avec l'ID {associationName}");
        LogMessage($"--------------------------------------------------------------------------------");
        try
        {

            if (string.IsNullOrWhiteSpace(jwt))
            {
                LogMessage("Bearer token null, user not logued in");
                return null;
            }
            LogMessage($"Bearer {jwt}");
            if (planningRepository == null)
            {
                LogMessage("IPlanningRepository est null - service non disponible");
                throw new InvalidOperationException("IPlanningRepository service is not available.");
            }
            if (associationRepository == null)
            {
                LogMessage("IAssociationRepository est null - service non disponible");
                throw new InvalidOperationException("IAssociationRepository service is not available.");
            }

            LogMessage($"IPlanningRepository obtenu, type: {planningRepository.GetType().FullName}");
            LogMessage($"IAssociationRepository obtenu, type: {associationRepository.GetType().FullName}");
            
            
            var association = await associationRepository.GetAssociationByName(associationName,jwt);
            LogMessage($"Association obtenue");

            if (association == null)
            {
                LogMessage($"No association with name {associationName} was found");
                throw new InvalidOperationException("IAssociationRepository service is not available.");
            }
            LogMessage($"Association obtenue, type: {association.Name}");

            var result = await planningRepository.GetAllAssociationEventsAsync(association.Id,jwt);
            LogMessage($"Résultat obtenu {result.ToString()}");

        return result;

        }
        catch (Exception e)
        {
            LogMessage($"Une erreur s'est déclenchée lors de la récupération l'association avec le nom {associationName}");
            LogMessage($"Erreur : {e.Message}");
            LogMessage($"StackTrace : {e.StackTrace}");
            
            LogMessage($"--------------------------------------------------------------------------------");
            return null;
        }
    }
    
    
     [McpServerTool, Description("Gets all the events of an association given its name for a user")]
    public  async Task<IEnumerable<EventDto>?> GetAllMyEvents()
    {
        LogMessage($"Début de récupération des évènementsde l'utilisateur connécté");
        LogMessage($"--------------------------------------------------------------------------------");
        try
        {
         
            LogMessage("Création du scope");

            LogMessage("Tentative d'obtention de IPlanningRepository");

            if (planningRepository == null)
            {
                LogMessage("IPlanningRepository est null - service non disponible");
                throw new InvalidOperationException("IPlanningRepository service is not available.");
            }

            LogMessage($"IPlanningRepository obtenu, type: {planningRepository.GetType().FullName}");
            var result = await planningRepository.GetAllMyEventsAsync();
            LogMessage($"Résultat obtenu {result.ToString()}");

        return result;

        }
        catch (Exception e)
        {
            LogMessage($"Une erreur s'est déclenchée lors de la récupération de mes evenements");
            LogMessage($"Erreur : {e.Message}");
            LogMessage($"StackTrace : {e.StackTrace}");
            
            LogMessage($"--------------------------------------------------------------------------------");
            return null;
        }
    }
    
    
    
    /*
    [McpServerTool, Description("Gets all the events of an association given by user")]
    public static async Task<IEnumerable<EventDto>?> GetAllMyEventsOfAnAssociation(Guid associationId,Guid userId)
    {
        LogMessage($"Début de récupération des évènements de l'association avec l'ID {associationId}");
        LogMessage($"--------------------------------------------------------------------------------");
        try
        {
            if (_serviceProvider == null)
            {
                LogMessage("ServiceProvider est null, impossible de continuer");
                return null;
            }

            LogMessage("Création du scope");
            using var scope = _serviceProvider.CreateScope();

            LogMessage("Tentative d'obtention de IPlanningRepository");
            var repository = scope.ServiceProvider.GetService<IPlanningRepository>();

            if (repository == null)
            {
                LogMessage("IPlanningRepository est null - service non disponible");
                throw new InvalidOperationException("IStockRepository service is not available.");
            }

            LogMessage($"IPlanningRepository obtenu, type: {repository.GetType().FullName}");
            LogMessage($"Appel de GetAllAssociationEventsAsync");

            var result = await repository.GetAllAssociationEventsAsync(associationId);

            LogMessage($"Résultat obtenu");

            return result;

        }
        catch (Exception e)
        {
            LogMessage($"Une erreur s'est déclenchée lors de la récupération des évènements de l'association avec l'ID {associationId}");
            LogMessage($"Erreur : {e.Message}");
            LogMessage($"StackTrace : {e.StackTrace}");
            
            LogMessage($"--------------------------------------------------------------------------------");
            return null;
        }
    }
    */

}
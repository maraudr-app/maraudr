using System.ComponentModel;
using Maraudr.MCP.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;

namespace Maraudr.MCP.Server.Tools;

[McpServerToolType]
public static class Tools
{
    private static IServiceProvider? _serviceProvider;
    
        public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [McpServerTool, Description("Gets the data of an item given its barcode for an association")]
    public static async Task<StockItemDto> GetStock(string barcode,Guid associationId)
    {
        var stockRepository  = _serviceProvider?.GetService<IStockRepository>();
        return await stockRepository.GetStockItemByBarCodeAsync(barcode,associationId) ?? null;
    }

    [McpServerTool, Description("Gets all the items of the stock given the id of an association")]
    public static async Task<IEnumerable<StockItemDto>> GetStock(Guid associationId)
    {
        var stockRepository  = _serviceProvider?.GetService<IStockRepository>();
        return await stockRepository.GetStockItemsAsync(associationId) ?? null;
    }

    [McpServerTool,
     Description("Gets all the items of the stock given the id of an association where the type matches entry")]
    public static async Task<IEnumerable<StockItemDto>> GetStock(Category type, Guid associationId)
    {
        var stockRepository  = _serviceProvider?.GetService<IStockRepository>();
        return await stockRepository.GetStockItemByTypeAsync(type,associationId) ?? null;
    }
   
    
}
using System.ComponentModel;
using Maraudr.MCP.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;

namespace Maraudr.MCP.Server.Tools;

[McpServerToolType]
public static class Tools
{
    private static IServiceProvider? _serviceProvider;
    
    // This would be injected by the MCP server framework
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

    
}
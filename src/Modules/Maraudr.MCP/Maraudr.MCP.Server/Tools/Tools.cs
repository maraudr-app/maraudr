using System.ComponentModel;
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

    [McpServerTool, Description("Gets current weather for a location")]
    public static async Task<string> GetStock(string location)
    {
        var weatherService = _serviceProvider?.GetService<IStockService>();
        return await weatherService?.GetWeatherAsync(location) ?? "Weather service unavailable";
    }

    [McpServerTool, Description("Queries the database")]
    public static async Task<string> QueryDatabase(string query)
    {
        var dbService = _serviceProvider?.GetService<IDatabaseService>();
        return await dbService?.ExecuteQueryAsync(query) ?? "Database service unavailable";
    }
}
using Maraudr.MCP.Domain.Interfaces;
using Maraudr.MCP.Infrastructure.McpClient;
using Maraudr.MCP.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Client;

namespace Maraudr.MCP.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register MCP Client services first
        services.AddMcpClient(configuration);
        
        // Then register repositories that depend on MCP Client
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<IPlanningRepository, PlanningRepository>();
        services.AddScoped<IAssociationRepository, AssociationRepository>();
        
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IMCPRepository, MCPRepository>();
        services.AddScoped<IGeoRepository, GeoRepository>();
        services.AddScoped<IDisponibilityRepository, DisponbilityRepository>();
        
        services.AddHttpClient<StockRepository>(client =>
        {
            var stockApiUrl = configuration["ApiSettings:StockApiUrl"];
            if (!string.IsNullOrEmpty(stockApiUrl))
            {
                client.BaseAddress = new Uri(stockApiUrl);
            }
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        
        // Also register HttpClient for AssociationRepository
        services.AddHttpClient<AssociationRepository>(client =>
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
    }

    public static void AddMcpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<McpClient.McpClientOptions>(
            configuration.GetSection(McpClient.McpClientOptions.SectionName));

        services.AddSingleton<McpClientService>();

        services.AddSingleton<IMcpClient>(provider =>
        {
            var clientService = provider.GetRequiredService<McpClientService>();
            return clientService.GetClientAsync().GetAwaiter().GetResult();
        });
    }
}
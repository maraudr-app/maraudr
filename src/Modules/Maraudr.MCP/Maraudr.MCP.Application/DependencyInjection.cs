using MCP.Maraudr.Application.Services;
using MCP.Maraudr.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MCP.Maraudr.Application;
public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IChatService,ChatService>();
        services.AddScoped<IToolService,ToolService>();
        
    }
}